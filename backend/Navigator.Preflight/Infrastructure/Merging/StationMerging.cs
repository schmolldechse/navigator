using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.Station;
using Navigator.Data.Enums;
using Navigator.Data.Models.Ris;
using Navigator.Data.Models.StaDa;
using Navigator.Preflight.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace Navigator.Preflight.Infrastructure.Merging;

public class StationMerging(
    ILogger<StationMerging> logger
) : IStationMerging
{
    private readonly string _tempPath = Path.Combine(Path.GetTempPath(), "navigator", "station_discovery");

    private readonly Dictionary<TransportType, double> _transportWeights = new()
    {
        { TransportType.HighSpeedTrain, 3 },
        { TransportType.IntercityTrain, 2 },
        { TransportType.InterRegionalTrain, 1 },
        { TransportType.RegionalTrain, 1 },
        { TransportType.CityTrain, 1 },
        { TransportType.Subway, 0.5 },
        { TransportType.Ferry, 0.2 },
        { TransportType.Tram, 0.2 },
        { TransportType.Bus, 0.1 },
        { TransportType.Taxi, 0.1 },
        { TransportType.Shuttle, 0.1 },
    };
    private const int _lowestPriceCategory = 8;
    private const double _priceCategoryWeight = 0.5;

    public async Task<IEnumerable<Station>> StartMergingAsync()
    {
        if (!Directory.Exists(_tempPath)) throw new DirectoryNotFoundException("Station discovery temp path not found.");

        logger.LogInformation("Starting to merge StaDa with RIS stations...");
        var watch = new Stopwatch();
        watch.Start();

        logger.LogInformation("Loading StaDa stations in cache...");
        var stadaStations = await LoadStadaStationsAsync();

        logger.LogInformation("Loading RIS stations in cache...");
        var risStations = await LoadRisStationsAsync();

        logger.LogInformation("Loaded {RisCount} RIS stations and {StaDaCount} StaDa stations in {ElapsedMilliseconds}ms", risStations.Count, stadaStations.Count, watch.ElapsedMilliseconds);

        ConcurrentDictionary<int, Station> mappedStations = new ConcurrentDictionary<int, Station>();
        foreach (var (evaNumber, risStation) in risStations)
        {
            var groupedEvaNumbers = risStation.GroupMembers
                .Select(entry => int.TryParse(entry, out var evaNum) ? evaNum : -1)
                .Where(entry => entry != -1)
                .Append(evaNumber)
                .ToArray();
            var products = risStation.AvailableTransports
                .Distinct()
                .ToList();

            foreach (var groupedEvaNumber in groupedEvaNumbers)
            {
                mappedStations.AddOrUpdate(
                    groupedEvaNumber,
                    (_) => new Station()
                    {
                        EvaNumber = groupedEvaNumber,
                        Name = risStation.Names.Keys
                            .Where(nameKey => nameKey.Equals("DE", StringComparison.OrdinalIgnoreCase))
                            .Select(nameKey => risStation.Names[nameKey])
                            .FirstOrDefault()?.NameLong ?? "Unknown mapped station",
                        Transports = products
                            .Select(product => new StationTransport()
                            {
                                EvaNumber = groupedEvaNumber,
                                TransportType = MapToNavigatorTransportType(product),
                                Enabled = false
                            })
                            .ToList(),
                        Latitude = risStation.Position.Latitude,
                        Longitude = risStation.Position.Longitude,
                        Ril100 = GetRil100(groupedEvaNumbers, stadaStations)
                            .Select(ril100 => new StationRil100()
                            {
                                EvaNumber = groupedEvaNumber,
                                Ril100Code = ril100
                            })
                            .ToList(),
                        QueryingEnabled = false,
                        Weight = 0.0
                    },
                    (_, existingStation) =>
                    {
                        if (evaNumber == groupedEvaNumber)
                        {
                            existingStation.Name = risStation.Names.Keys
                                .Where(nameKey => nameKey.Equals("DE", StringComparison.OrdinalIgnoreCase))
                                .Select(nameKey => risStation.Names[nameKey])
                                .FirstOrDefault()?.NameLong ?? existingStation.Name;
                            existingStation.Latitude = risStation.Position.Latitude;
                            existingStation.Longitude = risStation.Position.Longitude;
                        }

                        existingStation.Transports = existingStation.Transports
                            .Union(products
                                .Select(product => new StationTransport()
                                {
                                    EvaNumber = groupedEvaNumber,
                                    TransportType = MapToNavigatorTransportType(product),
                                    Enabled = false
                                }))
                            .DistinctBy(transport => transport.TransportType)
                            .ToList();
                        existingStation.Ril100 = existingStation.Ril100
                            .Union(GetRil100(groupedEvaNumbers.ToArray(), stadaStations)
                                .Select(ril100 => new StationRil100()
                                {
                                    EvaNumber = groupedEvaNumber,
                                    Ril100Code = ril100
                                }))
                            .DistinctBy(ril => ril.Ril100Code)
                            .ToList();
                        return existingStation;
                    });
            }
        }
        logger.LogInformation("Mapped a total of {MappedCount} stations in {ElapsedMilliseconds}ms", mappedStations.Count, watch.ElapsedMilliseconds);

        logger.LogInformation("Calculating weights for mapped stations...");
        watch.Restart();

        foreach (var station in mappedStations.Values)
        {
            station.Weight = CalculateWeight(station, stadaStations.GetValueOrDefault(station.EvaNumber)?.PriceCategory ?? -1);
        }
        logger.LogInformation("Weight calculation done in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);

        watch.Stop();
        return mappedStations.Values;
    }

    private async Task<ConcurrentDictionary<int, RisStations.StopPlaceSearchResult>> LoadRisStationsAsync()
    {
        var risFiles = Directory.GetFiles(_tempPath)
            .Where(file => !file.EndsWith("stada.json") && !file.EndsWith("manifest.json"))
            .ToList();
        if (!risFiles.Any())
        {
            logger.LogError("No RIS station files found in {TempPath}", _tempPath);
            throw new FileNotFoundException("No RIS station files found.", _tempPath);
        }

        ConcurrentDictionary<int, RisStations.StopPlaceSearchResult> stations = new ConcurrentDictionary<int, RisStations.StopPlaceSearchResult>();
        foreach (var risFile in risFiles)
        {
            var risStations = JsonSerializer.Deserialize<IEnumerable<RisStations.StopPlaceSearchResult>>(await File.ReadAllTextAsync(risFile));
            if (risStations is null)
            {
                logger.LogError("Failed to deserialize RIS stations from file {RisFile}", risFile);
                throw new InvalidOperationException("Failed to deserialize RIS stations.");
            }

            foreach (var station in risStations)
            {
                if (!int.TryParse(station.EvaNumber, out var evaNumber)) continue;
                stations.TryAdd(evaNumber, station);
            }
        }

        return stations;
    }

    private async Task<Dictionary<int, StadaStationInfo>> LoadStadaStationsAsync()
    {
        var stadaStations = new Dictionary<int, StadaStationInfo>();

        var stadaPath = Path.Combine(_tempPath, "stada.json");
        if (!File.Exists(stadaPath))
        {
            logger.LogError("StaDa station file not found at {StadaPath}", stadaPath);
            throw new FileNotFoundException("StaDa station file not found.", stadaPath);
        }

        var stations = JsonSerializer.Deserialize<IEnumerable<StaDa.Station>>(await File.ReadAllTextAsync(stadaPath));
        if (stations is null)
        {
            logger.LogError("Failed to deserialize StaDa stations from file {StadaPath}", stadaPath);
            throw new InvalidOperationException("Failed to deserialize StaDa stations.");
        }

        foreach (var station in stations)
        {
            var evaNumbers = station.EvaNumbers
                .Select(entry => (entry.Number, entry.IsMain))
                .ToArray();
            var ril100 = station.Ril100Identifiers
                .Select(entry => entry.RilIdentifier)
                .ToArray();

            foreach (var (evaNumber, isMain) in evaNumbers)
            {
                if (stadaStations.ContainsKey(evaNumber)) continue;

                stadaStations[evaNumber] = new StadaStationInfo()
                {
                    Ril100 = ril100,
                    PriceCategory = station.PriceCategory
                };
            }
        }

        return stadaStations;
    }

    private string[] GetRil100(int[] evaNumbers, Dictionary<int, StadaStationInfo> stadaStations) => evaNumbers
        .SelectMany(evaNumber => stadaStations.GetValueOrDefault(evaNumber)?.Ril100 ?? [])
        .Distinct()
        .ToArray();

    private TransportType MapToNavigatorTransportType(RisStations.TransportType transportType) => transportType switch
    {
        RisStations.TransportType.HIGH_SPEED_TRAIN => TransportType.HighSpeedTrain,
        RisStations.TransportType.INTERCITY_TRAIN => TransportType.IntercityTrain,
        RisStations.TransportType.INTER_REGIONAL_TRAIN => TransportType.InterRegionalTrain,
        RisStations.TransportType.REGIONAL_TRAIN => TransportType.RegionalTrain,
        RisStations.TransportType.CITY_TRAIN => TransportType.CityTrain,
        RisStations.TransportType.SUBWAY => TransportType.Subway,
        RisStations.TransportType.TRAM => TransportType.Tram,
        RisStations.TransportType.BUS => TransportType.Bus,
        RisStations.TransportType.FERRY => TransportType.Ferry,
        RisStations.TransportType.FLIGHT => TransportType.Flight,
        RisStations.TransportType.CAR => TransportType.Car,
        RisStations.TransportType.TAXI => TransportType.Taxi,
        RisStations.TransportType.SHUTTLE => TransportType.Shuttle,
        RisStations.TransportType.BIKE => TransportType.Bike,
        RisStations.TransportType.SCOOTER => TransportType.Scooter,
        RisStations.TransportType.WALK => TransportType.Walk,
        _ => TransportType.Unknown,
    };

    private double CalculateWeight(Station origin, int priceCategory = -1)
    {
        var weight = 0.1;
        foreach (var originTransport in origin.Transports)
        {
            if (!_transportWeights.Keys.Any(transport => transport == originTransport.TransportType)) continue;
            weight += _transportWeights[originTransport.TransportType];
        }

        if (priceCategory != -1)
            weight += Math.Pow(2, Math.Max(_lowestPriceCategory + 1 - priceCategory, 0)) * _priceCategoryWeight;
        return Math.Max(0.1, Math.Round(Math.Pow(weight, 3) * 10) / 10);
    }
}