using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

public sealed class StatisticsRefreshProgress
{
    [Column("operation")]
    public required string Operation { get; set; }

    [Column("window_start")]
    public required DateTime WindowStart { get; set; }

    [Column("window_end")]
    public required DateTime WindowEnd { get; set; }

    [Column("status")]
    public required string Status { get; set; }

    [Column("attempt")]
    public required int Attempt { get; set; }

    [Column("event_rows_affected")]
    public required long EventRowsAffected { get; set; }

    [Column("journey_rows_affected")]
    public required long JourneyRowsAffected { get; set; }

    [Column("cagg_refresh_count")]
    public required int CaggRefreshCount { get; set; }

    [Column("started_at")]
    public required DateTime StartedAt { get; set; }

    [Column("finished_at")]
    public DateTime? FinishedAt { get; set; }

    [Column("error_kind")]
    public string? ErrorKind { get; set; }
}
