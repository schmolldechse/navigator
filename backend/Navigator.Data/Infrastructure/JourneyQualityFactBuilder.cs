using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums;

namespace Navigator.Data.Infrastructure;

public static class JourneyQualityFactBuilder
{
    public static (List<JourneyEventQualityFact> EventFacts, List<JourneyQualityFact> JourneyFacts) Build(
        IEnumerable<Journey> journeys
    )
    {
        var eventFacts = new List<JourneyEventQualityFact>();
        var journeyFacts = new List<JourneyQualityFact>();

        foreach (var journey in journeys)
        {
            var orderedStopPlaces = journey.StopPlaces
                .OrderBy(stopPlace => stopPlace.PlannedTime)
                .ThenBy(stopPlace => stopPlace.StationEvaNumber)
                .ToList();
            if (!orderedStopPlaces.Any()) continue;

            var transport = journey.Transport;
            var isReplacement = journey.JourneyType == JourneyType.Replacement
                || transport.ReplacementTransportType is not null;
            var journeyDescription = ResolveJourneyDescription(transport);
            var originEvaNumber = ResolveOriginEvaNumber(orderedStopPlaces);
            var destinationEvaNumber = ResolveDestinationEvaNumber(orderedStopPlaces);
            var journeyStartTime = ResolveJourneyStartTime(orderedStopPlaces);
            var journeyEndTime = ResolveJourneyEndTime(orderedStopPlaces);
            var destinationStopPlace = ResolveDestinationStopPlace(orderedStopPlaces);
            var fullyCancelled = journey.Cancelled || orderedStopPlaces.All(stopPlace => stopPlace.Cancelled);
            var partiallyCancelled = !fullyCancelled && orderedStopPlaces.Any(stopPlace => stopPlace.Cancelled);
            var destinationNotReached = fullyCancelled || destinationStopPlace.Cancelled;
            var destinationDelaySeconds = destinationNotReached
                ? null
                : (int?)destinationStopPlace.Delay;

            journeyFacts.Add(new JourneyQualityFact
            {
                BucketHour = ToBucketHour(journeyStartTime),
                JourneyId = journey.Id,
                JourneyDate = journey.Date,
                JourneyStartTime = journeyStartTime,
                JourneyEndTime = journeyEndTime,
                AdministrationId = journey.AdministrationId,
                TransportType = transport.TransportType,
                JourneyDescription = journeyDescription,
                JourneyNumber = transport.Number,
                IsReplacement = isReplacement,
                OriginEvaNumber = originEvaNumber,
                DestinationEvaNumber = destinationEvaNumber,
                DestinationDelaySeconds = destinationDelaySeconds,
                FullyCancelled = fullyCancelled,
                PartiallyCancelled = partiallyCancelled,
                DestinationNotReached = destinationNotReached
            });

            foreach (var stopPlace in journey.StopPlaces)
            {
                eventFacts.Add(new JourneyEventQualityFact
                {
                    BucketHour = ToBucketHour(stopPlace.PlannedTime),
                    StopPlaceId = stopPlace.Id,
                    JourneyId = journey.Id,
                    JourneyDate = journey.Date,
                    PlannedTime = stopPlace.PlannedTime,
                    JourneyStartTime = journeyStartTime,
                    JourneyEndTime = journeyEndTime,
                    StationEvaNumber = stopPlace.StationEvaNumber,
                    ScheduleType = stopPlace.ScheduleType,
                    AdministrationId = journey.AdministrationId,
                    TransportType = transport.TransportType,
                    JourneyDescription = journeyDescription,
                    JourneyNumber = transport.Number,
                    IsReplacement = isReplacement,
                    OriginEvaNumber = originEvaNumber,
                    DestinationEvaNumber = destinationEvaNumber,
                    StopCancelled = stopPlace.Cancelled,
                    EventDelaySeconds = stopPlace.Delay
                });
            }
        }

        return (eventFacts, journeyFacts);
    }

    private static DateTime ToBucketHour(DateTime dateTime) =>
        new(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            0,
            0,
            dateTime.Kind);

    private static string ResolveJourneyDescription(JourneyTransport transport)
    {
        if (!string.IsNullOrWhiteSpace(transport.JourneyDescription))
            return transport.JourneyDescription.Trim();

        var fallback = string.Join(
            " ",
            new[] { transport.Category, transport.Line }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!.Trim()));

        return string.IsNullOrWhiteSpace(fallback)
            ? transport.Number.ToString()
            : fallback;
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

    private static JourneyStopPlace ResolveDestinationStopPlace(IEnumerable<JourneyStopPlace> stopPlaces)
    {
        var ordered = stopPlaces.ToList();
        return ordered
            .Where(stopPlace => stopPlace.ScheduleType == ScheduleType.Arrival)
            .OrderByDescending(stopPlace => stopPlace.PlannedTime)
            .ThenByDescending(stopPlace => stopPlace.StationEvaNumber)
            .FirstOrDefault()
            ?? ordered
                .OrderByDescending(stopPlace => stopPlace.PlannedTime)
                .ThenByDescending(stopPlace => stopPlace.StationEvaNumber)
                .First();
    }
}
