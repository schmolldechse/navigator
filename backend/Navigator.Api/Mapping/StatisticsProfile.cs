using AutoMapper;

namespace Navigator.Api.Mapping;

public class StatisticsProfile : Profile
{
    public StatisticsProfile()
    {
        // --- DataPoint ---
        CreateMap<Data.Models.Statistics.BaseMetricDataPoint, Api.DTOs.Statistics.BaseMetricDataPoint>()
            .Include<Data.Models.Statistics.DataPoint.TimestampDataPoint, Api.DTOs.Statistics.DataPoint.TimestampDataPoint>()
            .Include<Data.Models.Statistics.DataPoint.TransportTypeDataPoint, Api.DTOs.Statistics.DataPoint.TransportTypeDataPoint>()
            .Include<Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint, Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint>()
            .Include<Data.Models.Statistics.DataPoint.StationDataPoint, Api.DTOs.Statistics.DataPoint.StationDataPoint>();

        CreateMap<Data.Models.Statistics.DataPoint.TimestampDataPoint, Api.DTOs.Statistics.DataPoint.TimestampDataPoint>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

        CreateMap<Data.Models.Statistics.DataPoint.TransportTypeDataPoint, Api.DTOs.Statistics.DataPoint.TransportTypeDataPoint>()
            .ForMember(dest => dest.TransportType, opt => opt.MapFrom(src => src.TransportType))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

        CreateMap<Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint, Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.TransportType, opt => opt.MapFrom(src => src.TransportType))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

        CreateMap<Data.Models.Statistics.DataPoint.StationDataPoint, Api.DTOs.Statistics.DataPoint.StationDataPoint>()
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => src.EvaNumber))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

        // --- Incoming Metric Request ---
        CreateMap<Data.Models.Statistics.BaseMetricRequest, Api.DTOs.Statistics.BaseMetricRequest>()
            .Include<Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest, Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest>()
            .Include<Data.Models.Statistics.Request.HourlyTransportSnapshotMetricRequest, Api.DTOs.Statistics.Request.HourlyTransportSnapshotMetricRequest>()
            .Include<Data.Models.Statistics.Request.JourneySnapshotMetricRequest, Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest>()
            .Include<Data.Models.Statistics.Request.RisIdSnapshotMetricRequest, Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest>()
            .Include<Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest, Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest>();

        CreateMap<Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest, Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest>()
            .ForMember(dest => dest.MetricQueryType, opt => opt.MapFrom(src => src.MetricQueryType))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End));

        CreateMap<Data.Models.Statistics.Request.HourlyTransportSnapshotMetricRequest, Api.DTOs.Statistics.Request.HourlyTransportSnapshotMetricRequest>()
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
        CreateMap<Data.Models.Statistics.MetricSeries, Api.DTOs.Statistics.MetricSeries>()
            .ForMember(dest => dest.SeriesType, opt => opt.MapFrom(src => src.SeriesType))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
            .ForMember(dest => dest.DataPoints, opt => opt.MapFrom(src => src.DataPoints));
    }
}
