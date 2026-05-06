using Riok.Mapperly.Abstractions;

namespace Navigator.Api.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StatisticsMapper
{
    #region API to Data layer requests
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest, Navigator.Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.HourlyTransportSnapshotMetricRequest, Navigator.Data.Models.Statistics.Request.HourlyTransportSnapshotMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest, Navigator.Data.Models.Statistics.Request.JourneySnapshotMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.JourneyServiceMetricRequest, Navigator.Data.Models.Statistics.Request.JourneyServiceMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.MessageSummaryMetricRequest, Navigator.Data.Models.Statistics.Request.MessageSummaryMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest, Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.StationSummaryMetricRequest, Navigator.Data.Models.Statistics.Request.StationSummaryMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.StationTimeSeriesMetricRequest, Navigator.Data.Models.Statistics.Request.StationTimeSeriesMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest, Navigator.Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest>]
    public partial Navigator.Data.Models.Statistics.BaseMetricRequest MapBaseRequest(Navigator.Api.DTOs.Statistics.BaseMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest.End))]
    public partial Navigator.Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest MapDatabaseSizeSnapshotRequest(Navigator.Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.HourlyTransportSnapshotMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.HourlyTransportSnapshotMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.HourlyTransportSnapshotMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.HourlyTransportSnapshotMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.HourlyTransportSnapshotMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.HourlyTransportSnapshotMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.HourlyTransportSnapshotMetricRequest.TransportTypes), nameof(Navigator.Data.Models.Statistics.Request.HourlyTransportSnapshotMetricRequest.TransportTypes))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.HourlyTransportSnapshotMetricRequest.Stepping), nameof(Navigator.Data.Models.Statistics.Request.HourlyTransportSnapshotMetricRequest.Stepping))]
    public partial Navigator.Data.Models.Statistics.Request.HourlyTransportSnapshotMetricRequest MapHourlyTransportSnapshotRequest(Navigator.Api.DTOs.Statistics.Request.HourlyTransportSnapshotMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.JourneySnapshotMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.JourneySnapshotMetricRequest.End))]
    public partial Navigator.Data.Models.Statistics.Request.JourneySnapshotMetricRequest MapJourneySnapshotRequest(Navigator.Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneyServiceMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.JourneyServiceMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneyServiceMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.JourneyServiceMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneyServiceMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.JourneyServiceMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneyServiceMetricRequest.TransportTypes), nameof(Navigator.Data.Models.Statistics.Request.JourneyServiceMetricRequest.TransportTypes))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneyServiceMetricRequest.JourneyTypes), nameof(Navigator.Data.Models.Statistics.Request.JourneyServiceMetricRequest.JourneyTypes))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneyServiceMetricRequest.OperatorCodes), nameof(Navigator.Data.Models.Statistics.Request.JourneyServiceMetricRequest.OperatorCodes))]
    public partial Navigator.Data.Models.Statistics.Request.JourneyServiceMetricRequest MapJourneyServiceRequest(Navigator.Api.DTOs.Statistics.Request.JourneyServiceMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.MessageSummaryMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.MessageSummaryMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.MessageSummaryMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.MessageSummaryMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.MessageSummaryMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.MessageSummaryMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.MessageSummaryMetricRequest.TransportTypes), nameof(Navigator.Data.Models.Statistics.Request.MessageSummaryMetricRequest.TransportTypes))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.MessageSummaryMetricRequest.MessageTypes), nameof(Navigator.Data.Models.Statistics.Request.MessageSummaryMetricRequest.MessageTypes))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.MessageSummaryMetricRequest.EvaNumbers), nameof(Navigator.Data.Models.Statistics.Request.MessageSummaryMetricRequest.EvaNumbers))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.MessageSummaryMetricRequest.Limit), nameof(Navigator.Data.Models.Statistics.Request.MessageSummaryMetricRequest.Limit))]
    public partial Navigator.Data.Models.Statistics.Request.MessageSummaryMetricRequest MapMessageSummaryRequest(Navigator.Api.DTOs.Statistics.Request.MessageSummaryMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest.End))]
    public partial Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest MapRisIdSnapshotRequest(Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationSummaryMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.StationSummaryMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationSummaryMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.StationSummaryMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationSummaryMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.StationSummaryMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationSummaryMetricRequest.TransportTypes), nameof(Navigator.Data.Models.Statistics.Request.StationSummaryMetricRequest.TransportTypes))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationSummaryMetricRequest.EvaNumbers), nameof(Navigator.Data.Models.Statistics.Request.StationSummaryMetricRequest.EvaNumbers))]
    public partial Navigator.Data.Models.Statistics.Request.StationSummaryMetricRequest MapStationSummaryRequest(Navigator.Api.DTOs.Statistics.Request.StationSummaryMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationTimeSeriesMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.StationTimeSeriesMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationTimeSeriesMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.StationTimeSeriesMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationTimeSeriesMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.StationTimeSeriesMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationTimeSeriesMetricRequest.TransportTypes), nameof(Navigator.Data.Models.Statistics.Request.StationTimeSeriesMetricRequest.TransportTypes))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationTimeSeriesMetricRequest.EvaNumbers), nameof(Navigator.Data.Models.Statistics.Request.StationTimeSeriesMetricRequest.EvaNumbers))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationTimeSeriesMetricRequest.Stepping), nameof(Navigator.Data.Models.Statistics.Request.StationTimeSeriesMetricRequest.Stepping))]
    public partial Navigator.Data.Models.Statistics.Request.StationTimeSeriesMetricRequest MapStationTimeSeriesRequest(Navigator.Api.DTOs.Statistics.Request.StationTimeSeriesMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest.TransportTypes), nameof(Navigator.Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest.TransportTypes))]
    public partial Navigator.Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest MapTransportTypeDistributionRequest(Navigator.Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest source);
    #endregion

    #region Data to API layer datapoints
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.StationDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.StationDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.CategoryDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.CategoryDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.TimestampCategoryDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.TimestampCategoryDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.TimestampDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.TimestampStationTransportTypeDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.TimestampStationTransportTypeDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.TransportTypeDataPoint>]
    public partial Navigator.Api.DTOs.Statistics.BaseMetricDataPoint MapBaseDatapoint(Navigator.Data.Models.Statistics.BaseMetricDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.CategoryDataPoint.Value), nameof(Navigator.Api.DTOs.Statistics.DataPoint.CategoryDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.CategoryDataPoint.Category), nameof(Navigator.Api.DTOs.Statistics.DataPoint.CategoryDataPoint.Category))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.CategoryDataPoint MapCategoryDatapoint(Navigator.Data.Models.Statistics.DataPoint.CategoryDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.StationDataPoint.Value), nameof(Navigator.Data.Models.Statistics.DataPoint.StationDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.StationDataPoint.EvaNumber), nameof(Navigator.Data.Models.Statistics.DataPoint.StationDataPoint.EvaNumber))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.StationDataPoint MapStationDatapoint(Navigator.Data.Models.Statistics.DataPoint.StationDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampCategoryDataPoint.Value), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampCategoryDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampCategoryDataPoint.Timestamp), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampCategoryDataPoint.Timestamp))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampCategoryDataPoint.Category), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampCategoryDataPoint.Category))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.TimestampCategoryDataPoint MapTimestampCategoryDatapoint(Navigator.Data.Models.Statistics.DataPoint.TimestampCategoryDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint.Value), nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint.Timestamp), nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint.Timestamp))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.TimestampDataPoint MapTimestampDatapoint(Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampStationTransportTypeDataPoint.Value), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampStationTransportTypeDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampStationTransportTypeDataPoint.Timestamp), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampStationTransportTypeDataPoint.Timestamp))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampStationTransportTypeDataPoint.EvaNumber), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampStationTransportTypeDataPoint.EvaNumber))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampStationTransportTypeDataPoint.TransportType), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampStationTransportTypeDataPoint.TransportType))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.TimestampStationTransportTypeDataPoint MapTimestampStationTransportTypeDatapoint(Navigator.Data.Models.Statistics.DataPoint.TimestampStationTransportTypeDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.Value), nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.Timestamp), nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.Timestamp))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.TransportType), nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.TransportType))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint MapTimestampTransportTypeDatapoint(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint.Value), nameof(Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint.TransportType), nameof(Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint.TransportType))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.TransportTypeDataPoint MapTransportTypeDatapoint(Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint source);
    #endregion

    #region Data to API layer metricseries
    [MapProperty(nameof(Navigator.Data.Models.Statistics.MetricSeries.SeriesType), nameof(Navigator.Api.DTOs.Statistics.MetricSeries.SeriesType))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.MetricSeries.Unit), nameof(Navigator.Api.DTOs.Statistics.MetricSeries.Unit))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.MetricSeries.DataPoints), nameof(Navigator.Api.DTOs.Statistics.MetricSeries.DataPoints))]
    public partial Navigator.Api.DTOs.Statistics.MetricSeries MapMetricSeries(Navigator.Data.Models.Statistics.MetricSeries source);
    #endregion
}
