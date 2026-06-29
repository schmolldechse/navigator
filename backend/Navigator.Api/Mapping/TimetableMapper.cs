using Navigator.Api.DTOs.Timetable;
using Navigator.Api.Enums;
using Navigator.Data.Enums;
using Navigator.Data.Models.Ris;
using Riok.Mapperly.Abstractions;

namespace Navigator.Api.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TimetableMapper
{
    #region Timetable Administration
    [MapProperty(nameof(RisBoards.Administration.AdministrationID), nameof(TimetableEntryAdministration.AdministrationId))]
    [MapProperty(nameof(RisBoards.Administration.OperatorCode), nameof(TimetableEntryAdministration.OperatorCode))]
    [MapProperty(nameof(RisBoards.Administration.OperatorName), nameof(TimetableEntryAdministration.OperatorName))]
    public partial TimetableEntryAdministration MapAdministration(RisBoards.Administration source);
    #endregion

    #region Timetable Entry
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(RisBoards.StopDeparture.JourneyID), nameof(TimetableDeparture.JourneyId))]
    [MapProperty(nameof(RisBoards.StopDeparture.Administration), nameof(TimetableDeparture.Administration))]
    [MapProperty(nameof(RisBoards.StopDeparture.Transport), nameof(TimetableDeparture.Transport), Use = nameof(MapDepartureTransport))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisBoards.StopDeparture.Transport.Destination), nameof(TimetableDeparture.Destination), Use = nameof(MapRichStopPlace))]
    [MapValue(nameof(TimetableDeparture.DifferingDestination), null)] // DifferingDestination is determined separately as it requires multiple source properties to be mapped
    [MapValue(nameof(TimetableDeparture.Direction), null)] // Direction is determined separately as it requires multiple source properties to be mapped
    [MapValue(nameof(TimetableDeparture.ViaStops), null)] // ViaStops is determined separately as it requires multiple source properties to be mapped
    [MapValue(nameof(TimetableDeparture.Schedule), null)] // Schedule is determined separately as it requires multiple source properties to be mapped
    [MapValue(nameof(TimetableDeparture.Messages), null)] // Messages require custom mapping logic
    [MapProperty(nameof(RisBoards.StopDeparture.Canceled), nameof(TimetableDeparture.Cancelled))]
    [MapProperty(nameof(RisBoards.StopDeparture.Additional), nameof(TimetableDeparture.Additional))]
    [MapProperty(nameof(RisBoards.StopDeparture.OnDemand), nameof(TimetableDeparture.Demand))]
    [MapValue(nameof(TimetableDeparture.TravelsWith), null)] // TravelsWith is determined separately as it requires multiple source properties to be mapped
    private partial TimetableDeparture MapDepartureTimetableInternal(RisBoards.StopDeparture source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    [UserMapping(Default = true)]
    public TimetableDeparture MapDepartureTimetable(RisBoards.StopDeparture source)
    {
        var timetableEntry = MapDepartureTimetableInternal(source);

        if (source.Transport?.DifferingDestination != null)
            timetableEntry.DifferingDestination = MapRichStopPlace(source.Transport.DifferingDestination);

        timetableEntry.Direction = source.Transport?.Direction?.StopPlaces?.Select(MapStopPlace) ?? Enumerable.Empty<TimetableEntryStopPlace>();
        timetableEntry.ViaStops = source.Transport?.Via?.Select(MapRichStopPlacePrio) ?? Enumerable.Empty<TimetableEntryRichStopPlace>();

        timetableEntry.Transport.JourneyType = MapJourneyType(source.JourneyType);
        timetableEntry.Schedule = MapDepartureSchedule(source);

        var messages = new List<TimetableEntryMessage>();

        messages.AddRange((source.Attributes ?? []).Select(attribute => new TimetableEntryMessage()
        {
            Type = MessageType.Attribute,
            Key = MapMessageKey(attribute.Code),
            Text = attribute.Text,
            TextShort = attribute.TextShort
        }));

        messages.AddRange((source.Disruptions ?? []).SelectMany(disruption => disruption.Descriptions).Where(description => description.Key.Contains("DE")).Select(description => new TimetableEntryMessage()
        {
            Type = MessageType.Disruption,
            Key = MessageKey.UNPLANNED_INFO,
            Text = description.Value.Text,
            TextShort = description.Value.Text
        }));

        messages.AddRange((source.Messages ?? []).Select(message => new TimetableEntryMessage()
        {
            Type = MessageType.Note,
            Key = MapMessageKey(message.Code),
            Text = message.Text,
            TextShort = message.TextShort
        }));

        timetableEntry.TravelsWith = source.TravelsWith?.Select(MapTravelsWithDeparture) ?? Enumerable.Empty<TimetableEntryCoupledTransport>();

        return timetableEntry;
    }

#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(RisBoards.StopArrival.JourneyID), nameof(TimetableArrival.JourneyId))]
    [MapProperty(nameof(RisBoards.StopArrival.Administration), nameof(TimetableArrival.Administration))]
    [MapProperty(nameof(RisBoards.StopArrival.Transport), nameof(TimetableArrival.Transport), Use = nameof(MapArrivalTransport))]
    [MapProperty(nameof(@Navigator.Data.Models.Ris.RisBoards.StopArrival.Transport.Origin), nameof(TimetableArrival.Origin), Use = nameof(MapRichStopPlace))]
    [MapValue(nameof(TimetableArrival.DifferingOrigin), null)] // DifferingOrigin is determined separately as it requires multiple source properties to be mapped
    [MapValue(nameof(TimetableArrival.Direction), null)] // Direction is determined separately as it requires multiple source properties to be mapped
    [MapValue(nameof(TimetableArrival.ViaStops), null)] // ViaStops is determined separately as it requires multiple source properties to be mapped
    [MapValue(nameof(TimetableArrival.Schedule), null)] // Schedule is determined separately as it requires multiple source properties to be mapped
    [MapValue(nameof(TimetableArrival.Messages), null)] // Messages require custom mapping logic
    [MapProperty(nameof(RisBoards.StopArrival.Canceled), nameof(TimetableArrival.Cancelled))]
    [MapProperty(nameof(RisBoards.StopArrival.Additional), nameof(TimetableArrival.Additional))]
    [MapProperty(nameof(RisBoards.StopArrival.OnDemand), nameof(TimetableArrival.Demand))]
    [MapValue(nameof(TimetableArrival.TravelsWith), null)] // TravelsWith is determined separately as it requires multiple source properties to be mapped
    private partial TimetableArrival MapArrivalTimetableInternal(RisBoards.StopArrival source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    [UserMapping(Default = true)]
    public TimetableArrival MapArrivalTimetable(RisBoards.StopArrival source)
    {
        var timetableEntry = MapArrivalTimetableInternal(source);

        if (source.Transport?.DifferingOrigin != null)
            timetableEntry.DifferingOrigin = MapRichStopPlace(source.Transport.DifferingOrigin);

        timetableEntry.Direction = source.Transport?.Direction?.StopPlaces?.Select(MapStopPlace) ?? Enumerable.Empty<TimetableEntryStopPlace>();
        timetableEntry.ViaStops = source.Transport?.Via?.Select(MapRichStopPlacePrio) ?? Enumerable.Empty<TimetableEntryRichStopPlace>();

        timetableEntry.Transport.JourneyType = MapJourneyType(source.JourneyType);
        timetableEntry.Schedule = MapArrivalSchedule(source);

        var messages = new List<TimetableEntryMessage>();

        messages.AddRange((source.Attributes ?? []).Select(attribute => new TimetableEntryMessage()
        {
            Type = MessageType.Attribute,
            Key = MapMessageKey(attribute.Code),
            Text = attribute.Text,
            TextShort = attribute.TextShort
        }));

        messages.AddRange((source.Disruptions ?? []).SelectMany(disruption => disruption.Descriptions).Where(description => description.Key.Contains("DE")).Select(description => new TimetableEntryMessage()
        {
            Type = MessageType.Disruption,
            Key = MessageKey.UNPLANNED_INFO,
            Text = description.Value.Text,
            TextShort = description.Value.Text
        }));

        messages.AddRange((source.Messages ?? []).Select(message => new TimetableEntryMessage()
        {
            Type = MessageType.Note,
            Key = MapMessageKey(message.Code),
            Text = message.Text,
            TextShort = message.TextShort
        }));

        timetableEntry.TravelsWith = source.TravelsWith?.Select(travelsWith => travelsWith.JourneyID) ?? Enumerable.Empty<string>();

        return timetableEntry;
    }

    private JourneyType MapJourneyType(RisBoards.JourneyType sourceType) => sourceType switch
    {
        RisBoards.JourneyType.REGULAR => JourneyType.Regular,
        RisBoards.JourneyType.REPLACEMENT => JourneyType.Replacement,
        RisBoards.JourneyType.RELIEF => JourneyType.Relief,
        RisBoards.JourneyType.EXTRA => JourneyType.Extra,
        _ => throw new MappingException($"Value '{sourceType}' is not valid to be mapped for JourneyType.")
    };

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

    #region Timetable Transport
    [MapProperty(nameof(RisBoards.TransportPublicDestinationVia.Type), nameof(TimetableEntryTransport.Type), Use = nameof(MapRisTypeToTransportType))]
    [MapValue(nameof(TimetableEntryTransport.ReplacementType), null)]
    [MapProperty(nameof(RisBoards.TransportPublicDestinationVia.Category), nameof(TimetableEntryTransport.Category))]
    [MapProperty(nameof(RisBoards.TransportPublicDestinationVia.CategoryInternal), nameof(TimetableEntryTransport.CategoryInternal))]
    [MapValue(nameof(TimetableEntryTransport.JourneyType), JourneyType.Regular)] // JourneyType is determined at the TimetableEntry level
    [MapProperty(nameof(RisBoards.TransportPublicDestinationVia.JourneyDescription), nameof(TimetableEntryTransport.JourneyDescription))]
    [MapProperty(nameof(RisBoards.TransportPublicDestinationVia.Number), nameof(TimetableEntryTransport.Number))]
    [MapProperty(nameof(RisBoards.TransportPublicDestinationVia.Line), nameof(TimetableEntryTransport.Line))]
    private partial TimetableEntryTransport MapDepartureTransportInternal(RisBoards.TransportPublicDestinationVia source);

    [UserMapping(Default = true)]
    public TimetableEntryTransport MapDepartureTransport(RisBoards.TransportPublicDestinationVia source)
    {
        var transport = MapDepartureTransportInternal(source);

        if (!string.IsNullOrEmpty(source.ReplacementTransport?.RealType))
            transport.ReplacementType = MapStringToTransportType(source.ReplacementTransport?.RealType);

        return transport;
    }

    [MapProperty(nameof(RisBoards.TransportPublicOriginVia.Type), nameof(TimetableEntryTransport.Type), Use = nameof(MapRisTypeToTransportType))]
    [MapValue(nameof(TimetableEntryTransport.ReplacementType), null)]
    [MapProperty(nameof(RisBoards.TransportPublicOriginVia.Category), nameof(TimetableEntryTransport.Category))]
    [MapProperty(nameof(RisBoards.TransportPublicOriginVia.CategoryInternal), nameof(TimetableEntryTransport.CategoryInternal))]
    [MapValue(nameof(TimetableEntryTransport.JourneyType), JourneyType.Regular)] // JourneyType is determined at the TimetableEntry level
    [MapProperty(nameof(RisBoards.TransportPublicOriginVia.JourneyDescription), nameof(TimetableEntryTransport.JourneyDescription))]
    [MapProperty(nameof(RisBoards.TransportPublicOriginVia.Number), nameof(TimetableEntryTransport.Number))]
    [MapProperty(nameof(RisBoards.TransportPublicOriginVia.Line), nameof(TimetableEntryTransport.Line))]
    private partial TimetableEntryTransport MapArrivalTransportInternal(RisBoards.TransportPublicOriginVia source);

    [UserMapping(Default = true)]
    public TimetableEntryTransport MapArrivalTransport(RisBoards.TransportPublicOriginVia source)
    {
        var transport = MapArrivalTransportInternal(source);

        if (!string.IsNullOrEmpty(source.ReplacementTransport?.RealType))
            transport.ReplacementType = MapStringToTransportType(source.ReplacementTransport?.RealType);

        return transport;
    }

    private TransportType MapStringToTransportType(string? sourceType) => sourceType?.ToUpperInvariant() switch
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

    private TransportType MapRisTypeToTransportType(RisBoards.TransportType sourceType) => sourceType switch
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
        _ => TransportType.Unknown
    };
    #endregion

    #region Timetable StopPlaces
    [MapProperty(nameof(RisBoards.StopPlaceEmbedded.Name), nameof(TimetableEntryStopPlace.Name))]
    [MapProperty(nameof(RisBoards.StopPlaceEmbedded.EvaNumber), nameof(TimetableEntryStopPlace.EvaNumber), Use = nameof(ParseStationEvaNumber))]
    public partial TimetableEntryStopPlace MapStopPlace(RisBoards.StopPlaceEmbedded source);

    [MapProperty(nameof(RisBoards.StopAtStopPlace.Name), nameof(TimetableEntryRichStopPlace.Name))]
    [MapProperty(nameof(RisBoards.StopAtStopPlace.EvaNumber), nameof(TimetableEntryRichStopPlace.EvaNumber), Use = nameof(ParseStationEvaNumber))]
    [MapProperty(nameof(RisBoards.StopAtStopPlace.Canceled), nameof(TimetableEntryRichStopPlace.Cancelled))]
    [MapperIgnoreTarget(nameof(TimetableEntryRichStopPlace.Additional))]
    public partial TimetableEntryRichStopPlace MapRichStopPlace(RisBoards.StopAtStopPlace source);

    [MapProperty(nameof(RisBoards.StopAtStopPlacePrio.Name), nameof(TimetableEntryRichStopPlace.Name))]
    [MapProperty(nameof(RisBoards.StopAtStopPlacePrio.EvaNumber), nameof(TimetableEntryRichStopPlace.EvaNumber), Use = nameof(ParseStationEvaNumber))]
    [MapProperty(nameof(RisBoards.StopAtStopPlacePrio.Canceled), nameof(TimetableEntryRichStopPlace.Cancelled))]
    [MapProperty(nameof(RisBoards.StopAtStopPlacePrio.Additional), nameof(TimetableEntryRichStopPlace.Additional))]
    public partial TimetableEntryRichStopPlace MapRichStopPlacePrio(RisBoards.StopAtStopPlacePrio source);

    private int ParseStationEvaNumber(string? source)
    {
        if (!int.TryParse(source, out var evaNumber))
            throw new MappingException($"Value '{source}' is not valid to be mapped for Station EvaNumber.");
        return evaNumber;
    }
    #endregion

    #region Timetable Schedule
    [MapProperty(nameof(RisBoards.StopDeparture.TimeSchedule), nameof(TimetableEntrySchedule.PlannedTime))]
    [MapProperty(nameof(RisBoards.StopDeparture.Time), nameof(TimetableEntrySchedule.ActualTime))]
    [MapperIgnoreTarget(nameof(TimetableEntrySchedule.Delay))]
    [MapProperty(nameof(RisBoards.StopDeparture.PlatformSchedule), nameof(TimetableEntrySchedule.PlannedPlatform))]
    [MapProperty(nameof(RisBoards.StopDeparture.Platform), nameof(TimetableEntrySchedule.ActualPlatform))]
    [MapProperty(nameof(RisBoards.StopDeparture.TimeType), nameof(TimetableEntrySchedule.TimeType), Use = nameof(MapTimeType))]
    public partial TimetableEntrySchedule MapDepartureSchedule(RisBoards.StopDeparture source);

    [MapProperty(nameof(RisBoards.StopDeparture.TimeSchedule), nameof(TimetableEntrySchedule.PlannedTime))]
    [MapProperty(nameof(RisBoards.StopDeparture.Time), nameof(TimetableEntrySchedule.ActualTime))]
    [MapperIgnoreTarget(nameof(TimetableEntrySchedule.Delay))]
    [MapProperty(nameof(RisBoards.StopDeparture.PlatformSchedule), nameof(TimetableEntrySchedule.PlannedPlatform))]
    [MapProperty(nameof(RisBoards.StopDeparture.Platform), nameof(TimetableEntrySchedule.ActualPlatform))]
    [MapProperty(nameof(RisBoards.StopDeparture.TimeType), nameof(TimetableEntrySchedule.TimeType), Use = nameof(MapTimeType))]
    public partial TimetableEntrySchedule MapArrivalSchedule(RisBoards.StopArrival source);

    private TimeType MapTimeType(RisBoards.TimeType sourceType) => sourceType switch
    {
        RisBoards.TimeType.SCHEDULE => TimeType.Schedule,
        RisBoards.TimeType.PREVIEW => TimeType.Preview,
        RisBoards.TimeType.REAL => TimeType.Real,
        _ => throw new MappingException($"Value '{sourceType}' is not valid to be mapped for TimeType.")
    };
    #endregion

    #region Timetable Coupled Transports
    [MapProperty(nameof(RisBoards.TransportPublicDestinationPortionWorking.JourneyID), nameof(TimetableEntryCoupledTransport.JourneyId))]
    [MapValue(nameof(TimetableEntryCoupledTransport.SeparationAt), null)]
    private partial TimetableEntryCoupledTransport MapTravelsWithDepartureInternal(RisBoards.TransportPublicDestinationPortionWorking source);

    [UserMapping(Default = true)]
    public TimetableEntryCoupledTransport MapTravelsWithDeparture(RisBoards.TransportPublicDestinationPortionWorking source)
    {
        var coupledTransport = MapTravelsWithDepartureInternal(source);

        if (source.SeparationAt is not null)
            coupledTransport.SeparationAt = MapStopPlace(source.SeparationAt);

        return coupledTransport;
    }
    #endregion
}
