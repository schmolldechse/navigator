using Navigator.Data.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Station;

public class StationGatheringInfoDTO
{
    [JsonPropertyName("queryingEnabled")]
    public required bool QueryingEnabled { get; set; }

    [JsonPropertyName("lastQueried")]
    public required DateTime LastQueried { get; set; }

    [JsonPropertyName("active")]
    public required TransportType[] ActiveTransportTypes { get; set; }

    [JsonPropertyName("disabled")]
    public required TransportType[] DisabledTransportTypes { get; set; }
}
