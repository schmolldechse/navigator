using AutoMapper;

namespace Navigator.Api.Mapping;

public class StatisticsProfile : Profile
{
    public StatisticsProfile()
    {
        // --- DataPoint ---
        CreateMap<Data.Models.Statistics.BaseMetricDataPoint, Api.DTOs.Statistics.BaseMetricDataPoint>()
            .Include<Data.Models.Statistics.DataPoint.TimestampMetricDataPoint, Api.DTOs.Statistics.DataPoint.TimestampMetricDataPoint>()
            .Include<Data.Models.Statistics.DataPoint.TransportTypeMetricDataPoint, Api.DTOs.Statistics.DataPoint.TransportTypeMetricDataPoint>()
            .Include<Data.Models.Statistics.DataPoint.TimestampTransportTypeMetricDataPoint, Api.DTOs.Statistics.DataPoint.TimestampTransportTypeMetricDataPoint>();

        CreateMap<Data.Models.Statistics.DataPoint.TimestampMetricDataPoint, Api.DTOs.Statistics.DataPoint.TimestampMetricDataPoint>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

        CreateMap<Data.Models.Statistics.DataPoint.TransportTypeMetricDataPoint, Api.DTOs.Statistics.DataPoint.TransportTypeMetricDataPoint>()
            .ForMember(dest => dest.TransportType, opt => opt.MapFrom(src => src.TransportType))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

        CreateMap<Data.Models.Statistics.DataPoint.TimestampTransportTypeMetricDataPoint, Api.DTOs.Statistics.DataPoint.TimestampTransportTypeMetricDataPoint>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.TransportType, opt => opt.MapFrom(src => src.TransportType))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

        // --- Incoming Metric Request ---
        CreateMap<Data.Models.Statistics.BaseMetricRequest, Api.DTOs.Statistics.BaseMetricRequest>()
            .Include<Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest, Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest>()
            .Include<Data.Models.Statistics.Request.GlobalStopSummaryMetricRequest, Api.DTOs.Statistics.Request.GlobalStopSummaryMetricRequest>()
            .Include<Data.Models.Statistics.Request.JourneySnapshotMetricRequest, Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest>()
            .Include<Data.Models.Statistics.Request.RisIdSnapshotMetricRequest, Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest>()
            .Include<Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest, Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest>();

        CreateMap<Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest, Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest>()
            .ForMember(dest => dest.MetricQueryType, opt => opt.MapFrom(src => src.MetricQueryType))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End));

        CreateMap<Data.Models.Statistics.Request.GlobalStopSummaryMetricRequest, Api.DTOs.Statistics.Request.GlobalStopSummaryMetricRequest>()
            .ForMember(dest => dest.MetricQueryType, opt => opt.MapFrom(src => src.MetricQueryType))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End))
            .ForMember(dest => dest.TransportTypes, opt => opt.MapFrom(src => src.TransportTypes));

        CreateMap<Data.Models.Statistics.Request.JourneySnapshotMetricRequest, Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest>()
            .ForMember(dest => dest.MetricQueryType, opt => opt.MapFrom(src => src.MetricQueryType))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End));

        CreateMap<Data.Models.Statistics.Request.RisIdSnapshotMetricRequest, Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest>()
            .ForMember(dest => dest.MetricQueryType, opt => opt.MapFrom(src => src.MetricQueryType))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End));

        CreateMap<Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest, Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest>()
            .ForMember(dest => dest.MetricQueryType, opt => opt.MapFrom(src => src.MetricQueryType))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End))
            .ForMember(dest => dest.TransportTypes, opt => opt.MapFrom(src => src.TransportTypes));

        // --- General ---
        CreateMap<Data.Models.Statistics.MetricSummary, Api.DTOs.Statistics.MetricSummary>()
            .ForMember(dest => dest.StartValue, opt => opt.MapFrom(src => src.StartValue))
            .ForMember(dest => dest.EndValue, opt => opt.MapFrom(src => src.EndValue))
            .ForMember(dest => dest.MinValue, opt => opt.MapFrom(src => src.MinValue))
            .ForMember(dest => dest.MaxValue, opt => opt.MapFrom(src => src.MaxValue))
            .ForMember(dest => dest.AbsoluteChange, opt => opt.MapFrom(src => src.AbsoluteChange));

        CreateMap<Data.Models.Statistics.MetricSeries, Api.DTOs.Statistics.MetricSeries>()
            .ForMember(dest => dest.SeriesType, opt => opt.MapFrom(src => src.SeriesType))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.DataPoints, opt => opt.MapFrom(src => src.DataPoints));
    }
}
