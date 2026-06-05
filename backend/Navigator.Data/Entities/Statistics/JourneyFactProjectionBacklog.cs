using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

public class JourneyFactProjectionBacklog
{
    [Column("journey_id")]
    public required string JourneyId { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }

    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }

    [Column("available_at")]
    public required DateTime AvailableAt { get; set; }

    [Column("attempts")]
    public required int Attempts { get; set; }

    [Column("last_attempt_at")]
    public DateTime? LastAttemptAt { get; set; }

    [Column("locked_until")]
    public DateTime? LockedUntil { get; set; }

    [Column("locked_by")]
    public string? LockedBy { get; set; }

    [Column("last_error")]
    public string? LastError { get; set; }

    [Column("dead_lettered_at")]
    public DateTime? DeadLetteredAt { get; set; }
}
