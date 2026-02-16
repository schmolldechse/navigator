using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics;

public class MetricSeries
{
    public required MetricSeriesType SeriesType { get; set; }
    public required MetricUnit Unit { get; set; }

    public required MetricSummary Summary { get; set; }
    
    public required IEnumerable<BaseMetricDataPoint> DataPoints { get; set; }
}