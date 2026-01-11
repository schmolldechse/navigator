using System.ComponentModel;

namespace Navigator.Data.Enums.Metric;

public enum MetricDimension
{
    /// <summary>
    /// Data is measured over time (e.g. Line Chart)
    /// </summary>
    Time,

    /// <summary>
    /// Data is split by category (e.g. Pie/Bar Chart)
    /// </summary>
    Categorical
}
