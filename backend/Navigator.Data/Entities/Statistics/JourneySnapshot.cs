using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

public class JourneySnapshot
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("measured_at")]
    public required DateTime MeasuredAt { get; set; }

    [Column("total")]
    public required int Total { get; set; }
}
