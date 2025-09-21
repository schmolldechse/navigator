using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using daemon.Models.Database;
using daemon.Models.Database.Station;
using Microsoft.Extensions.Logging;

namespace daemon.Service;

public class StationMergingService
{
	private readonly ILogger<StationMergingService> _logger;

	private readonly string _tempFolder = Path.Combine(Path.GetTempPath(), "navigator", "station_discovery");
	private readonly string _stadaFile;

	private const int LowestPriceCategory = 8;
	private const double PriceCategoryWeight = 0.5;

	private readonly Dictionary<string, double> _productWeights = new()
	{
		{ "HIGH_SPEED_TRAIN", 3 },
		{ "INTERCITY_TRAIN", 2 },
		{ "INTER_REGIONAL_TRAIN", 1 },
		{ "REGIONAL_TRAIN", 1 },
		{ "CITY_TRAIN", 1 },
		{ "SUBWAY", 0.5 },
		{ "FERRY", 0.2 },
		{ "TRAM", 0.2 },
		{ "BUS", 0.1 },
		{ "SHUTTLE", 0.1 }
	};

	public StationMergingService(ILogger<StationMergingService> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");

		if (!Directory.Exists(_tempFolder))
			Directory.CreateDirectory(_tempFolder);
		_stadaFile = Path.Combine(_tempFolder, "stada.json");
	}

	public async Task<List<Station>> MergeFiles(
		Dictionary<int, StadaStationInfo>? stadaStations,
		CancellationToken cancellationToken = default
	)
	{
		stadaStations ??= await LoadStadaStationsAsync(cancellationToken);

		var risFiles = Directory.GetFiles(_tempFolder).Where(file => !file.EndsWith("stada.json")).ToList();
		if (!risFiles.Any())
			throw new InvalidOperationException("No RIS files found in the temp folder");

		ConcurrentDictionary<int, Station> stations = new ConcurrentDictionary<int, Station>();
		var tasks = risFiles.Select(async file =>
		{
			await using var fileStream = File.OpenRead(file);
			using var document = await JsonDocument.ParseAsync(fileStream, cancellationToken: cancellationToken);

			var root = document.RootElement.GetProperty("stopPlaces");
			if (root.ValueKind != JsonValueKind.Array)
				throw new InvalidOperationException($"Invalid RIS data format in file {file}");

			foreach (var stationElement in root.EnumerateArray().ToList())
			{
				if (
					!stationElement.TryGetProperty("evaNumber", out var evaNumberElement)
					|| evaNumberElement.ValueKind != JsonValueKind.String
				)
					continue;
				int evaNumber = int.Parse(evaNumberElement.GetString()!);

				// groupEvaNumbers: includes the current evaNumber and all groupMembers
				var groupEvaNumbers = stationElement
					.GetProperty("groupMembers")
					.EnumerateArray()
					.Where(entry => entry.ValueKind == JsonValueKind.String)
					.Select(entry => int.Parse(entry.GetString()!))
					.Append(evaNumber)
					.ToArray();

				var products = stationElement
					.GetProperty("availableTransports")
					.EnumerateArray()
					.Select(product => product.GetString())
					.Distinct()
					.ToList();
				var name = stationElement.GetProperty("names").GetProperty("DE").GetProperty("nameLong").GetString()!;

				foreach (var groupEvaNumber in groupEvaNumbers)
				{
					stations.AddOrUpdate(
						groupEvaNumber,
						(_) =>
							new Station()
							{
								EvaNumber = groupEvaNumber,
								Name = stationElement
									.GetProperty("names")
									.GetProperty("DE")
									.GetProperty("nameLong")
									.GetString()!,
								Products = products
									.Select(product => new TransportOccurence()
									{
										EvaNumber = groupEvaNumber,
										TransportType = ParseTransportType(product),
										QueryingEnabled = false,
									})
									.ToList(),
								Latitude = stationElement.GetProperty("position").GetProperty("latitude").GetDouble(),
								Longitude = stationElement.GetProperty("position").GetProperty("longitude").GetDouble(),
								Ril100 = GetRil100Identifiers(groupEvaNumbers, stadaStations)
									.Select(ril100 => new Ril100() { EvaNumber = groupEvaNumber, Ril100Identifier = ril100 })
									.ToList(),
							},
						(_, existingStation) =>
						{
							if (evaNumber == groupEvaNumber)
							{
								existingStation.Name = name;
								existingStation.Latitude = stationElement.GetProperty("position")
									.GetProperty("latitude").GetDouble();
								existingStation.Longitude = stationElement.GetProperty("position")
									.GetProperty("longitude").GetDouble();
							}

							existingStation.Products = existingStation
								.Products.Union(
									products.Select(product => new TransportOccurence()
									{
										EvaNumber = groupEvaNumber,
										TransportType = ParseTransportType(product),
										QueryingEnabled = false,
									})
								)
								.DistinctBy(product => product.TransportType)
								.ToList();

							existingStation.Ril100 = existingStation
								.Ril100.Union(
									GetRil100Identifiers(groupEvaNumbers, stadaStations)
										.Select(ril100 => new Ril100()
										{
											EvaNumber = groupEvaNumber,
											Ril100Identifier = ril100,
										})
								)
								.DistinctBy(ril100 => ril100.Ril100Identifier)
								.ToList();
							return existingStation;
						}
					);
				}
			}
		});
		await Task.WhenAll(tasks);
		return stations.Values.ToList();
	}

	public async Task<Dictionary<int, StadaStationInfo>> LoadStadaStationsAsync(CancellationToken cancellationToken)
	{
		var stadaStations = new Dictionary<int, StadaStationInfo>();

		var content = JsonSerializer.Deserialize<JsonElement>(
			await File.ReadAllTextAsync(_stadaFile, Encoding.UTF8, cancellationToken)
		);

		var root = content.GetProperty("Result").GetProperty("result");
		if (root.ValueKind != JsonValueKind.Array)
			throw new InvalidOperationException("Invalid STADA data format");

		var stations = root.EnumerateArray().ToList();

		Parallel.ForEach(
			stations,
			station =>
			{
				var evaNumbers = station
					.GetProperty("evaNumbers")
					.EnumerateArray()
					.Where(element => element.ValueKind == JsonValueKind.Object)
					.Select(element =>
					{
						if (element.GetProperty("number").ValueKind != JsonValueKind.Number)
							throw new InvalidOperationException("Invalid EVA number format in STADA data");
						return (element.GetProperty("number").GetInt32(), element.GetProperty("isMain").GetBoolean());
					})
					.ToArray();

				var ril100Identifiers = station
					.GetProperty("ril100Identifiers")
					.EnumerateArray()
					.Where(e => e.ValueKind == JsonValueKind.Object)
					.Select(e => e.GetProperty("rilIdentifier").GetString()!)
					.ToArray();

				foreach (var evaNumber in evaNumbers)
				{
					lock (stadaStations)
					{
						stadaStations[evaNumber.Item1] = new StadaStationInfo()
						{
							Ril100Identifiers = ril100Identifiers,
							PriceCategory = evaNumber.Item2 ? station.GetProperty("priceCategory").GetInt16() : null,
						};
					}
				}
			}
		);
		return stadaStations;
	}
	
	private TransportType ParseTransportType(string input)
	{
		if (Enum.TryParse<TransportType>(input, true, out var result))
			return result;
		throw new FormatException($"Unknown transport type: {input}");
	}

	public double CalculateWeight(Station origin, int priceCategory = -1)
	{
		var weight = 0.1;
		foreach (var product in origin.Products)
		{
			if (!_productWeights.Keys.Any(key => string.Equals(key, product.TransportType.ToString(), StringComparison.OrdinalIgnoreCase)))
				continue;
			weight += _productWeights[product.TransportType.ToString()];
		}

		if (priceCategory != -1)
			weight += Math.Pow(2, Math.Max(LowestPriceCategory + 1 - priceCategory, 0)) * PriceCategoryWeight;
		return Math.Max(0.1, Math.Round(Math.Pow(weight, 3) * 10) / 10);
	}

	private string[] GetRil100Identifiers(int[] evaNumbers, Dictionary<int, StadaStationInfo> stadaStations) =>
		evaNumbers
			.SelectMany(evaNumber => stadaStations.GetValueOrDefault(evaNumber)?.Ril100Identifiers ?? [])
			.Distinct()
			.ToArray();
}

public class StadaStationInfo
{
	public string[] Ril100Identifiers { get; set; } = [];
	public int? PriceCategory { get; set; }
}
