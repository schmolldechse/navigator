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

    public async Task<List<Station>> MergeStationsAsync(CancellationToken cancellationToken = default)
    {
        var evaNumberRilDict = await LoadStadaStationsAsync(cancellationToken);
        var rilProductsDict = new ConcurrentDictionary<string, HashSet<string>>(); // use HashSet for unique products
        
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

                // groupEvaNumbers: includes the current evaNumber and all groupMembers
                var groupEvaNumbers = stationElement.GetProperty("groupMembers").EnumerateArray()
                    .Where(entry => entry.ValueKind == JsonValueKind.String)
                    .Select(entry => int.Parse(entry.GetString()!))
                    .Append(evaNumber)
                    .ToArray();

                // parameters
                var ril100Identifiers = GetRil100Identifiers(groupEvaNumbers, evaNumberRilDict);
                var products = stationElement.GetProperty("availableTransports").EnumerateArray()
                    .Select(product => Product.MapProduct(product.GetString()))
                    .Concat(GetProductsByRil(ril100Identifiers, rilProductsDict))
                    .Distinct()
                    .ToList();
                var name = stationElement.GetProperty("names").GetProperty("DE").GetProperty("nameLong").GetString()!;

                // save products by ril100
                ril100Identifiers.ToList().ForEach(ril100 => rilProductsDict.AddOrUpdate(
                    ril100,
                    _ => [..products],
                    (_, existingProducts) => [..existingProducts, ..products]
                ));

                foreach (var iteratingEvaNumber in groupEvaNumbers)
                {
                    allStations.AddOrUpdate(iteratingEvaNumber,
                        _ => new Station
                        {
                            Name = name,
                            EvaNumber = iteratingEvaNumber,
                            Coordinates = new()
                            {
                                Latitude = stationElement.GetProperty("position").GetProperty("latitude").GetDouble(),
                                Longitude = stationElement.GetProperty("position").GetProperty("longitude").GetDouble()
                            },
                            Ril100 = ril100Identifiers.Select(rill100 => new Ril100()
                            {
                                Ril100Identifier = rill100,
                                EvaNumber = iteratingEvaNumber
                            }).ToList(),
                            Products = products.Select(product => new Product
                            {
                                EvaNumber = iteratingEvaNumber,
                                ProductName = product,
                                QueryingEnabled = false
                            }).ToList(),
                        },
                        (_, existingStation) =>
                        {
                            existingStation.Name = name;
                            existingStation.EvaNumber = iteratingEvaNumber;
                            existingStation.Ril100 = existingStation.Ril100.Union(ril100Identifiers.Select(rill100 => new Ril100()
                            {
                                Ril100Identifier = rill100,
                                EvaNumber = iteratingEvaNumber
                            }).ToList()).DistinctBy(x => x.Ril100Identifier).ToList();
                            
                            // this update process is necessary if the station itself does not have any "ril100" identifier
                            existingStation.Products = existingStation.Products.Union(products.Select(product => new Product
                            {
                                EvaNumber = iteratingEvaNumber,
                                ProductName = product,
                                QueryingEnabled = false
                            }).ToList()).DistinctBy(x => x.ProductName).ToList();
                            return existingStation;
                        });
                }
            }
        });
        await Task.WhenAll(risFileTasks);
        
        // update products by ril100 identifiers
        allStations.Values.Where(station => station.Ril100.Any(ril100 => rilProductsDict.ContainsKey(ril100.Ril100Identifier)))
            .AsParallel()
            .ForAll(station =>
            {
                var ril100Identifiers = station.Ril100
                    .Select(ril100 => ril100.Ril100Identifier)
                    .ToArray();

                station.Products = station.Products.Concat(GetProductsByRil(ril100Identifiers, rilProductsDict)
                    .Select(product => new Product()
                    {
                        EvaNumber = station.EvaNumber,
                        ProductName = product,
                        QueryingEnabled = false
                    })).DistinctBy(x => x.ProductName).ToList();
            });
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

    private string[] GetProductsByRil(string[] ril100Identifiers,
        ConcurrentDictionary<string, HashSet<string>> productsByRil) => ril100Identifiers
        .Where(productsByRil.ContainsKey)
        .SelectMany(ril => productsByRil[ril])
        .Distinct()
        .ToArray();
}