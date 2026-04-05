using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

public class RisIdSnapshot
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("measured_at")]
    public required DateTime MeasuredAt { get; set; }

    [Column("active")]
    public required int Active { get; set; }

    [Column("inactive")]
    public required int Inactive { get; set; }
}
