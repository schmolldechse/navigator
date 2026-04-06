using Navigator.Api.DTOs.Station;
using Navigator.Data.Enums;
using Navigator.Data.Models.Station;
using Riok.Mapperly.Abstractions;

namespace Navigator.Api.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StationMapper
{
    #region API to data layer requests
    [MapProperty(nameof(StationByGeographicCoordinatesRequest.Latitude), nameof(StationsByCoordinateRequest.Latitude))]
    [MapProperty(nameof(StationByGeographicCoordinatesRequest.Longitude), nameof(StationsByCoordinateRequest.Longitude))]
    [MapProperty(nameof(StationByGeographicCoordinatesRequest.Limit), nameof(StationsByCoordinateRequest.Limit))]
    [MapProperty(nameof(StationByGeographicCoordinatesRequest.MaxDistance), nameof(StationsByCoordinateRequest.MaxDistance))]
    public partial StationsByCoordinateRequest MapStationCoordinateRequest(StationByGeographicCoordinatesRequest source);

    [MapProperty(nameof(StationBySerchtermRequest.SearchTerm), nameof(VendoStationsBySearchRequest.SearchTerm))]
    [MapProperty(nameof(StationBySerchtermRequest.MaxResults), nameof(VendoStationsBySearchRequest.MaxResults))]
    [MapProperty(nameof(StationBySerchtermRequest.LocationTypes), nameof(VendoStationsBySearchRequest.LocationTypes))]
    public partial VendoStationsBySearchRequest MapStationBySearchtermRequest(StationBySerchtermRequest source);
    #endregion

    #region Vendo -> Station DTO
    [MapProperty(nameof(VendoStation.CoordinatesResponse.Latitude), nameof(Navigator.Api.DTOs.Station.StationPosition.Latitude))]
    [MapProperty(nameof(VendoStation.CoordinatesResponse.Longitude), nameof(Navigator.Api.DTOs.Station.StationPosition.Longitude))]
    public partial Navigator.Api.DTOs.Station.StationPosition MapVendoPositon(VendoStation.CoordinatesResponse source);

#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(VendoStation.EvaNumber), nameof(Navigator.Api.DTOs.Station.Station.EvaNumber))]
    [MapProperty(nameof(VendoStation.Name), nameof(Navigator.Api.DTOs.Station.Station.Name))]
    [MapProperty(nameof(VendoStation.Coordinates), nameof(Navigator.Api.DTOs.Station.Station.Position), Use = nameof(MapVendoPositon))]
    [MapValue(nameof(Navigator.Api.DTOs.Station.Station.Transports), null)]
    [MapperIgnoreTarget(nameof(Navigator.Api.DTOs.Station.Station.Ril100))]
    private partial Navigator.Api.DTOs.Station.Station MapVendoStationInternal(VendoStation source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    public Navigator.Api.DTOs.Station.Station MapVendoStation(VendoStation source)
    {
        var station = MapVendoStationInternal(source);
        station.Transports = source.Products.Select(product => MapVendoProduct(product)).ToArray();
        return station;
    }

    public static TransportType MapVendoProduct(string? sourceProduct) => sourceProduct?.ToUpperInvariant() switch
    {
        "HOCHGESCHWINDIGKEITSZUEGE" => TransportType.HighSpeedTrain,
        "INTERCITYUNDEUROCITYZUEGE" => TransportType.IntercityTrain,
        "INTERREGIOUNDSCHNELLZUEGE" => TransportType.InterRegionalTrain,
        "NAHVERKEHRSONSTIGEZUEGE" => TransportType.RegionalTrain,
        "SBAHNEN" => TransportType.CityTrain,
        "UBAHN" => TransportType.Subway,
        "STRASSENBAHN" => TransportType.Tram,
        "BUSSE" => TransportType.Bus,
        "SCHIFFE" => TransportType.Ferry,
        "TAXI" => TransportType.Taxi,
        "ANRUFPFLICHTIGEVERKEHRE" => TransportType.Shuttle,
        _ => throw new MappingException($"Value '{sourceProduct}' is not valid to be mapped for TransportType.")
    };
    #endregion

    #region Database -> Station DTO
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(Navigator.Data.Entities.Station.Station.EvaNumber), nameof(Navigator.Api.DTOs.Station.BaseStation.EvaNumber))]
    [MapProperty(nameof(Navigator.Data.Entities.Station.Station.Name), nameof(Navigator.Api.DTOs.Station.BaseStation.Name))]
    [MapValue(nameof(Navigator.Api.DTOs.Station.BaseStation.Position), null)]
    private partial BaseStation MapDatabaseStationToBaseStationInternal(Navigator.Data.Entities.Station.Station source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    [UserMapping(Default = true)]
    public BaseStation MapDatabaseStationToBaseStation(Navigator.Data.Entities.Station.Station source)
    {
        var station = MapDatabaseStationToBaseStationInternal(source);
        station.Position = new StationPosition
        {
            Latitude = source.Latitude,
            Longitude = source.Longitude
        };
        return station;
    }


#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(Navigator.Data.Entities.Station.Station.EvaNumber), nameof(Navigator.Api.DTOs.Station.Station.EvaNumber))]
    [MapProperty(nameof(Navigator.Data.Entities.Station.Station.Name), nameof(Navigator.Api.DTOs.Station.Station.Name))]
    [MapValue(nameof(Navigator.Api.DTOs.Station.Station.Position), null)]
    [MapValue(nameof(Navigator.Api.DTOs.Station.Station.Transports), null)]
    [MapValue(nameof(Navigator.Api.DTOs.Station.Station.Ril100), null)]
    private partial Station MapDatabaseStationToStationInternal(Navigator.Data.Entities.Station.Station source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    [UserMapping(Default = true)]
    public Station MapDatabaseStationToStation(Navigator.Data.Entities.Station.Station source)
    {
        var station = MapDatabaseStationToStationInternal(source);
        station.Transports = source.Transports.Select(transport => transport.TransportType).ToArray();
        station.Ril100 = source.Ril100.Select(ril => ril.Ril100Code).ToArray();
        return station;
    }


#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(Navigator.Data.Entities.Station.Station.QueryingEnabled), nameof(Navigator.Api.DTOs.Station.StationGatheringInfo.QueryingEnabled))]
    [MapProperty(nameof(Navigator.Data.Entities.Station.Station.LastQueried), nameof(Navigator.Api.DTOs.Station.StationGatheringInfo.LastQueried))]
    [MapValue(nameof(Navigator.Api.DTOs.Station.StationGatheringInfo.ActiveTransportTypes), null)]
    [MapValue(nameof(Navigator.Api.DTOs.Station.StationGatheringInfo.DisabledTransportTypes), null)]
    private partial StationGatheringInfo MapDatabaseStationToStationGatheringInfoInternal(Navigator.Data.Entities.Station.Station source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member

    [UserMapping(Default = true)]
    public StationGatheringInfo MapDatabaseStationToStationGatheringInfo(Navigator.Data.Entities.Station.Station source)
    {
        var gatheringInfo = MapDatabaseStationToStationGatheringInfoInternal(source);
        gatheringInfo.ActiveTransportTypes = source.Transports
            .Where(transport => transport.Enabled)
            .Select(transport => transport.TransportType)
            .ToArray();
        gatheringInfo.DisabledTransportTypes = source.Transports
            .Where(transport => !transport.Enabled)
            .Select(transport => transport.TransportType)
            .ToArray();
        return gatheringInfo;
    }
    #endregion

    #region Station DTO -> Database
#pragma warning disable RMG012 // Source member was not found for target member
#pragma warning disable RMG076 // Cannot assign null to non-nullable member
    [MapProperty(nameof(Navigator.Api.DTOs.Station.Station.EvaNumber), nameof(Navigator.Data.Entities.Station.Station.EvaNumber))]
    [MapProperty(nameof(Navigator.Api.DTOs.Station.Station.Name), nameof(Navigator.Data.Entities.Station.Station.Name))]
    [MapValue(nameof(Navigator.Data.Entities.Station.Station.Weight), 0D)]
    [MapValue(nameof(Navigator.Data.Entities.Station.Station.Latitude), -1D)]
    [MapValue(nameof(Navigator.Data.Entities.Station.Station.Longitude), -1D)]
    [MapValue(nameof(Navigator.Data.Entities.Station.Station.QueryingEnabled), false)]
    [MapValue(nameof(Navigator.Data.Entities.Station.Station.Ril100), null)]
    [MapValue(nameof(Navigator.Data.Entities.Station.Station.Transports), null)]
    private partial Navigator.Data.Entities.Station.Station MapStationToDatabaseStationInternal(Station source);
#pragma warning restore RMG076 // Cannot assign null to non-nullable member
#pragma warning restore RMG012 // Source member was not found for target member

    public Navigator.Data.Entities.Station.Station MapStationToDatabaseStation(Station source)
    {
        var station = MapStationToDatabaseStationInternal(source);
        station.Latitude = source.Position.Latitude;
        station.Longitude = source.Position.Longitude;

        station.Ril100 = (source.Ril100 ?? []).Select(ril => new Data.Entities.Station.StationRil100()
        {
            EvaNumber = station.EvaNumber,
            Ril100Code = ril,
        }).ToList();
        station.Transports = (station.Transports ?? []).Select(transport => new Data.Entities.Station.StationTransport()
        {
            EvaNumber = station.EvaNumber,
            TransportType = transport.TransportType,
            Enabled = false
        }).ToList();
        return station;
    }
    #endregion
}
