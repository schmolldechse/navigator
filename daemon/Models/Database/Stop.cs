using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace daemon.Models.Database;

public class Stop
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("journey_id")]
    [MaxLength(82)] // 8 (date) + 1 (dash) + 36 (UUID) [ +1 (dash) + 36 (UUID) ] = 45 | 82
    public string JourneyId { get; set; }

    [Column("journey_date")]
    public virtual DateOnly JourneyDate { 
        get => DateOnly.ParseExact(JourneyId.Substring(0, 8), "yyyyMMdd", null);
        private set { } 
    }

    [ForeignKey(nameof(JourneyId))] public virtual Journey Journey { get; set; } = null!;
    
    [Column("name")]
    [MaxLength(512)]
    public required string Name { get; set; }
    
    [Column("eva_number")]
    public required int EvaNumber { get; set; }
    
    [Column("cancelled")]
    public required bool Cancelled { get; set; }

    public virtual Time? Arrival { get; set; } = null!;

    public virtual Time? Departure { get; set; } = null!;

    public virtual ICollection<StopMessage> Messages { get; set; } = new List<StopMessage>();

    public static Stop CreateStop(JsonElement element, string journeyId)
    {
        var cancelled = false;
        if (element.TryGetProperty("status", out var statusElement) && statusElement.ValueKind != JsonValueKind.Null)
            cancelled = statusElement.GetString() == "Canceled";
        else if (element.TryGetProperty("canceled", out var canceledElement) && canceledElement.ValueKind != JsonValueKind.Null)
            cancelled = canceledElement.GetBoolean();
        else throw new ArgumentNullException(nameof(element), $"Failed to parse the Stop's cancelled status for {journeyId}");

        var stop = new Stop()
        {
            Cancelled = cancelled,
            EvaNumber = int.Parse(element.GetProperty("station").GetProperty("evaNo").GetString() ??
                                  throw new ArgumentNullException(nameof(element),
                                      $"Failed to parse the Stop's evaNumber for {journeyId}")),
            Name = element.GetProperty("station").GetProperty("name").GetString() ??
                   throw new ArgumentNullException(nameof(element), $"Failed to parse the Stop's name for {journeyId}"),
            Messages = element.GetProperty("messages").EnumerateArray()
                .Select(message => StopMessage.CreateStopMessage(message, journeyId)).ToList(),
        };

        if (!element.TryGetProperty("track", out var trackElement) || trackElement.ValueKind == JsonValueKind.Null)
            throw new ArgumentNullException(nameof(element), $"Failed to parse the Stop's track for {journeyId}");

        if (element.TryGetProperty("arrivalTime", out var arrivalTimeProp) &&
            arrivalTimeProp.ValueKind != JsonValueKind.Null)
            stop.Arrival = Time.CreateTime(arrivalTimeProp, trackElement);

        if (element.TryGetProperty("departureTime", out var departureTimeProp) &&
            departureTimeProp.ValueKind != JsonValueKind.Null)
            stop.Departure = Time.CreateTime(departureTimeProp, trackElement);

        return stop;
    }
}
