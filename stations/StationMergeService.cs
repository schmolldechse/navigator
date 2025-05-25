using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using stations.Models.Database;

namespace stations;

public class StationMergeService
{
    private readonly ILogger<StationMergeService> _logger;

    private readonly string _tempFolder = Path.Combine(Path.GetTempPath(), "navigator", "stations");
    private readonly string _stadaFile;

    private readonly Dictionary<string, string[]> ProductMapping = new()
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
        if (string.IsNullOrEmpty(transport))
            return "Unknown";
        foreach (var pair in ProductMapping)
        {
            if (pair.Value.Any(value => string.Equals(value, transport, StringComparison.OrdinalIgnoreCase)))
                return pair.Key;
        }

        return "Unknown";
    }

    public async Task<List<Station>> MergeStationsAsync(CancellationToken cancellationToken = default)
    {
        var stadaStations = new Dictionary<int, string[]>();
        {
            var content = JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync(_stadaFile, Encoding.UTF8));
            var root = content.GetProperty("Result").GetProperty("result");
            if (root.ValueKind != JsonValueKind.Array)
                throw new InvalidOperationException("Invalid STADA data format");

            root.EnumerateArray().ToList().ForEach(station =>
            {
                var evaNumbers = station.GetProperty("evaNumbers").EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.Object)
                    .Where(e => e.GetProperty("number").ValueKind == JsonValueKind.Number)
                    .Select(e => e.GetProperty("number").GetInt32())
                    .ToArray();
                var ril100Identifiers = station.GetProperty("ril100Identifiers").EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.Object)
                    .Select(e => e.GetProperty("rilIdentifier").GetString())
                    .Where(ril => !string.IsNullOrWhiteSpace(ril))
                    .Select(ril => ril!)
                    .ToArray();

                evaNumbers.AsEnumerable().ToList()
                    .ForEach(evaNumber => stadaStations.Add(evaNumber, ril100Identifiers));
            });
        }
        
        var stations = new List<Station>();
        {
            var risFiles = Directory.GetFiles(_tempFolder).Where(file => !file.EndsWith("stada.json")).ToList();
            if (!risFiles.Any()) throw new InvalidOperationException("No RIS files found in the temp folder");

            risFiles.ForEach(file =>
            {
                var document = JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(file, Encoding.UTF8));
                var root = document.GetProperty("stopPlaces");
                if (root.ValueKind != JsonValueKind.Array)
                    throw new InvalidOperationException($"Invalid RIS data format in file {file}");

                root.EnumerateArray().ToList().ForEach(stationDocument =>
                {
                    var evaNumberProperty = stationDocument.GetProperty("evaNumber");
                    if (evaNumberProperty.ValueKind != JsonValueKind.String)
                        throw new InvalidOperationException("Invalid EVA number format in RIS data");

                    int.TryParse(stationDocument.GetProperty("evaNumber").GetString(), out var evaNumber);
                    var existingStation = stations.FirstOrDefault(station => station.EvaNumber == evaNumber);
                    if (existingStation != null)
                    {
                        // Adjust the existing Station object as needed
                        existingStation.Name = stationDocument.GetProperty("names").GetProperty("DE").GetProperty("nameLong")
                            .GetString() ?? existingStation.Name;
                        existingStation.Ril100 = stadaStations.GetValueOrDefault(evaNumber, existingStation.Ril100?.ToArray() ?? []).ToList();
                        existingStation.Products = stationDocument.GetProperty("availableTransports").EnumerateArray()
                            .Select(product => MapProduct(product.GetString())).Distinct().ToList();
                        existingStation.Coordinates = new Coordinates()
                        {
                            Latitude = stationDocument.GetProperty("position").GetProperty("longitude").GetDouble(),
                            Longitude = stationDocument.GetProperty("position").GetProperty("latitude").GetDouble()
                        };
                        return;
                    }

                    var station = new Station()
                    {
                        EvaNumber = evaNumber,
                        Name = stationDocument.GetProperty("names").GetProperty("DE").GetProperty("nameLong")
                            .GetString() ?? throw new ArgumentNullException("Name", "Station name is missing"),
                        Ril100 = stadaStations.GetValueOrDefault(evaNumber, []).ToList(),
                        Products = stationDocument.GetProperty("availableTransports").EnumerateArray()
                            .Select(product => MapProduct(product.GetString())).Distinct().ToList(),
                        Coordinates = new Coordinates()
                        {
                            Latitude = stationDocument.GetProperty("position").GetProperty("longitude").GetDouble(),
                            Longitude = stationDocument.GetProperty("position").GetProperty("latitude").GetDouble()
                        }
                    };

                    var groupMembers = stationDocument.GetProperty("groupMembers").EnumerateArray().ToList()
                        .Where(groupMember => groupMember.ValueKind == JsonValueKind.String)
                        .Where(groupMember => int.TryParse(groupMember.GetString(), out var groupEvaNumber) &&
                                              groupEvaNumber != evaNumber)
                        .Select(groupMember =>
                        {
                            int.TryParse(groupMember.GetString(), out var groupEvaNumber);
                            var ril100 = stadaStations.GetValueOrDefault(evaNumber, [])
                                .Concat(stadaStations.GetValueOrDefault(groupEvaNumber, [])).Distinct().ToList();
                            return new Station()
                            {
                                Name = station.Name,
                                EvaNumber = groupEvaNumber,
                                Ril100 = ril100,
                                Products = station.Products,
                                Coordinates = station.Coordinates
                            };
                        })
                        .Append(station);
                    stations.AddRange(groupMembers);
                });
            });
            return stations;
        }
    }
}
