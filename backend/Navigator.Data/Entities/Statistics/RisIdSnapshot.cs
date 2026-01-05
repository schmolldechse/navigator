using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

[Table("risid_snapshot", Schema = "statistics")]
[Index(nameof(MeasuredAt))]
public class RisIdSnapshot
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("measured_at")]
    public required DateTime MeasuredAt { get; set; }

    [Column("active")]
    public required int Active { get; set; }

    [Column("inactive")]
    public required int Inactive { get; set; }
}
