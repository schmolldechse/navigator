using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableEntryAdministration
{
    [JsonPropertyName("administrationId")]
    public required string AdministrationId { get; set; }

    [JsonPropertyName("operatorCode")]
    public required string OperatorCode { get; set; }

    [JsonPropertyName("operatorName")]
    public required string OperatorName { get; set; }
}
