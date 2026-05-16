using Riok.Mapperly.Abstractions;

namespace Navigator.Api.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StatisticsMapper
{
    #region API to Data layer requests
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest, Navigator.Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest, Navigator.Data.Models.Statistics.Request.JourneySnapshotMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest, Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest, Navigator.Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.GlobalTransportQualityMetricRequest, Navigator.Data.Models.Statistics.Request.GlobalTransportQualityMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.StationQualityMetricRequest, Navigator.Data.Models.Statistics.Request.StationQualityMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.AdministrationRankingMetricRequest, Navigator.Data.Models.Statistics.Request.AdministrationRankingMetricRequest>]
    [MapDerivedType<Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest, Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest>]
    public partial Navigator.Data.Models.Statistics.BaseMetricRequest MapBaseRequest(Navigator.Api.DTOs.Statistics.BaseMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest.End))]
    public partial Navigator.Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest MapDatabaseSizeSnapshotRequest(Navigator.Api.DTOs.Statistics.Request.DatabaseSizeSnapshotMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.JourneySnapshotMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.JourneySnapshotMetricRequest.End))]
    public partial Navigator.Data.Models.Statistics.Request.JourneySnapshotMetricRequest MapJourneySnapshotRequest(Navigator.Api.DTOs.Statistics.Request.JourneySnapshotMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest.End))]
    public partial Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest MapRisIdSnapshotRequest(Navigator.Api.DTOs.Statistics.Request.RisIdSnapshotMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest.TransportTypes), nameof(Navigator.Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest.TransportTypes))]
    public partial Navigator.Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest MapTransportTypeDistributionRequest(Navigator.Api.DTOs.Statistics.Request.TransportTypeDistributionMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.GlobalTransportQualityMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.GlobalTransportQualityMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.GlobalTransportQualityMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.GlobalTransportQualityMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.GlobalTransportQualityMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.GlobalTransportQualityMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.GlobalTransportQualityMetricRequest.TransportTypes), nameof(Navigator.Data.Models.Statistics.Request.GlobalTransportQualityMetricRequest.TransportTypes))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.GlobalTransportQualityMetricRequest.Stepping), nameof(Navigator.Data.Models.Statistics.Request.GlobalTransportQualityMetricRequest.Stepping))]
    public partial Navigator.Data.Models.Statistics.Request.GlobalTransportQualityMetricRequest MapGlobalTransportQualityRequest(Navigator.Api.DTOs.Statistics.Request.GlobalTransportQualityMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationQualityMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.StationQualityMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationQualityMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.StationQualityMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationQualityMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.StationQualityMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationQualityMetricRequest.TransportTypes), nameof(Navigator.Data.Models.Statistics.Request.StationQualityMetricRequest.TransportTypes))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.StationQualityMetricRequest.EvaNumbers), nameof(Navigator.Data.Models.Statistics.Request.StationQualityMetricRequest.EvaNumbers))]
    public partial Navigator.Data.Models.Statistics.Request.StationQualityMetricRequest MapStationQualityRequest(Navigator.Api.DTOs.Statistics.Request.StationQualityMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.AdministrationRankingMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.AdministrationRankingMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.AdministrationRankingMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.AdministrationRankingMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.AdministrationRankingMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.AdministrationRankingMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.AdministrationRankingMetricRequest.EvaNumber), nameof(Navigator.Data.Models.Statistics.Request.AdministrationRankingMetricRequest.EvaNumber))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.AdministrationRankingMetricRequest.Limit), nameof(Navigator.Data.Models.Statistics.Request.AdministrationRankingMetricRequest.Limit))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.AdministrationRankingMetricRequest.Offset), nameof(Navigator.Data.Models.Statistics.Request.AdministrationRankingMetricRequest.Offset))]
    public partial Navigator.Data.Models.Statistics.Request.AdministrationRankingMetricRequest MapAdministrationRankingRequest(Navigator.Api.DTOs.Statistics.Request.AdministrationRankingMetricRequest source);

    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest.SeriesType), nameof(Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest.MetricSeriesType))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest.Start), nameof(Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest.Start))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest.End), nameof(Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest.End))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest.EvaNumber), nameof(Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest.EvaNumber))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest.LineRegex), nameof(Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest.LineRegex))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest.NumberRegex), nameof(Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest.NumberRegex))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest.Limit), nameof(Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest.Limit))]
    [MapProperty(nameof(Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest.Offset), nameof(Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest.Offset))]
    public partial Navigator.Data.Models.Statistics.Request.LineRankingMetricRequest MapLineRankingRequest(Navigator.Api.DTOs.Statistics.Request.LineRankingMetricRequest source);
    #endregion

    #region Data to API layer datapoints
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.AdministrationRankingDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.AdministrationRankingDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.LineRankingDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.LineRankingDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.StationDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.StationDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.TimestampDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint>]
    [MapDerivedType<Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint, Navigator.Api.DTOs.Statistics.DataPoint.TransportTypeDataPoint>]
    public partial Navigator.Api.DTOs.Statistics.BaseMetricDataPoint MapBaseDatapoint(Navigator.Data.Models.Statistics.BaseMetricDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.AdministrationRankingDataPoint.Value), nameof(Navigator.Api.DTOs.Statistics.DataPoint.AdministrationRankingDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.AdministrationRankingDataPoint.Sample), nameof(Navigator.Api.DTOs.Statistics.DataPoint.AdministrationRankingDataPoint.Sample))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.AdministrationRankingDataPoint.Administration), nameof(Navigator.Api.DTOs.Statistics.DataPoint.AdministrationRankingDataPoint.Administration))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.AdministrationRankingDataPoint MapAdministrationRankingDatapoint(Navigator.Data.Models.Statistics.DataPoint.AdministrationRankingDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.LineRankingDataPoint.Value), nameof(Navigator.Api.DTOs.Statistics.DataPoint.LineRankingDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.LineRankingDataPoint.Sample), nameof(Navigator.Api.DTOs.Statistics.DataPoint.LineRankingDataPoint.Sample))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.LineRankingDataPoint.Line), nameof(Navigator.Api.DTOs.Statistics.DataPoint.LineRankingDataPoint.Line))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.LineRankingDataPoint.Administration), nameof(Navigator.Api.DTOs.Statistics.DataPoint.LineRankingDataPoint.Administration))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.LineRankingDataPoint.StartStation), nameof(Navigator.Api.DTOs.Statistics.DataPoint.LineRankingDataPoint.StartStation))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.LineRankingDataPoint.EndStation), nameof(Navigator.Api.DTOs.Statistics.DataPoint.LineRankingDataPoint.EndStation))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.LineRankingDataPoint MapLineRankingDatapoint(Navigator.Data.Models.Statistics.DataPoint.LineRankingDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.StationDataPoint.Value), nameof(Navigator.Api.DTOs.Statistics.DataPoint.StationDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.StationDataPoint.Sample), nameof(Navigator.Api.DTOs.Statistics.DataPoint.StationDataPoint.Sample))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.StationDataPoint.Station), nameof(Navigator.Api.DTOs.Statistics.DataPoint.StationDataPoint.Station))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.StationDataPoint MapStationDatapoint(Navigator.Data.Models.Statistics.DataPoint.StationDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint.Value), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint.Sample), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampDataPoint.Sample))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint.Timestamp), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampDataPoint.Timestamp))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.TimestampDataPoint MapTimestampDatapoint(Navigator.Data.Models.Statistics.DataPoint.TimestampDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.Value), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.Sample), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint.Sample))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.Timestamp), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint.Timestamp))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint.TransportType), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint.TransportType))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.TimestampTransportTypeDataPoint MapTimestampTransportTypeDatapoint(Navigator.Data.Models.Statistics.DataPoint.TimestampTransportTypeDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint.Value), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TransportTypeDataPoint.Value))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint.Sample), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TransportTypeDataPoint.Sample))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint.TransportType), nameof(Navigator.Api.DTOs.Statistics.DataPoint.TransportTypeDataPoint.TransportType))]
    public partial Navigator.Api.DTOs.Statistics.DataPoint.TransportTypeDataPoint MapTransportTypeDatapoint(Navigator.Data.Models.Statistics.DataPoint.TransportTypeDataPoint source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.MetricSample.Numerator), nameof(Navigator.Api.DTOs.Statistics.MetricSample.Numerator))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.MetricSample.Denominator), nameof(Navigator.Api.DTOs.Statistics.MetricSample.Denominator))]
    public partial Navigator.Api.DTOs.Statistics.MetricSample MapMetricSample(Navigator.Data.Models.Statistics.MetricSample source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.Subject.AdministrationMetricSubject.AdministrationId), nameof(Navigator.Api.DTOs.Statistics.Subject.AdministrationMetricSubject.AdministrationId))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.Subject.AdministrationMetricSubject.OperatorCode), nameof(Navigator.Api.DTOs.Statistics.Subject.AdministrationMetricSubject.OperatorCode))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.Subject.AdministrationMetricSubject.OperatorName), nameof(Navigator.Api.DTOs.Statistics.Subject.AdministrationMetricSubject.OperatorName))]
    public partial Navigator.Api.DTOs.Statistics.Subject.AdministrationMetricSubject MapAdministrationSubject(Navigator.Data.Models.Statistics.Subject.AdministrationMetricSubject source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.Subject.LineMetricSubject.Number), nameof(Navigator.Api.DTOs.Statistics.Subject.LineMetricSubject.Number))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.Subject.LineMetricSubject.JourneyDescription), nameof(Navigator.Api.DTOs.Statistics.Subject.LineMetricSubject.JourneyDescription))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.Subject.LineMetricSubject.TransportType), nameof(Navigator.Api.DTOs.Statistics.Subject.LineMetricSubject.TransportType))]
    public partial Navigator.Api.DTOs.Statistics.Subject.LineMetricSubject MapLineSubject(Navigator.Data.Models.Statistics.Subject.LineMetricSubject source);

    [MapProperty(nameof(Navigator.Data.Models.Statistics.Subject.StationMetricSubject.EvaNumber), nameof(Navigator.Api.DTOs.Statistics.Subject.StationMetricSubject.EvaNumber))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.Subject.StationMetricSubject.Name), nameof(Navigator.Api.DTOs.Statistics.Subject.StationMetricSubject.Name))]
    public partial Navigator.Api.DTOs.Statistics.Subject.StationMetricSubject MapStationSubject(Navigator.Data.Models.Statistics.Subject.StationMetricSubject source);
    #endregion

    #region Data to API layer metricseries
    [MapProperty(nameof(Navigator.Data.Models.Statistics.MetricSeries.SeriesType), nameof(Navigator.Api.DTOs.Statistics.MetricSeries.SeriesType))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.MetricSeries.Unit), nameof(Navigator.Api.DTOs.Statistics.MetricSeries.Unit))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.MetricSeries.Page), nameof(Navigator.Api.DTOs.Statistics.MetricSeries.Page))]
    [MapProperty(nameof(Navigator.Data.Models.Statistics.MetricSeries.DataPoints), nameof(Navigator.Api.DTOs.Statistics.MetricSeries.DataPoints))]
    public partial Navigator.Api.DTOs.Statistics.MetricSeries MapMetricSeries(Navigator.Data.Models.Statistics.MetricSeries source);
    #endregion
}
