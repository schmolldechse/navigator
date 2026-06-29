using Navigator.Data.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Station;

public class StationGatheringInfo
{
    [JsonPropertyName("queryingEnabled")]
    public required bool QueryingEnabled { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("lastQueried")]
    public DateTimeOffset? LastQueried { get; set; }

    [JsonPropertyName("active")]
    public required TransportType[] ActiveTransportTypes { get; set; }

    [JsonPropertyName("disabled")]
    public required TransportType[] DisabledTransportTypes { get; set; }
}
