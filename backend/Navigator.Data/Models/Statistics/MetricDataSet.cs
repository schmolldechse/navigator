using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics;

public class MetricDataSet
{
    public required MetricSeriesType SeriesType { get; set; }
    public required MetricUnit Unit { get; set; }

    public bool IsCumulative { get; set; }

    public required MetricDataSummary Summary { get; set; }
    
    public required IEnumerable<MetricDataPoint> DataPoints { get; set; }
}