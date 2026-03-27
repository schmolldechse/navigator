using Navigator.Data.Enums;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

public class TransportTypeDistributionMetricRequest : BaseMetricRequest
{
    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("transportTypes")]
    [Description("Optional list of transport types to filter the metric by. If not provided, all transport types will be included.")]
    public TransportType[]? TransportTypes { get; set; }

    public override Navigator.Data.Models.Statistics.BaseMetricRequest BuildRequest() => new Navigator.Data.Models.Statistics.Request.TransportTypeDistributionMetricRequest()
    {
        End = End,
        TransportTypes = TransportTypes ?? Array.Empty<TransportType>()
    };
}
