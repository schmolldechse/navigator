using AutoMapper;
using Navigator.Api.DTOs.Station;
using Navigator.Data.Entities.Station;
using Navigator.Data.Enums;
using Navigator.Data.Models.Station;

namespace Navigator.Data.Mapping;

public class StationProfile : Profile
{
    public StationProfile()
    {
        // Repository requests
        CreateMap<CoordinatesRequestDTO, StationsByCoordinateRequest>();
        CreateMap<StationSearchRequestDTO, VendoStationsBySearchRequest>();

        // DTOs
        CreateMap<VendoStation.CoordinatesResponse, PositionDTO>();

        CreateMap<VendoStation, StationDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => int.Parse(src.EvaNumber)))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Coordinates))
            .ForMember(dest => dest.Transports, opt => opt.MapFrom(src => src.Products.Select(product => MapProductToTransport(product))));

        CreateMap<Station, StationSummaryDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => src.EvaNumber))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => new PositionDTO
            {
                Latitude = src.Latitude,
                Longitude = src.Longitude
            }));

        CreateMap<StationDTO, Station>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => src.EvaNumber))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Position.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Position.Longitude))
            .ForMember(dest => dest.Transports, opt => opt.MapFrom(src => src.Transports.Select(transport => new StationTransport()
            {
                EvaNumber = src.EvaNumber,
                TransportType = transport,
                Enabled = false
            }).ToList()))
            .ForMember(dest => dest.QueryingEnabled, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.LastQueried, opt => opt.Ignore())
            .ForMember(dest => dest.Ril100, opt => opt.Ignore());

        CreateMap<Station, StationDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => src.EvaNumber))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => new PositionDTO
            {
                Latitude = src.Latitude,
                Longitude = src.Longitude
            }))
            .ForMember(dest => dest.Ril100, opt => opt.MapFrom(src => src.Ril100.Select(ril => ril.Ril100Code).ToArray()))
            .ForMember(dest => dest.Transports, opt => opt.MapFrom(src => src.Transports.Select(transport => transport.TransportType).ToArray()));

        CreateMap<Station, StationGatheringInfoDTO>()
            .ForMember(dest => dest.QueryingEnabled, opt => opt.MapFrom(src => src.QueryingEnabled))
            .ForMember(dest => dest.LastQueried, opt => opt.MapFrom(src => src.LastQueried))
            .ForMember(dest => dest.ActiveTransportTypes, opt => opt.MapFrom(src => src.Transports.Where(transport => transport.Enabled == true).Select(transport => transport.TransportType).ToArray()))
            .ForMember(dest => dest.DisabledTransportTypes, opt => opt.MapFrom(src => src.Transports.Where(transport => transport.Enabled == false).Select(transport => transport.TransportType).ToArray()));
    }

    private TransportType? MapProductToTransport(string product)
    {
        if (product is null) return null;
        return product.ToUpperInvariant() switch
        {
            "HOCHGESCHWINDIGKEITSZUEGE" => TransportType.HighSpeedTrain,
            "INTERCITYUNDEUROCITYZUEGE" => TransportType.IntercityTrain,
            "INTERREGIOUNDSCHNELLZUEGE" => TransportType.InterRegionalTrain,
            "NAHVERKEHRSONSTIGEZUEGE" => TransportType.RegionalTrain,
            "SBAHNEN" => TransportType.CityTrain,
            "BUSSE" => TransportType.Bus,
            "SCHIFFE" => TransportType.Ferry,
            "UBAHN" => TransportType.Subway,
            "STRASSENBAHN" => TransportType.Tram,
            "ANRUFPFLICHTIGEVERKEHRE" => TransportType.Shuttle,
            _ => TransportType.Unknown
        };
    }
}
