using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Subject;

public class AdministrationMetricSubject
{
    [MaxLength(32)]
    [JsonPropertyName("administrationId")]
    public required string AdministrationId { get; set; }

    [MaxLength(32)]
    [JsonPropertyName("operatorCode")]
    public required string OperatorCode { get; set; }

    [MaxLength(128)]
    [JsonPropertyName("operatorName")]
    public required string OperatorName { get; set; }
}
