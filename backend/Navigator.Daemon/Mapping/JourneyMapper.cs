using Navigator.Data.Entities.Journey;
using Navigator.Data.Enums;
using Navigator.Data.Models.Ris;
using Riok.Mapperly.Abstractions;
using System.Globalization;

namespace Navigator.Daemon.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class JourneyMapper
{
    #region Journey Administration
    [MapperIgnoreTarget(nameof(Administration.Id))]
    [MapProperty(nameof(RisJourneys.Administration.AdministrationID), nameof(Administration.AdministrationId))]
    [MapProperty(nameof(RisJourneys.Administration.OperatorCode), nameof(Administration.OperatorCode))]
    [MapProperty(nameof(RisJourneys.Administration.OperatorName), nameof(Administration.OperatorName))]
    public partial Administration MapAdministration(RisJourneys.Administration source);
    #endregion

    #region Journey
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(RisJourneys.JourneyEventBased.JourneyID), nameof(Journey.Id))]
    [MapValue(nameof(Journey.Date), null)]
    [MapValue(nameof(Journey.InsertedAt), null)]
    [MapValue(nameof(Journey.AdministrationId), null)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.HeaderAdministration), nameof(Journey.Administration), Use = nameof(MapAdministration))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.JourneyCancelled), nameof(Journey.Cancelled))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.Type), nameof(Journey.JourneyType), Use = nameof(MapJourneyType))]
    [MapValue(nameof(Journey.Transport), null)]
    [MapperIgnoreTarget(nameof(Journey.StopPlaces))]
    private partial Journey MapJourneyInternal(RisJourneys.JourneyEventBased source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    [UserMapping(Default = true)]
    public Journey MapJourney(RisJourneys.JourneyEventBased source)
    {
        var journey = MapJourneyInternal(source);

        if (!DateOnly.TryParseExact(journey.Id[..8], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            throw new MappingException($"Value '{journey.Id}' is not valid to be mapped for Date.");
        journey.Date = date;

        journey.Transport = MapJourneyTransport(source);
        journey.Transport.Date = date;

        journey.StopPlaces = new List<JourneyStopPlace>();
        if (source.Events is not null)
        {
            foreach (var sourceEvent in source.Events)
            {
                var stopPlace = MapJourneyStopPlace(sourceEvent);
                stopPlace.JourneyId = journey.Id;
                stopPlace.Date = date;

                journey.StopPlaces.Add(stopPlace);
            }
        }

        return journey;
    }

    private JourneyType MapJourneyType(string? sourceType) => sourceType?.ToUpperInvariant() switch
    {
        "REGULAR" => JourneyType.Regular,
        "REPLACEMENT" => JourneyType.Replacement,
        "RELIEF" => JourneyType.Relief,
        "EXTRA" => JourneyType.Extra,
        _ => throw new MappingException($"Value '{sourceType}' is not valid to be mapped for JourneyType.")
    };
    #endregion

    #region JourneyTransport
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(RisJourneys.JourneyEventBased.JourneyID), nameof(JourneyTransport.JourneyId))]
    [MapperIgnoreTarget(nameof(JourneyTransport.Journey))]
    [MapValue(nameof(JourneyTransport.Date), null)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Type), nameof(JourneyTransport.TransportType), Use = nameof(MapTransportType))]
    [MapperIgnoreTarget(nameof(JourneyTransport.ReplacementTransportType))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Category), nameof(JourneyTransport.Category))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.CategoryInternal), nameof(JourneyTransport.CategoryInternal))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.JourneyDescription), nameof(JourneyTransport.JourneyDescription))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Label), nameof(JourneyTransport.Label))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Line), nameof(JourneyTransport.Line))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.JourneyNumber), nameof(JourneyTransport.Number))]
    private partial JourneyTransport MapJourneyTransportInternal(RisJourneys.JourneyEventBased source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    [UserMapping(Default = true)]
    public JourneyTransport MapJourneyTransport(RisJourneys.JourneyEventBased source)
    {
        if (source.Info.TransportAtStart is null)
            throw new MappingException($"Mapping 'JourneyTransport' (for Journey '{source.JourneyID}') failed. Missing 'Info.TransportAtStart' property!");

        var transport = MapJourneyTransportInternal(source);

        var possibleReplacements = source.Events
            .Where(journeyEvent => !string.IsNullOrEmpty(journeyEvent.Transport?.ReplacementTransport?.RealType))
            .Select(journeyEvent => MapTransportType(journeyEvent.Transport.ReplacementTransport.RealType))
            .Where(transportType => transportType != TransportType.Unknown)
            .ToList();
        if (possibleReplacements.Any()) transport.ReplacementTransportType = possibleReplacements
                .GroupBy(transportType => transportType)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .First();
        return transport;
    }

    private TransportType MapTransportType(string? sourceType) => sourceType?.ToUpperInvariant() switch
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
        _ => TransportType.Unknown
    };
    #endregion

    #region JourneyStopPlace
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapperIgnoreTarget(nameof(JourneyStopPlace.Id))]
    [MapValue(nameof(JourneyStopPlace.JourneyId), null)]
    [MapperIgnoreTarget(nameof(JourneyStopPlace.Journey))]
    [MapValue(nameof(JourneyStopPlace.Date), null)]
#pragma warning disable RMG072 // The source type of the referenced mapping does not match
    [MapProperty(nameof(RisJourneys.JourneyEvent.Type), nameof(JourneyStopPlace.ScheduleType), Use = nameof(MapScheduleType))]
#pragma warning restore RMG072 // The source type of the referenced mapping does not match
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEvent.StopPlace.EvaNumber), nameof(JourneyStopPlace.StationEvaNumber), Use = nameof(ParseStationEvaNumber))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Cancelled), nameof(JourneyStopPlace.Cancelled))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Additional), nameof(JourneyStopPlace.Additional))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.OnDemand), nameof(JourneyStopPlace.Demand))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.NoPassengerChange), nameof(JourneyStopPlace.NoPassengerChange))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEvent.TimeSchedule.UtcDateTime), nameof(JourneyStopPlace.PlannedTime))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEvent.Time.UtcDateTime), nameof(JourneyStopPlace.ActualTime))]
    [MapperIgnoreTarget(nameof(JourneyStopPlace.Delay))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.TimeType), nameof(JourneyStopPlace.TimeType), Use = nameof(MapTimeType))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.PlatformSchedule), nameof(JourneyStopPlace.PlannedPlatform))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Platform), nameof(JourneyStopPlace.ActualPlatform))]
    public partial JourneyStopPlace MapJourneyStopPlace(RisJourneys.JourneyEvent source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    private ScheduleType MapScheduleType(RisJourneys.EventType? eventType) => eventType switch
    {
        RisJourneys.EventType.ARRIVAL => ScheduleType.Arrival,
        RisJourneys.EventType.DEPARTURE => ScheduleType.Departure,
        _ => throw new MappingException($"Value '{eventType}' is not valid to be mapped for ScheduleType.")
    };

    private int ParseStationEvaNumber(string? source)
    {
        if (!int.TryParse(source, out var evaNumber))
            throw new MappingException($"Value '{source}' is not valid to be mapped for Station EvaNumber.");
        return evaNumber;
    }

    private TimeType MapTimeType(string? source) => source?.ToUpperInvariant() switch
    {
        "SCHEDULE" => TimeType.Schedule,
        "PREVIEW" => TimeType.Preview,
        "REAL" => TimeType.Real,
        _ => throw new MappingException($"Value '{source}' is not valid to be mapped for TimeType.")
    };
    #endregion

}
