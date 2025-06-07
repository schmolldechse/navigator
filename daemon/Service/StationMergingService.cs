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

    public StationMergingService(ILogger<StationMergingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");

        if (!Directory.Exists(_tempFolder)) Directory.CreateDirectory(_tempFolder);
        _stadaFile = Path.Combine(_tempFolder, "stada.json");
    }

    public async Task<List<Station>> MergeFiles(CancellationToken cancellationToken = default)
    {
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
                        }
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
                            })).DistinctBy(x => x.ProductName).ToList();
                        return existingStation;
                    });
                }
            }
        });
        await Task.WhenAll(tasks);
        return stations.Values.ToList();
    }

    public async Task<List<Station>> MergeWithStada(List<Station> stations, CancellationToken cancellationToken = default)
    {
        var stadaStations = await LoadStadaStationsAsync(cancellationToken);
        foreach (var station in stations)
        {
            if (!stadaStations.ContainsKey(station.EvaNumber)) continue;
            station.Ril100 = stadaStations.GetValueOrDefault(station.EvaNumber, []).Select(ril100 => new Ril100()
            {
                EvaNumber = station.EvaNumber,
                Ril100Identifier = ril100
            }).ToList();
        }
        return stations;
    } 

    private async Task<Dictionary<int, string[]>> LoadStadaStationsAsync(CancellationToken cancellationToken)
    {
        var stadaStations = new Dictionary<int, string[]>();

        var content = JsonSerializer.Deserialize<JsonElement>(
            await File.ReadAllTextAsync(_stadaFile, Encoding.UTF8, cancellationToken));

        var root = content.GetProperty("Result").GetProperty("result");
        if (root.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException("Invalid STADA data format");

        var stations = root.EnumerateArray().ToList();

        Parallel.ForEach(stations, station =>
        {
            var evaNumbers = station.GetProperty("evaNumbers").EnumerateArray()
                .Where(e => e.ValueKind == JsonValueKind.Object)
                .Where(e => e.GetProperty("number").ValueKind == JsonValueKind.Number)
                .Select(e => e.GetProperty("number").GetInt32())
                .ToArray();

            var ril100Identifiers = station.GetProperty("ril100Identifiers").EnumerateArray()
                .Where(e => e.ValueKind == JsonValueKind.Object)
                .Select(e => e.GetProperty("rilIdentifier").GetString()!)
                .ToArray();

            foreach (var evaNumber in evaNumbers)
            {
                lock (stadaStations)
                {
                    stadaStations[evaNumber] = ril100Identifiers;
                }
            }
        });
        return stadaStations;
    }

    private string[] GetRil100Identifiers(int[] evaNumbers, Dictionary<int, string[]> stadaStations) => evaNumbers
        .SelectMany(evaNumber => stadaStations.GetValueOrDefault(evaNumber, []))
        .Distinct()
        .ToArray();
}