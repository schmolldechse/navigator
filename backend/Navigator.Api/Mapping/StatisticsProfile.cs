using AutoMapper;
using Navigator.Api.DTOs.Statistics;
using Navigator.Data.Models.Statistics;

namespace Navigator.Api.Mapping;

public class StatisticsProfile : Profile
{
    public StatisticsProfile()
    {
        CreateMap<Data.Models.Statistics.MetricDataPoint, Api.DTOs.Statistics.MetricDataPoint>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

        CreateMap<MetricDataSummary, MetricSummary>()
            .ForMember(dest => dest.StartValue, opt => opt.MapFrom(src => src.StartValue))
            .ForMember(dest => dest.EndValue, opt => opt.MapFrom(src => src.EndValue))
            .ForMember(dest => dest.MinValue, opt => opt.MapFrom(src => src.MinValue))
            .ForMember(dest => dest.MaxValue, opt => opt.MapFrom(src => src.MaxValue))
            .ForMember(dest => dest.AbsoluteChange, opt => opt.MapFrom(src => src.AbsoluteChange));

        CreateMap<MetricDataSet, MetricSeries>()
            .ForMember(dest => dest.Timerange, opt => opt.Ignore())
            .ForMember(dest => dest.SeriesType, opt => opt.MapFrom(src => src.SeriesType))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
            .ForMember(dest => dest.IsCumulative, opt => opt.MapFrom(src => src.IsCumulative))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.DataPoints, opt => opt.MapFrom(src => src.DataPoints));
    }
}
