using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

public sealed class StatisticsRefreshQueueItem
{
    [Column("window_start")]
    public required DateTime WindowStart { get; set; }

    [Column("window_end")]
    public required DateTime WindowEnd { get; set; }

    [Column("status")]
    public required StatisticsRefreshQueueStatus Status { get; set; }

    [Column("source")]
    public required StatisticsRefreshQueueSource Source { get; set; }

    [Column("mark_count")]
    public required int MarkCount { get; set; }

    [Column("attempt")]
    public required int Attempt { get; set; }

    [Column("first_marked_at")]
    public required DateTime FirstMarkedAt { get; set; }

    [Column("last_marked_at")]
    public required DateTime LastMarkedAt { get; set; }

    [Column("started_at")]
    public DateTime? StartedAt { get; set; }

    [Column("finished_at")]
    public DateTime? FinishedAt { get; set; }

    [Column("error_kind")]
    public string? ErrorKind { get; set; }
}
