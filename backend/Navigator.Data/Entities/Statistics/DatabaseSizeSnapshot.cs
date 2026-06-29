using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

public class DatabaseSizeSnapshot
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("measured_at")]
    public required DateTime MeasuredAt { get; set; }

    [Column("size_in_bytes")]
    public required long SizeInBytes { get; set; }
}
