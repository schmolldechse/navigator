using Navigator.Data.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Station;

public class Station : BaseStation
{
    [JsonPropertyName("transports")]
    public required TransportType[] Transports { get; set; }

    [JsonPropertyName("ril100")]
    public string[]? Ril100 { get; set; }
}
