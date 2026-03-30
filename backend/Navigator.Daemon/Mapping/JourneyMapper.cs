using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.Journey.Message;
using Navigator.Data.Enums;
using Navigator.Data.Models.Ris;
using Riok.Mapperly.Abstractions;
using System.Globalization;

namespace Navigator.Daemon.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class JourneyMapper
{
    #region Journey
#pragma warning disable RMG020 // Source member is not mapped to any target member
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(RisJourneys.JourneyEventBased.JourneyID), nameof(Journey.Id))]
    [MapValue(nameof(Journey.Date), null)]
    [MapValue(nameof(Journey.InsertedAt), null)]
    [MapValue(nameof(Journey.AdministrationId), null)]
    [MapValue(nameof(Journey.Administration), null)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.JourneyCancelled), nameof(Journey.Cancelled))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.Type), nameof(Journey.JourneyType), Use = nameof(MapJourneyType))]
    [MapValue(nameof(Journey.Transport), null)]
    [MapValue(nameof(Journey.StopPlaces), null)]
    [MapValue(nameof(Journey.Messages), null)]
    private partial Journey MapJourneyInternal(RisJourneys.JourneyEventBased source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
#pragma warning restore RMG020 // Source member is not mapped to any target member

    [UserMapping(Default = true)]
    public Journey MapJourney(RisJourneys.JourneyEventBased source)
    {
        var journey = MapJourneyInternal(source);

        if (!DateOnly.TryParseExact(journey.Id[..8], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            throw new MappingException($"Value '{journey.Id}' is not valid to be mapped for Date.");
        journey.Date = date;

        journey.Transport = MapJourneyTransport(source);

        var messageLookup = new Dictionary<int, JourneyMessage>();
        journey.Messages = new List<JourneyMessage>();

        void MapMessages<T>(IEnumerable<T>? sourceList, Func<T, int> idSelector, Func<T, JourneyMessage> mapper)
        {
            if (sourceList is null) return;
            foreach (var item in sourceList)
            { 
                var message = mapper(item);
                message.JourneyId = journey.Id;

                journey.Messages.Add(message);

                var messageId = idSelector(item);
                if (!messageLookup.ContainsKey(messageId))
                    messageLookup[messageId] = message;
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

        journey.StopPlaces = new List<JourneyStopPlace>();
        if (source.Events is not null)
        {
            foreach (var sourceEvent in source.Events)
            {
                var stopPlace = MapJourneyStopPlace(sourceEvent);
                stopPlace.JourneyId = journey.Id;

                if (sourceEvent.Messages is null || !sourceEvent.Messages.Any())
                {
                    journey.StopPlaces.Add(stopPlace);
                    continue;
                }

                stopPlace.Messages = [];
                foreach (var messageId in sourceEvent.Messages)
                {
                    if (!messageLookup.TryGetValue(messageId, out var linkedMessage)) continue;
                    stopPlace.Messages.Add(new JourneyStopPlaceMessage()
                    {
                        StopPlace = stopPlace,
                        Message = linkedMessage
                    });
                }

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

    #region JourneyAdministration
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapValue(nameof(Administration.Id), null)]
    [MapProperty(nameof(RisJourneys.Administration.AdministrationID), nameof(Administration.AdministrationId))]
    [MapProperty(nameof(RisJourneys.Administration.OperatorCode), nameof(Administration.OperatorCode))]
    [MapProperty(nameof(RisJourneys.Administration.OperatorName), nameof(Administration.OperatorName))]
    public partial Administration MapAdministration(RisJourneys.Administration source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
    #endregion

    #region JourneyTransport
#pragma warning disable RMG020 // Source member is not mapped to any target member
    [MapProperty(nameof(RisJourneys.JourneyEventBased.JourneyID), nameof(JourneyTransport.Id))]
    [MapValue(nameof(JourneyTransport.Journey), null)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Type), nameof(JourneyTransport.TransportType), Use = nameof(MapTransportType))]
    [MapValue(nameof(JourneyTransport.ReplacementTransportType), null)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Category), nameof(JourneyTransport.Category))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.CategoryInternal), nameof(JourneyTransport.CategoryInternal))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.JourneyDescription), nameof(JourneyTransport.JourneyDescription))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Label), nameof(JourneyTransport.Label))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.Line), nameof(JourneyTransport.Line))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEventBased.Info.TransportAtStart.JourneyNumber), nameof(JourneyTransport.Number))]
    private partial JourneyTransport MapJourneyTransportInternal(RisJourneys.JourneyEventBased source);
#pragma warning restore RMG020 // Source member is not mapped to any target member

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
    [MapValue(nameof(JourneyStopPlace.Id), null)]
    [MapValue(nameof(JourneyStopPlace.JourneyId), null)]
    [MapValue(nameof(JourneyStopPlace.Journey), null)]
#pragma warning disable RMG072 // The source type of the referenced mapping does not match
    [MapProperty(nameof(RisJourneys.JourneyEvent.Type), nameof(JourneyStopPlace.ScheduleType), Use = nameof(MapScheduleType))]
#pragma warning restore RMG072 // The source type of the referenced mapping does not match
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.JourneyEvent.StopPlace.EvaNumber), nameof(JourneyStopPlace.StationEvaNumber), Use = nameof(ParseStationEvaNumber))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Cancelled), nameof(JourneyStopPlace.Cancelled))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Additional), nameof(JourneyStopPlace.Additional))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.OnDemand), nameof(JourneyStopPlace.Demand))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.NoPassengerChange), nameof(JourneyStopPlace.NoPassengerChange))]
    [MapValue(nameof(JourneyStopPlace.PlannedTime), null)]
    [MapValue(nameof(JourneyStopPlace.ActualTime), null)]
    [MapValue(nameof(JourneyStopPlace.Delay), null)]
    [MapProperty(nameof(RisJourneys.JourneyEvent.TimeType), nameof(JourneyStopPlace.TimeType), Use = nameof(MapTimeType))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.PlatformSchedule), nameof(JourneyStopPlace.PlannedPlatform))]
    [MapProperty(nameof(RisJourneys.JourneyEvent.Platform), nameof(JourneyStopPlace.ActualPlatform))]
    [MapValue(nameof(JourneyStopPlace.Messages), null)]
    private partial JourneyStopPlace MapJourneyStopPlaceInternal(RisJourneys.JourneyEvent source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    public JourneyStopPlace MapJourneyStopPlace(RisJourneys.JourneyEvent source)
    {
        var stopPlace = MapJourneyStopPlaceInternal(source);
        stopPlace.PlannedTime = source.TimeSchedule.UtcDateTime;
        stopPlace.ActualTime = source.Time.UtcDateTime;
        return stopPlace;
    }

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

    #region JourneyMessage

    #region Attribute
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapValue(nameof(JourneyMessage.Id), null)]
    [MapValue(nameof(JourneyMessage.JourneyId), null)]
    [MapValue(nameof(JourneyMessage.Journey), null)]
    [MapValue(nameof(JourneyMessage.Type), MessageType.Attribute)]
    [MapProperty(nameof(RisJourneys.MessageAttribute.Code), nameof(JourneyMessage.Code))]
    [MapProperty(nameof(RisJourneys.MessageAttribute.Text), nameof(JourneyMessage.Text))]
    [MapValue(nameof(JourneyMessage.TextShort), null)]
    [MapValue(nameof(JourneyMessage.DisruptionCause), null)]
    [MapValue(nameof(JourneyMessage.DisruptionEffect), null)]
    [MapValue(nameof(JourneyMessage.NoteCategory), null)]
    [MapValue(nameof(JourneyMessage.References), null)]
    [MapValue(nameof(JourneyMessage.JourneyStopPlaceMessages), null)]
    public partial JourneyMessage MapMessageAttribute(RisJourneys.MessageAttribute source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
    #endregion

    #region Disruption
#pragma warning disable RMG012 // Source member was not found for target member
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapValue(nameof(JourneyMessage.Id), null)]
    [MapValue(nameof(JourneyMessage.JourneyId), null)]
    [MapValue(nameof(JourneyMessage.Journey), null)]
    [MapValue(nameof(JourneyMessage.Type), MessageType.Disruption)]
    [MapValue(nameof(JourneyMessage.Code), null)]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.MessageDisruptionCommunication.LangDe.Text), nameof(JourneyMessage.Text))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.MessageDisruptionCommunication.LangDe.TextShort), nameof(JourneyMessage.TextShort))]
    [MapProperty(nameof(RisJourneys.MessageDisruptionCommunication.Cause), nameof(JourneyMessage.DisruptionCause))]
    [MapProperty(nameof(RisJourneys.MessageDisruptionCommunication.Effect), nameof(JourneyMessage.DisruptionEffect))]
    [MapValue(nameof(JourneyMessage.NoteCategory), null)]
    [MapValue(nameof(JourneyMessage.References), null)]
    [MapValue(nameof(JourneyMessage.JourneyStopPlaceMessages), null)]
    private partial JourneyMessage MapMessageDisruptionInternal(RisJourneys.MessageDisruptionCommunication source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
#pragma warning restore RMG012 // Source member was not found for target member

    [UserMapping(Default = true)]
    public JourneyMessage MapMessageDisruption(RisJourneys.MessageDisruptionCommunication source)
    {
        var message = MapMessageDisruptionInternal(source);
        var references = new List<JourneyMessageReference>();

        references.AddRange((source.LangDe?.Links ?? []).Select(link => new JourneyMessageReference
        {
            ReferenceType = MessageReferenceType.Link,
            Url = link.Url,
            Label = link.Label,
            Message = null!
        }));

        references.AddRange((source.LangDe?.Images ?? []).Select(image => new JourneyMessageReference
        {
            ReferenceType = MessageReferenceType.Image,
            Url = image.Url,
            Label = image.Label,
            Message = null!
        }));

        references.AddRange((source.LangDe?.Attachments ?? []).Select(attachment => new JourneyMessageReference
        {
            ReferenceType = MessageReferenceType.Attachment,
            Url = attachment.Url,
            Label = attachment.Label,
            Message = null!
        }));

        message.References = references;
        return message;
    }
    #endregion

    #region Note
#pragma warning disable RMG012 // Source member was not found for target member
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapValue(nameof(JourneyMessage.Id), null)]
    [MapValue(nameof(JourneyMessage.JourneyId), null)]
    [MapValue(nameof(JourneyMessage.Journey), null)]
    [MapValue(nameof(JourneyMessage.Type), MessageType.Note)]
    [MapProperty(nameof(RisJourneys.MessageNote.Code), nameof(JourneyMessage.Code))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.MessageNote.LangDe.Text), nameof(JourneyMessage.Text))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisJourneys.MessageNote.LangDe.TextShort), nameof(JourneyMessage.TextShort))]
    [MapValue(nameof(JourneyMessage.DisruptionCause), null)]
    [MapValue(nameof(JourneyMessage.DisruptionEffect), null)]
    [MapProperty(nameof(RisJourneys.MessageNote.Category), nameof(JourneyMessage.NoteCategory))]
    [MapValue(nameof(JourneyMessage.References), null)]
    [MapValue(nameof(JourneyMessage.JourneyStopPlaceMessages), null)]
    private partial JourneyMessage MapMessageNoteInternal(RisJourneys.MessageNote source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
#pragma warning restore RMG012 // Source member was not found for target member

    [UserMapping(Default = true)]
    public JourneyMessage MapMessageNote(RisJourneys.MessageNote source)
    {
        var message = MapMessageNoteInternal(source);
        var references = new List<JourneyMessageReference>();

        references.AddRange((source.LangDe?.Links ?? []).Select(link => new JourneyMessageReference
        {
            ReferenceType = MessageReferenceType.Link,
            Url = link.Url,
            Label = link.Label,
            Message = null!
        }));

        references.AddRange((source.LangDe?.Images ?? []).Select(image => new JourneyMessageReference
        {
            ReferenceType = MessageReferenceType.Image,
            Url = image.Url,
            Label = image.Label,
            Message = null!
        }));

        references.AddRange((source.LangDe?.Attachments ?? []).Select(attachment => new JourneyMessageReference
        {
            ReferenceType = MessageReferenceType.Attachment,
            Url = attachment.Url,
            Label = attachment.Label,
            Message = null!
        }));

        message.References = references;
        return message;
    }
    #endregion

    #region RisCause
#pragma warning disable RMG012 // Source member was not found for target member
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapValue(nameof(JourneyMessage.Id), null)]
    [MapValue(nameof(JourneyMessage.JourneyId), null)]
    [MapValue(nameof(JourneyMessage.Journey), null)]
    [MapValue(nameof(JourneyMessage.Type), MessageType.RisCause)]
    [MapProperty(nameof(RisJourneys.MessageRisCauseCode.Code), nameof(JourneyMessage.Code))]
    [MapProperty(nameof(RisJourneys.MessageRisCauseCode.Text), nameof(JourneyMessage.Text))]
    [MapValue(nameof(JourneyMessage.TextShort), null)]
    [MapValue(nameof(JourneyMessage.DisruptionCause), null)]
    [MapValue(nameof(JourneyMessage.DisruptionEffect), null)]
    [MapValue(nameof(JourneyMessage.NoteCategory), null)]
    [MapValue(nameof(JourneyMessage.References), null)]
    [MapValue(nameof(JourneyMessage.JourneyStopPlaceMessages), null)]
    public partial JourneyMessage MapMessageRisCause(RisJourneys.MessageRisCauseCode source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
#pragma warning restore RMG012 // Source member was not found for target member
    #endregion

    #region RisQualityDeviation
#pragma warning disable RMG012 // Source member was not found for target member
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapValue(nameof(JourneyMessage.Id), null)]
    [MapValue(nameof(JourneyMessage.JourneyId), null)]
    [MapValue(nameof(JourneyMessage.Journey), null)]
    [MapValue(nameof(JourneyMessage.Type), MessageType.RisQualityDeviation)]
    [MapProperty(nameof(RisJourneys.MessageRisQualityDeviation.Code), nameof(JourneyMessage.Code))]
    [MapProperty(nameof(RisJourneys.MessageRisQualityDeviation.Text), nameof(JourneyMessage.Text))]
    [MapValue(nameof(JourneyMessage.TextShort), null)]
    [MapValue(nameof(JourneyMessage.DisruptionCause), null)]
    [MapValue(nameof(JourneyMessage.DisruptionEffect), null)]
    [MapValue(nameof(JourneyMessage.NoteCategory), null)]
    [MapValue(nameof(JourneyMessage.References), null)]
    [MapValue(nameof(JourneyMessage.JourneyStopPlaceMessages), null)]
    public partial JourneyMessage MapMessageRisQualityDeviation(RisJourneys.MessageRisQualityDeviation source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
#pragma warning restore RMG012 // Source member was not found for target member
    #endregion

    #endregion
}
