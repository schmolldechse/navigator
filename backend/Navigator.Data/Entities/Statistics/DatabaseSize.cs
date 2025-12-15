using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

[Table("database_size", Schema = "statistics")]
[Index(nameof(MeasuredAt))]
public class DatabaseSize
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Column("measured_at")]
    public required DateTime MeasuredAt { get; set; }

    [Column("size_bytes")]
    public required long SizeBytes { get; set; }
}