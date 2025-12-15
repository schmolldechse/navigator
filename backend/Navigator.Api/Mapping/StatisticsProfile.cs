using AutoMapper;
using Navigator.Api.DTOs.Statistics;
using Navigator.Data.Entities.Statistics;

namespace Navigator.Api.Mapping;

public class StatisticsProfile : Profile
{
    public StatisticsProfile()
    {
        CreateMap<DatabaseSize, MeasuredStatisticValue>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.MeasuredAt))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.SizeInBytes));
    }
}
