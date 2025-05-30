using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public abstract class AbstractMessage
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("code")]
    public required int Code { get; set; }
    
    [Column("message")]
    [MaxLength(2048)]
    public required string Message { get; set; }
    
    [Column("summary")]
    [MaxLength(2048)]
    public required string Summary { get; set; }
}

public class JourneyMessage : AbstractMessage
{
    [Column("journey_id")]
    [MaxLength(45)] // 8 (date) + 1 (dash) + 36 (UUID) = 45
    public required string JourneyId { get; set; }

    [ForeignKey(nameof(JourneyId))] public virtual Journey Journey { get; set; } = null!;
}

public class StopMessage : AbstractMessage
{
    [Column("stop_id")]
    public required int StopId { get; set; }

    [ForeignKey(nameof(StopId))] public virtual Stop Stop { get; set; } = null!;
}