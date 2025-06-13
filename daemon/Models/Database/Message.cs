using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace daemon.Models.Database;

public abstract class AbstractMessage
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("code")]
    public required int Code { get; set; }
    
    [Column("message")]
    [MaxLength(2048)]
    public required string Message { get; set; }
    
    [Column("summary")]
    [MaxLength(2048)]
    public required string Summary { get; set; }
}

public class JourneyMessage : AbstractMessage
{
    [Column("journey_id")]
    [MaxLength(82)] // 8 (date) + 1 (dash) + 36 (UUID) [ +1 (dash) + 36 (UUID) ] = 45 | 82
    public string JourneyId { get; set; }

    [ForeignKey(nameof(JourneyId))] public virtual Journey Journey { get; set; } = null!;
    
    public static JourneyMessage CreateJourneyMessage(JsonElement element, string journeyId)
    {
        var code = -1;
        if (element.TryGetProperty("code", out var codeElement) && codeElement.ValueKind != JsonValueKind.Null) code = int.Parse(codeElement.ToString());
        else if (element.TryGetProperty("id", out var idElement) && idElement.ValueKind != JsonValueKind.Null) code = int.Parse(idElement.ToString());
        else throw new ArgumentNullException(nameof(element), $"Failed to parse the JourneyMessage code for {journeyId}");

        return new()
        {
            Code = code,
            Message = element.GetProperty("caption").GetString() ?? throw new ArgumentNullException(nameof(element),
                $"Failed to parse the JourneyMessage message for {journeyId}"),
            Summary = element.GetProperty("shortText").GetString() ?? throw new ArgumentNullException(nameof(element),
                $"Failed to parse the JourneyMessage summary for {journeyId}"),
        };
    }
}

public class StopMessage : AbstractMessage
{
    [Column("stop_id")]
    public int StopId { get; set; }

    [ForeignKey(nameof(StopId))] public virtual Stop Stop { get; set; } = null!;
    
    public static StopMessage CreateStopMessage(JsonElement element, string journeyId)
    {
        var code = -1;
        if (element.TryGetProperty("code", out var codeElement) && codeElement.ValueKind != JsonValueKind.Null) code = int.Parse(codeElement.ToString());
        else throw new ArgumentNullException(nameof(element), $"Failed to parse the StopMessage code for {journeyId}");

        var message = string.Empty;
        if (element.TryGetProperty("text", out var messageElement) && messageElement.ValueKind != JsonValueKind.Null)
            message = messageElement.GetString();

        var summary = string.Empty;
        if (element.TryGetProperty("textShort", out var summaryElement) && summaryElement.ValueKind != JsonValueKind.Null)
            summary = summaryElement.GetString();
        else summary = message;
        
        return new()
        {
            Code = code,
            Message = message,
            Summary = summary
        };
    }
}