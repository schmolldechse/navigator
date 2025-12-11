using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

[Table("database_size", Schema = "statistics")]
public class DatabaseSize
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Column("measured_at")]
    public required DateTimeOffset MeasuredAt { get; set; }

    [Column("size_bytes")]
    public required long SizeBytes { get; set; }
}