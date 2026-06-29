using Navigator.Api.DTOs.Journey;
using Navigator.Api.DTOs.Journey.Message;
using Navigator.Api.Enums;
using Navigator.Data.Enums;
using Navigator.Data.Models.Ris;
using Riok.Mapperly.Abstractions;

namespace Navigator.Api.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class JourneyMapper
{
    #region Journey Administration
    [MapProperty(nameof(RisJourneys.Administration.AdministrationID), nameof(JourneyAdministration.AdministrationId))]
    [MapProperty(nameof(RisJourneys.Administration.OperatorCode), nameof(JourneyAdministration.OperatorCode))]
    [MapProperty(nameof(RisJourneys.Administration.OperatorName), nameof(JourneyAdministration.OperatorName))]
    public partial JourneyAdministration MapAdministration(RisJourneys.Administration source);
    #endregion

    #region Journey
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(RisJourneys.JourneyEventBased.JourneyID), nameof(Journey.JourneyId))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.HeaderAdministration), nameof(Journey.Administration), Use = nameof(MapAdministration))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.Type), nameof(Journey.Type), Use = nameof(MapJourneyType))]
    [MapValue(nameof(Journey.Transport), null)]
    [MapValue(nameof(Journey.ContinuationBy), null)]
    [MapValue(nameof(Journey.ContinuationFor), null)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.JourneyCancelled), nameof(Journey.Cancelled))]
    [MapValue(nameof(Journey.Destination), null)]
    [MapValue(nameof(Journey.DifferingDestination), null)]
    [MapValue(nameof(Journey.Origin), null)]
    [MapValue(nameof(Journey.DifferingOrigin), null)]
    [MapValue(nameof(Journey.ScheduledEvents), null)]
    [MapValue(nameof(Journey.Messages), null)]
    private partial Journey MapJourneyInternal(RisJourneys.JourneyEventBased source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    [UserMapping(Default = true)]
    public Journey MapJourney(RisJourneys.JourneyEventBased source)
    {
        var journey = MapJourneyInternal(source);

        journey.Transport = MapJourneyTransport(source);
        journey.ContinuationBy = (source.ContinuationBy ?? []).Select(continuation => continuation.JourneyID);
        journey.ContinuationFor = (source.ContinuationFor ?? []).Select(continuation => continuation.JourneyID);

        journey.Destination = MapJourneyRichStopPlace(source.Info.Destination);
        if (source.Info.DifferingDestination != null) journey.DifferingDestination = MapJourneyStopPlace(source.Info.DifferingDestination);

        journey.Origin = MapJourneyRichStopPlace(source.Info.Origin);
        if (source.Info.DifferingOrigin != null) journey.DifferingOrigin = MapJourneyStopPlace(source.Info.DifferingOrigin);

        journey.Messages = new List<JourneyMessage>();

        void MapMessages<T>(IEnumerable<T>? sourceList, Func<T, int> idSelector, Func<T, JourneyMessage> mapper)
        {
            if (sourceList is null) return;
            foreach (var item in sourceList)
            {
                var message = mapper(item);
                journey.Messages.Add(message);
            }
        }
        if (source.Messages is not null)
        {
            MapMessages(source.Messages.Attributes, message => message.MessageID, MapMessageAttribute);
            MapMessages(source.Messages.Disruptions, message => message.MessageID, MapMessageDisruption);
            MapMessages(source.Messages.Notes, message => message.MessageID, MapMessageNote);
            MapMessages(source.Messages.RisCauseCodes, message => message.MessageID, MapMessageRisCause);
            MapMessages(source.Messages.RisQualityDeviations, message => message.MessageID, MapMessageRisQualityDeviation);
        }

        journey.ScheduledEvents = source.Events.Select(MapJourneyScheduledEvent).ToList();
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

    #region Journey ScheduledEvent
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapValue(nameof(JourneyScheduledEvent.StopPlace), null)]
    [MapValue(nameof(JourneyScheduledEvent.DifferingStopPlace), null)]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Cancelled), nameof(JourneyScheduledEvent.Cancelled))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Additional), nameof(JourneyScheduledEvent.Additional))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.NoPassengerChange), nameof(JourneyScheduledEvent.NoPassengerChange))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.OnDemand), nameof(JourneyScheduledEvent.Demand))]
#pragma warning disable RMG072 // The source type of the referenced mapping does not match
    [MapProperty(nameof(RisJourneys.JourneyEvent.Type), nameof(JourneyScheduledEvent.ScheduleType), Use = nameof(MapScheduleType))]
#pragma warning restore RMG072 // The source type of the referenced mapping does not match
    [MapProperty(nameof(RisJourneys.JourneyEvent.TimeSchedule), nameof(JourneyScheduledEvent.PlannedTime))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Time), nameof(JourneyScheduledEvent.ActualTime))]
    [MapValue(nameof(JourneyScheduledEvent.Delay), null)]
    [MapProperty(nameof(RisJourneys.JourneyEvent.PlatformSchedule), nameof(JourneyScheduledEvent.PlannedPlatform))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Platform), nameof(JourneyScheduledEvent.ActualPlatform))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.TimeType), nameof(JourneyScheduledEvent.TimeType), Use = nameof(MapTimeType))]
    [MapValue(nameof(JourneyScheduledEvent.TravelsWith), null)]
    [MapValue(nameof(JourneyScheduledEvent.MessageIds), null)]
    private partial JourneyScheduledEvent MapJourneyScheduledEventInternal(RisJourneys.JourneyEvent source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    public JourneyScheduledEvent MapJourneyScheduledEvent(RisJourneys.JourneyEvent source)
    {
        var scheduledEvent = MapJourneyScheduledEventInternal(source);
        scheduledEvent.StopPlace = MapJourneyStopPlace(source.StopPlace);
        if (source.StopPlace.DifferingStopPlace != null) scheduledEvent.DifferingStopPlace = MapJourneyStopPlace(source.StopPlace.DifferingStopPlace);

        scheduledEvent.Delay = (int)(scheduledEvent.ActualTime - scheduledEvent.PlannedTime).TotalSeconds;

        scheduledEvent.TravelsWith = source.TravelsWith?.Select(travel => travel.JourneyID);
        scheduledEvent.MessageIds = source.Messages;
        return scheduledEvent;
    }

    private ScheduleType MapScheduleType(RisJourneys.EventType? eventType) => eventType switch
    {
        RisJourneys.EventType.ARRIVAL => ScheduleType.Arrival,
        RisJourneys.EventType.DEPARTURE => ScheduleType.Departure,
        _ => throw new MappingException($"Value '{eventType}' is not valid to be mapped for ScheduleType.")
    };

    private TimeType MapTimeType(string? source) => source?.ToUpperInvariant() switch
    {
        "SCHEDULE" => TimeType.Schedule,
        "PREVIEW" => TimeType.Preview,
        "REAL" => TimeType.Real,
        _ => throw new MappingException($"Value '{source}' is not valid to be mapped for TimeType.")
    };
    #endregion

    #region Journey Transport
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Type), nameof(JourneyTransport.Type), Use = nameof(MapTransportType))]
    [MapValue(nameof(JourneyTransport.ReplacementType), null)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Category), nameof(JourneyTransport.Category))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.CategoryInternal), nameof(JourneyTransport.CategoryInternal))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.JourneyDescription), nameof(JourneyTransport.JourneyDescription))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Label), nameof(JourneyTransport.Label))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Line), nameof(JourneyTransport.Line))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.JourneyNumber), nameof(JourneyTransport.Number))]
    private partial JourneyTransport MapJourneyTransportInternal(RisJourneys.JourneyEventBased source);

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
        if (possibleReplacements.Any()) transport.ReplacementType = possibleReplacements
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

    #region Journey StopPlaces
    [MapProperty(nameof(RisJourneys.StopPlaceEmbeddedWithCancel.Name), nameof(JourneyStopPlace.Name))]
    [MapProperty(nameof(RisJourneys.StopPlaceEmbeddedWithCancel.EvaNumber), nameof(JourneyStopPlace.EvaNumber), Use = nameof(ParseStationEvaNumber))]
    public partial JourneyStopPlace MapJourneyStopPlace(RisJourneys.StopPlaceEmbedded source);

    [MapProperty(nameof(RisJourneys.StopPlaceEmbeddedWithCancel.Name), nameof(JourneyRichStopPlace.Name))]
    [MapProperty(nameof(RisJourneys.StopPlaceEmbeddedWithCancel.EvaNumber), nameof(JourneyRichStopPlace.EvaNumber), Use = nameof(ParseStationEvaNumber))]
    [MapProperty(nameof(RisJourneys.StopPlaceEmbeddedWithCancel.Cancelled), nameof(JourneyRichStopPlace.Cancelled))]
    public partial JourneyRichStopPlace MapJourneyRichStopPlace(RisJourneys.StopPlaceEmbeddedWithCancel source);

    [MapProperty(nameof(RisJourneys.StopPlaceInJourney.Name), nameof(JourneyStopPlace.Name))]
    [MapProperty(nameof(RisJourneys.StopPlaceInJourney.EvaNumber), nameof(JourneyStopPlace.EvaNumber), Use = nameof(ParseStationEvaNumber))]
    public partial JourneyStopPlace MapJourneyStopPlace(RisJourneys.StopPlaceInJourney source);

    [MapProperty(nameof(RisJourneys.StopPlaceDifferingInJourney.Name), nameof(JourneyStopPlace.Name))]
    [MapProperty(nameof(RisJourneys.StopPlaceDifferingInJourney.EvaNumber), nameof(JourneyStopPlace.EvaNumber), Use = nameof(ParseStationEvaNumber))]
    public partial JourneyStopPlace MapJourneyStopPlace(RisJourneys.StopPlaceDifferingInJourney source);

    private int ParseStationEvaNumber(string? source)
    {
        if (!int.TryParse(source, out var evaNumber))
            throw new MappingException($"Value '{source}' is not valid to be mapped for Station EvaNumber.");
        return evaNumber;
    }
    #endregion

    #region Journey Message

    #region Attribute
    [MapProperty(nameof(RisJourneys.MessageAttribute.MessageID), nameof(AttributeMessage.MessageId))]
    [MapProperty(nameof(RisJourneys.MessageAttribute.Code), nameof(AttributeMessage.Key), Use = nameof(MapMessageKey))]
    [MapProperty(nameof(RisJourneys.MessageAttribute.Text), nameof(AttributeMessage.Text))]
    [MapperIgnoreTarget(nameof(AttributeMessage.Type))]
    public partial AttributeMessage MapMessageAttribute(RisJourneys.MessageAttribute source);
    #endregion

    #region Disruption
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(RisJourneys.MessageDisruptionCommunication.MessageID), nameof(DisruptionMessage.MessageId))]
    [MapValue(nameof(DisruptionMessage.Key), MessageKey.UNPLANNED_INFO)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.MessageDisruptionCommunication.LangDe.Text), nameof(DisruptionMessage.Text))]
    [MapperIgnoreTarget(nameof(DisruptionMessage.Type))]
    [MapProperty(nameof(RisJourneys.MessageDisruptionCommunication.Cause), nameof(DisruptionMessage.Cause))]
    [MapProperty(nameof(RisJourneys.MessageDisruptionCommunication.Effect), nameof(DisruptionMessage.Effect))]
    [MapProperty(nameof(RisJourneys.MessageDisruptionCommunication.DisruptionID), nameof(DisruptionMessage.DisruptionId))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.MessageDisruptionCommunication.LangDe.TextShort), nameof(DisruptionMessage.TextShort))]
    public partial DisruptionMessage MapMessageDisruption(RisJourneys.MessageDisruptionCommunication source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
    #endregion

    #region Note
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(RisJourneys.MessageNote.MessageID), nameof(NoteMessage.MessageId))]
    [MapValue(nameof(NoteMessage.Key), MessageKey.UNPLANNED_INFO)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.MessageNote.LangDe.Text), nameof(NoteMessage.Text))]
    [MapperIgnoreTarget(nameof(AttributeMessage.Type))]
    [MapProperty(nameof(RisJourneys.MessageNote.Category), nameof(NoteMessage.Category))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.MessageNote.LangDe.TextShort), nameof(NoteMessage.TextShort))]
    public partial NoteMessage MapMessageNote(RisJourneys.MessageNote source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
    #endregion

    #region RisCause
    [MapProperty(nameof(RisJourneys.MessageRisCauseCode.MessageID), nameof(RisCauseMessage.MessageId))]
    [MapProperty(nameof(RisJourneys.MessageRisCauseCode.Code), nameof(RisCauseMessage.Key), Use = nameof(MapMessageKey))]
    [MapProperty(nameof(RisJourneys.MessageRisCauseCode.Text), nameof(RisCauseMessage.Text))]
    [MapperIgnoreTarget(nameof(RisCauseMessage.Type))]
    public partial RisCauseMessage MapMessageRisCause(RisJourneys.MessageRisCauseCode source);
    #endregion

    #region RisQualityDeviation
    [MapProperty(nameof(RisJourneys.MessageRisQualityDeviation.MessageID), nameof(RisQualityDeviationMessage.MessageId))]
    [MapProperty(nameof(RisJourneys.MessageRisQualityDeviation.Code), nameof(RisQualityDeviationMessage.Key), Use = nameof(MapMessageKey))]
    [MapProperty(nameof(RisJourneys.MessageRisQualityDeviation.Text), nameof(RisQualityDeviationMessage.Text))]
    [MapperIgnoreTarget(nameof(RisQualityDeviationMessage.Type))]
    public partial RisQualityDeviationMessage MapMessageRisQualityDeviation(RisJourneys.MessageRisQualityDeviation source);
    #endregion

    private MessageKey MapMessageKey(string? messageKey) => messageKey?.ToUpperInvariant() switch
    {
        "1" or "3" or "5" or "6" or "7" or "10" or "12" or "13" or "14" or "15" or "16" or "17" or "18" or "19" or "21" or "24" or "27" or "28" or "32" or "42" or "43" or "44" or "45" or "47" or "48" or "51" or "56" or "62" or "63" or "67" or "68" or "69" or "72" or "84" or "88" or "89" or "94" or "99" or "1000" or "CK" or "EF" or "EH" or "FT" or "HS" or "KA" or "OA" or "OC" or "RG" or "RO" or "RT" or "SI" or "SM" or "ZN" => MessageKey.UNPLANNED_INFO,
        "2" or "8" or "9" or "11" or "22" or "30" or "31" or "33" or "34" or "35" or "36" or "37" or "38" or "39" or "40" or "41" or "49" or "50" or "52" or "53" or "54" or "55" or "58" or "59" or "60" or "61" or "64" or "65" or "66" or "96" or "97" or "98" => MessageKey.GENERAL_WARNING,
        "25" => MessageKey.ADDITIONAL_COACHES,
        "26" or "79" or "82" or "85" => MessageKey.MISSING_COACHES,
        "73" or "74" or "75" or "76" or "80" => MessageKey.CHANGED_COACH_SEQUENCE,
        "29" or "78" => MessageKey.REPLACEMENT_SERVICE,
        "57" => MessageKey.ADDITIONAL_STOP,
        "70" or "71" => MessageKey.WIFI_DISTRIBUTION,
        "77" => MessageKey.NO_FIRST_CLASS,
        "83" or "93" or "95" or "DC" or "OG" => MessageKey.ACCESSIBILITY_WARNING,
        "86" or "87" => MessageKey.RESERVATIONS_MISSING,
        "RP" => MessageKey.RESERVATIONS_REQUIRED,
        "90" => MessageKey.NO_FOOD,
        "AB" or "KF" or "RF" or "TF" => MessageKey.BICYCLE_TRANSPORT,
        "91" or "NF" => MessageKey.BICYCLE_TRANSPORT_NOT_POSSIBLE,
        "92" or "FB" or "FK" or "FS" or "G" => MessageKey.BICYCLE_WARNING,
        "FF" or "FO" or "FR" => MessageKey.BICYCLE_RESERVATION_REQUIRED,
        "N+" or "NG" or "NJ" => MessageKey.TICKET_INFORMATION,
        _ => MessageKey.UNPLANNED_INFO
    };
    #endregion
}
