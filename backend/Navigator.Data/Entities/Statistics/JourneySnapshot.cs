using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

[Table("journey_snapshot", Schema = "statistics")]
[Index(nameof(MeasuredAt))]
public class JourneySnapshot
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("measured_at")]
    public required DateTime MeasuredAt { get; set; }

    [Column("total")]
    public required int Total { get; set; }
}
