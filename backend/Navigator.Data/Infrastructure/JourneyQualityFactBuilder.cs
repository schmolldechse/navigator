using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums;

namespace Navigator.Data.Infrastructure;

public static class JourneyQualityFactBuilder
{
    public static (List<JourneyEventQualityFact> EventFacts, List<JourneyRouteQualityFact> RouteFacts) Build(
        IEnumerable<Journey> journeys
    )
    {
        var eventFacts = new List<JourneyEventQualityFact>();
        var routeFacts = new List<JourneyRouteQualityFact>();

        foreach (var journey in journeys)
        {
            var orderedStopPlaces = journey.StopPlaces
                .OrderBy(stopPlace => stopPlace.PlannedTime)
                .ThenBy(stopPlace => stopPlace.StationEvaNumber)
                .ToList();
            if (!orderedStopPlaces.Any()) continue;

            var transport = journey.Transport;
            var isReplacementTransport = journey.JourneyType == JourneyType.Replacement
                || transport.ReplacementTransportType is not null;
            var originEvaNumber = ResolveOriginEvaNumber(orderedStopPlaces);
            var destinationEvaNumber = ResolveDestinationEvaNumber(orderedStopPlaces);
            var journeyStartTime = ResolveJourneyStartTime(orderedStopPlaces);
            var journeyEndTime = ResolveJourneyEndTime(orderedStopPlaces);
            var terminalDelaySeconds = orderedStopPlaces
                .Where(stopPlace => stopPlace.Cancelled is not true)
                .OrderBy(stopPlace => stopPlace.ScheduleType == ScheduleType.Arrival ? 0 : 1)
                .ThenByDescending(stopPlace => stopPlace.PlannedTime)
                .Select(stopPlace => (int?)stopPlace.Delay)
                .FirstOrDefault();

            routeFacts.Add(new JourneyRouteQualityFact
            {
                JourneyId = journey.Id,
                Date = journey.Date,
                JourneyStartTime = journeyStartTime,
                JourneyEndTime = journeyEndTime,
                AdministrationId = journey.AdministrationId,
                TransportType = transport.TransportType,
                JourneyDescription = transport.JourneyDescription,
                Number = transport.Number,
                IsReplacementTransport = isReplacementTransport,
                OriginEvaNumber = originEvaNumber,
                DestinationEvaNumber = destinationEvaNumber,
                JourneyCancelled = journey.Cancelled,
                TerminalDelaySeconds = terminalDelaySeconds
            });

            foreach (var stopPlace in journey.StopPlaces)
            {
                eventFacts.Add(new JourneyEventQualityFact
                {
                    StopPlaceId = stopPlace.Id,
                    JourneyId = journey.Id,
                    Date = journey.Date,
                    PlannedTime = stopPlace.PlannedTime,
                    JourneyStartTime = journeyStartTime,
                    JourneyEndTime = journeyEndTime,
                    StationEvaNumber = stopPlace.StationEvaNumber,
                    ScheduleType = stopPlace.ScheduleType,
                    AdministrationId = journey.AdministrationId,
                    TransportType = transport.TransportType,
                    JourneyDescription = transport.JourneyDescription,
                    Number = transport.Number,
                    IsReplacementTransport = isReplacementTransport,
                    OriginEvaNumber = originEvaNumber,
                    DestinationEvaNumber = destinationEvaNumber,
                    Cancelled = stopPlace.Cancelled,
                    Delay = stopPlace.Delay
                });
            }
        }

        return (eventFacts, routeFacts);
    }

    private static int ResolveOriginEvaNumber(IEnumerable<JourneyStopPlace> stopPlaces)
    {
        var ordered = stopPlaces.ToList();
        return ordered
            .Where(stopPlace => stopPlace.ScheduleType == ScheduleType.Departure)
            .OrderBy(stopPlace => stopPlace.PlannedTime)
            .ThenBy(stopPlace => stopPlace.StationEvaNumber)
            .Select(stopPlace => (int?)stopPlace.StationEvaNumber)
            .FirstOrDefault()
            ?? ordered
                .OrderBy(stopPlace => stopPlace.PlannedTime)
                .ThenBy(stopPlace => stopPlace.StationEvaNumber)
                .Select(stopPlace => stopPlace.StationEvaNumber)
                .FirstOrDefault();
    }

    private static int ResolveDestinationEvaNumber(IEnumerable<JourneyStopPlace> stopPlaces)
    {
        var ordered = stopPlaces.ToList();
        return ordered
            .Where(stopPlace => stopPlace.ScheduleType == ScheduleType.Arrival)
            .OrderByDescending(stopPlace => stopPlace.PlannedTime)
            .ThenByDescending(stopPlace => stopPlace.StationEvaNumber)
            .Select(stopPlace => (int?)stopPlace.StationEvaNumber)
            .FirstOrDefault()
            ?? ordered
                .OrderByDescending(stopPlace => stopPlace.PlannedTime)
                .ThenByDescending(stopPlace => stopPlace.StationEvaNumber)
                .Select(stopPlace => stopPlace.StationEvaNumber)
                .FirstOrDefault();
    }

    private static DateTime ResolveJourneyStartTime(IEnumerable<JourneyStopPlace> stopPlaces)
    {
        var ordered = stopPlaces.ToList();
        return ordered
            .Where(stopPlace => stopPlace.ScheduleType == ScheduleType.Departure)
            .OrderBy(stopPlace => stopPlace.PlannedTime)
            .Select(stopPlace => (DateTime?)stopPlace.PlannedTime)
            .FirstOrDefault()
            ?? ordered
                .OrderBy(stopPlace => stopPlace.PlannedTime)
                .Select(stopPlace => stopPlace.PlannedTime)
                .First();
    }

    private static DateTime ResolveJourneyEndTime(IEnumerable<JourneyStopPlace> stopPlaces)
    {
        var ordered = stopPlaces.ToList();
        return ordered
            .Where(stopPlace => stopPlace.ScheduleType == ScheduleType.Arrival)
            .OrderByDescending(stopPlace => stopPlace.PlannedTime)
            .Select(stopPlace => (DateTime?)stopPlace.PlannedTime)
            .FirstOrDefault()
            ?? ordered
                .OrderByDescending(stopPlace => stopPlace.PlannedTime)
                .Select(stopPlace => stopPlace.PlannedTime)
                .First();
    }
}
