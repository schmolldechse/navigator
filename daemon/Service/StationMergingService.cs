using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using daemon.Models.Database;
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
        { "Hochgeschwindigkeitszuege", 3 },
        { "IntercityUndEurocityZuege", 2 },
        { "InterregioUndSchnellzuege", 1 },
        { "NahverkehrsonstigeZuege", 1 },
        { "Sbahnen", 1 },
        { "UBahn", 0.5 },
        { "Schiffe", 0.2 },
        { "Strassenbahn", 0.2 },
        { "Busse", 0.1 },
        { "AnrufpflichtigeVerkehre", 0.1 }
    };

    public StationMergingService(ILogger<StationMergingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");

        if (!Directory.Exists(_tempFolder)) Directory.CreateDirectory(_tempFolder);
        _stadaFile = Path.Combine(_tempFolder, "stada.json");
    }

    public async Task<List<Station>> MergeFiles(Dictionary<int, StadaStationInfo>? stadaStations, CancellationToken cancellationToken = default)
    {
        stadaStations ??= await LoadStadaStationsAsync(cancellationToken);
        
        var risFiles = Directory.GetFiles(_tempFolder).Where(file => !file.EndsWith("stada.json")).ToList();
        if (!risFiles.Any()) throw new InvalidOperationException("No RIS files found in the temp folder");

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
                if (!stationElement.TryGetProperty("evaNumber", out var evaNumberElement) ||
                    evaNumberElement.ValueKind != JsonValueKind.String)
                    continue;
                int evaNumber = int.Parse(evaNumberElement.GetString()!);

                // groupEvaNumbers: includes the current evaNumber and all groupMembers
                var groupEvaNumbers = stationElement.GetProperty("groupMembers").EnumerateArray()
                    .Where(entry => entry.ValueKind == JsonValueKind.String)
                    .Select(entry => int.Parse(entry.GetString()!))
                    .Append(evaNumber)
                    .ToArray();

                var products = stationElement.GetProperty("availableTransports").EnumerateArray()
                    .Select(product => Product.MapProduct(product.GetString()))
                    .Distinct()
                    .ToList();
                var name = stationElement.GetProperty("names").GetProperty("DE").GetProperty("nameLong").GetString()!;

                foreach (var groupEvaNumber in groupEvaNumbers)
                {
                    stations.AddOrUpdate(groupEvaNumber, (_) => new Station()
                    {
                        EvaNumber = groupEvaNumber,
                        Name = stationElement.GetProperty("names").GetProperty("DE").GetProperty("nameLong")
                            .GetString()!,
                        Products = products.Select(product => new Product()
                        {
                            EvaNumber = groupEvaNumber,
                            ProductName = product,
                            QueryingEnabled = false
                        }).ToList(),
                        Coordinates = new Coordinates()
                        {
                            Latitude = stationElement.GetProperty("position").GetProperty("latitude").GetDouble(),
                            Longitude = stationElement.GetProperty("position").GetProperty("longitude").GetDouble()
                        },
                        Ril100 = GetRil100Identifiers(groupEvaNumbers, stadaStations).Select(ril100 => new Ril100()
                            {
                                EvaNumber = groupEvaNumber,
                                Ril100Identifier = ril100
                            }).ToList()
                    }, (_, existingStation) =>
                    {
                        if (evaNumber == groupEvaNumber)
                        {
                            existingStation.Name = name;
                            existingStation.Coordinates = new Coordinates()
                            {
                                Latitude = stationElement.GetProperty("position").GetProperty("latitude").GetDouble(),
                                Longitude = stationElement.GetProperty("position").GetProperty("longitude").GetDouble()
                            };
                        }

                        existingStation.Products = existingStation.Products.Union(products.Select(product =>
                            new Product()
                            {
                                EvaNumber = groupEvaNumber,
                                ProductName = product,
                                QueryingEnabled = false
                            })).DistinctBy(product => product.ProductName).ToList();

                        existingStation.Ril100 = existingStation.Ril100.Union(
                            GetRil100Identifiers(groupEvaNumbers, stadaStations).Select(ril100 => new Ril100()
                            {
                                EvaNumber = groupEvaNumber,
                                Ril100Identifier = ril100
                            })).DistinctBy(ril100 => ril100.Ril100Identifier).ToList();
                        return existingStation;
                    });
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
            await File.ReadAllTextAsync(_stadaFile, Encoding.UTF8, cancellationToken));

        var root = content.GetProperty("Result").GetProperty("result");
        if (root.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException("Invalid STADA data format");

        var stations = root.EnumerateArray().ToList();

        Parallel.ForEach(stations, station =>
        {
            var evaNumbers = station.GetProperty("evaNumbers").EnumerateArray()
                .Where(element => element.ValueKind == JsonValueKind.Object)
                .Select(element =>
                {
                    if (element.GetProperty("number").ValueKind != JsonValueKind.Number) 
                        throw new InvalidOperationException("Invalid EVA number format in STADA data");
                    return (element.GetProperty("number").GetInt32(), element.GetProperty("isMain").GetBoolean());
                })
                .ToArray();

            var ril100Identifiers = station.GetProperty("ril100Identifiers").EnumerateArray()
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
                        PriceCategory = evaNumber.Item2 ? station.GetProperty("priceCategory").GetInt16() : null
                    };
                }
            }
        });
        return stadaStations;
    }

    public double CalculateWeight(Station origin, int priceCategory = -1)
    {
        var weight = 0.1;
        foreach (var product in origin.Products)
        {
            if (!_productWeights.Keys.Any(key => string.Equals(key, product.ProductName, StringComparison.OrdinalIgnoreCase))) continue;
            weight += _productWeights[product.ProductName];
        }

        if (priceCategory != -1) weight += Math.Pow(2, Math.Max(LowestPriceCategory + 1 - priceCategory, 0)) * PriceCategoryWeight;
        return Math.Max(0.1, Math.Round(Math.Pow(weight, 3) * 10) / 10);
    }

    private string[] GetRil100Identifiers(int[] evaNumbers, Dictionary<int, StadaStationInfo> stadaStations) => evaNumbers
        .SelectMany(evaNumber => stadaStations.GetValueOrDefault(evaNumber)?.Ril100Identifiers ?? [])
        .Distinct()
        .ToArray();
}

public class StadaStationInfo
{
    public string[] Ril100Identifiers { get; set; } = [];
    public int? PriceCategory { get; set; }
}