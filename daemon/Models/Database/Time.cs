using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class Time
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("planned_time")]
    public required DateTime PlannedTime { get; set; }
    
    [Column("actual_time")]
    public required DateTime ActualTime { get; set; }
    
    [Column("delay")]
    public required int Delay { get; set; }
    
    [Column("planned_platform")]
    [MaxLength(32)]
    public required string PlannedPlatform { get; set; }
    
    [Column("actual_platform")]
    [MaxLength(32)]
    public required string ActualPlatform { get; set; }
}
