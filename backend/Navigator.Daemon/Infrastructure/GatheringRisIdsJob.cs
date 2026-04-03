using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.RisId;
using Navigator.Data.Entities.Station;
using Navigator.Data.Enums;
using Navigator.Data.Models.Ris;
using Navigator.Data.Models.Station;
using Navigator.Data.Repository.RisIdRepository;
using Navigator.Data.Repository.StationRepository;
using Navigator.Data.Repository.TimetableRepository;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class GatheringRisIdsJob(
    ILogger<GatheringRisIdsJob> logger,
    IStationRepository stationRepository,
    ITimetableRepository timetableRepository,
    IRisIdRepository risIdRepository
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var randomStation = await stationRepository.GetRandomStationAsync(new ShuffledStationRequest()
        {
            OnlyIncludeActive = true,
            LastSeen = DateTime.UtcNow.Date.AddDays(-1)
        });
        if (randomStation == null) return;

        DateTime? lastQueried = EstimateLastQueried(randomStation);
        if (lastQueried == null) return;

        logger.LogInformation("Querying {StationName} (EvaNumber: {StationEvaNumber}) with last queried to {LastQueried}.",
            randomStation.Name,
            randomStation.EvaNumber,
            lastQueried);

        // it is necessary to create separate time variable here, we want to use ISO 8601 format with offset when querying the boards
        DateTimeOffset boardRequestTime = lastQueried.Value;
        var (arrivalBoard1, arrivalBoard2) = (
            timetableRepository.GetArrivalsAsync(new() { EvaNumber = randomStation.EvaNumber, TimeStart = boardRequestTime.Date, Duration = 720 }),
            timetableRepository.GetArrivalsAsync(new() { EvaNumber = randomStation.EvaNumber, TimeStart = boardRequestTime.Date.AddHours(12), Duration = 720 })
        );
        var (departureBoard1, departureBoard2) = (
            timetableRepository.GetDeparturesAsync(new() { EvaNumber = randomStation.EvaNumber, TimeStart = boardRequestTime.Date, Duration = 720 }),
            timetableRepository.GetDeparturesAsync(new() { EvaNumber = randomStation.EvaNumber, TimeStart = boardRequestTime.Date.AddHours(12), Duration = 720 })
        );
        await Task.WhenAll(arrivalBoard1, arrivalBoard2, departureBoard1, departureBoard2);
        var boards = (
            Arrivals: new[] { arrivalBoard1.Result.Arrivals, arrivalBoard2.Result.Arrivals },
            Departures: new[] { departureBoard1.Result.Departures, departureBoard2.Result.Departures }
        );

        var risIds = boards.Arrivals
            .SelectMany(board => board)
            .Select(arrival => new { arrival.JourneyID, arrival.Transport.Type, arrival.Transport.ReplacementTransport })
            .Concat(boards.Departures
                .SelectMany(board => board)
                .Select(departure => new { departure.JourneyID, departure.Transport.Type, departure.Transport.ReplacementTransport })
             )
            .DistinctBy(risId => risId.JourneyID)
            .ToList();

        var enabledTransports = randomStation.Transports
            .Where(transport => transport.Enabled)
            .Select(transport => transport.TransportType)
            .ToHashSet();

        DateTime discoveryDate = DateTime.UtcNow;
        var filteredByTransports = risIds
            .Where(risId =>
            {
                TransportType transportType = BoardsTransportToNavigatorTransport(risId.Type);
                return enabledTransports.Contains(transportType);
            })
            .Select(risId =>
            {
                // try to remove the 'yyyyMMdd-' part before the actual JourneyId
                var datePart = risId.JourneyID.Substring(0, 8);
                if (!DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out _)) return null;

                return new RisId()
                {
                    Id = risId.JourneyID.Substring(9),
                    TransportType = BoardsTransportToNavigatorTransport(risId.Type),
                    ReplacementTransportType = StringTransportToNavigatorTransport(risId.ReplacementTransport?.RealType),
                    DiscoveredAt = discoveryDate,
                    Active = true
                };
            })
            .Where(risId => risId != null)
            .Select(risId => risId!)
            .DistinctBy(risId => risId.Id)
            .ToList();

        randomStation.LastQueried = lastQueried;
        if (!filteredByTransports.Any())
        {
            logger.LogInformation("No RIS IDs found for {StationName} (EvaNumber: {StationEvaNumber})",
                randomStation.Name,
                randomStation.EvaNumber);
            await stationRepository.SaveStationsAsync([randomStation]);
            return;
        }

        var existingRisIds = await risIdRepository.GetRisIdsBatchAsync(filteredByTransports.Select(risId => risId.Id));
        var existingIdsSet = existingRisIds.Select(risId => risId.Id).ToHashSet();

        var newIds = filteredByTransports
            .Where(risId => !existingIdsSet.Contains(risId.Id))
            .ToList();
        if (newIds.Any()) await risIdRepository.SaveRisIdsBatchAsync(newIds);

        if (existingRisIds.Any())
        {
            foreach (var existingRisId in existingRisIds)
            {
                existingRisId.Active = true;
            }
            await risIdRepository.SaveRisIdsBatchAsync(existingRisIds);
        }

        await stationRepository.SaveStationsAsync([randomStation]);
        logger.LogInformation("Discovered {TotalRisIds} RIS IDs. Filtered to {Filtered} RIS IDs based on enabled transports. {NewRisIdCount} were new RIS IDs and updated {UpdatedRisIdCount} existing RIS IDs for {StationName} (EvaNumber: {StationEvaNumber})",
            risIds.Count,
            filteredByTransports.Count(),
            newIds.Count,
            existingRisIds.Count(),
            randomStation.Name,
            randomStation.EvaNumber);
    }

    private DateTime? EstimateLastQueried(Station station)
    {
        if (station.LastQueried == null) return DateTime.UtcNow.AddDays(-7);
        if (station.LastQueried.Value.Date < DateTime.UtcNow.Date) return new DateTime(
            DateOnly.FromDateTime(station.LastQueried.Value.AddDays(1)),
            TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay),
            DateTimeKind.Utc);
        return null;
    }

    private TransportType BoardsTransportToNavigatorTransport(RisBoards.TransportType transportType) => transportType switch
    {
        RisBoards.TransportType.HIGH_SPEED_TRAIN => TransportType.HighSpeedTrain,
        RisBoards.TransportType.INTERCITY_TRAIN => TransportType.IntercityTrain,
        RisBoards.TransportType.INTER_REGIONAL_TRAIN => TransportType.InterRegionalTrain,
        RisBoards.TransportType.REGIONAL_TRAIN => TransportType.RegionalTrain,
        RisBoards.TransportType.CITY_TRAIN => TransportType.CityTrain,
        RisBoards.TransportType.SUBWAY => TransportType.Subway,
        RisBoards.TransportType.TRAM => TransportType.Tram,
        RisBoards.TransportType.BUS => TransportType.Bus,
        RisBoards.TransportType.FERRY => TransportType.Ferry,
        RisBoards.TransportType.FLIGHT => TransportType.Flight,
        RisBoards.TransportType.CAR => TransportType.Car,
        RisBoards.TransportType.TAXI => TransportType.Taxi,
        RisBoards.TransportType.SHUTTLE => TransportType.Shuttle,
        RisBoards.TransportType.BIKE => TransportType.Bike,
        RisBoards.TransportType.SCOOTER => TransportType.Scooter,
        RisBoards.TransportType.WALK => TransportType.Walk,
        _ => TransportType.Unknown,
    };

    private TransportType? StringTransportToNavigatorTransport(string? transportType)
    {
        if (string.IsNullOrEmpty(transportType)) return null;
        return transportType switch
        {
            "HIGH_SPEED_TRAIN" => TransportType.HighSpeedTrain,
            "INTERCITY_TRAIN" => TransportType.IntercityTrain,
            "INTER_REGIONAL_TRAIN" => TransportType.InterRegionalTrain,
            "REGIONAL_TRAIN" => TransportType.RegionalTrain,
            "CITY_TRAIN" => TransportType.CityTrain,
            "SUBWAY" => TransportType.Subway,
            "TRAM" => TransportType.Tram,
            "BUS" => TransportType.Bus,
            "FERRY" => TransportType.Ferry,
            "FLIGHT" => TransportType.Flight,
            "CAR" => TransportType.Car,
            "TAXI" => TransportType.Taxi,
            "SHUTTLE" => TransportType.Shuttle,
            "BIKE" => TransportType.Bike,
            "SCOOTER" => TransportType.Scooter,
            "WALK" => TransportType.Walk,
            _ => TransportType.Unknown,
        };
    }
}
