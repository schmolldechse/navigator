using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using stations.Models.Database;
using System.Collections.Concurrent;

namespace stations;

public class StationMergeService
{
    private readonly ILogger<StationMergeService> _logger;

    private readonly string _tempFolder = Path.Combine(Path.GetTempPath(), "navigator", "stations");
    private readonly string _stadaFile;

    private readonly Dictionary<string, string[]> _productMapping = new()
    {
        { "Hochgeschwindigkeitszuege", new[] { "HIGH_SPEED_TRAIN", "nationalExpress" } },
        { "IntercityUndEurocityZuege", new[] { "INTERCITY_TRAIN", "national" } },
        { "InterregioUndSchnellzuege", new[] { "INTER_REGIONAL_TRAIN", "regionalExpress" } },
        { "NahverkehrsonstigeZuege", new[] { "REGIONAL_TRAIN", "regional" } },
        { "Sbahnen", new[] { "CITY_TRAIN", "suburban" } },
        { "Busse", new[] { "BUS" } },
        { "Schiffe", new[] { "FERRY" } },
        { "UBahn", new[] { "SUBWAY" } },
        { "Strassenbahn", new[] { "TRAM" } },
        { "AnrufpflichtigeVerkehre", new[] { "SHUTTLE", "taxi" } },
        { "Unknown", new string[] { } }
    };

    public StationMergeService(ILogger<StationMergeService> logger)
    {
        _logger = logger;

        if (!Directory.Exists(_tempFolder)) Directory.CreateDirectory(_tempFolder);
        _stadaFile = Path.Combine(_tempFolder, "stada.json");
    }

    private string MapProduct(string? transport)
    {
        if (string.IsNullOrEmpty(transport)) return "Unknown";
        foreach (var pair in _productMapping)
        {
            if (pair.Value.Any(value => string.Equals(value, transport, StringComparison.OrdinalIgnoreCase)))
                return pair.Key;
        }
        return "Unknown";
    }

    public async Task<List<Station>> MergeStationsAsync(CancellationToken cancellationToken = default)
    {
        var stadaStations = await LoadStadaStationsAsync(cancellationToken);
        var risFiles = Directory.GetFiles(_tempFolder).Where(file => !file.EndsWith("stada.json")).ToList();
        if (!risFiles.Any()) throw new InvalidOperationException("No RIS files found in the temp folder");

        var allStations = new ConcurrentDictionary<int, Station>();

        var risFileTasks = risFiles.Select(async file =>
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

                // groupEvaNumbers
                var groupEvaNumbers = stationElement.GetProperty("groupMembers").EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String)
                    .Select(e => int.Parse(e.GetString()!))
                    .Append(evaNumber)
                    .ToArray();

                var ril100Identifiers = GetRil100Identifiers(groupEvaNumbers, stadaStations);
                var name = stationElement.GetProperty("names").GetProperty("DE").GetProperty("nameLong").GetString()!;
                var coordinates = new Coordinates
                {
                    Latitude = stationElement.GetProperty("position").GetProperty("latitude").GetDouble(),
                    Longitude = stationElement.GetProperty("position").GetProperty("longitude").GetDouble()
                };
                var products = stationElement.GetProperty("availableTransports").EnumerateArray()
                    .Select(p => MapProduct(p.GetString()))
                    .Distinct()
                    .ToList();

                foreach (var iteratingEvaNumber in groupEvaNumbers)
                {
                    allStations.AddOrUpdate(iteratingEvaNumber,
                        _ => new Station
                        {
                            Name = name,
                            EvaNumber = iteratingEvaNumber,
                            Coordinates = coordinates,
                            Products = products,
                            Ril100 = ril100Identifiers.Distinct().ToList()
                        },
                        (_, existingStation) =>
                        {
                            existingStation.Name = name;
                            existingStation.EvaNumber = iteratingEvaNumber;
                            existingStation.Ril100 = existingStation.Ril100.Union(ril100Identifiers).Distinct().ToList();
                            return existingStation;
                        });
                }
            }
        });

        await Task.WhenAll(risFileTasks);
        return allStations.Values.ToList();
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
