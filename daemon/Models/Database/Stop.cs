using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class Stop
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("journey_id")]
    [MaxLength(45)] // 8 (date) + 1 (dash) + 36 (UUID) = 45
    public required string JourneyId { get; set; }

    [ForeignKey(nameof(JourneyId))] public virtual Journey Journey { get; set; } = null!;
    
    [Column("name")]
    [MaxLength(512)]
    public required string Name { get; set; }
    
    [Column("eva_number")]
    public required int EvaNumber { get; set; }
    
    [Column("cancelled")]
    public required bool Cancelled { get; set; }
    
    [Column("arrival")]
    protected int? ArrivalId { get; set; }

    [ForeignKey(nameof(ArrivalId))] public virtual Time? Arrival { get; set; } = null!;

    [Column("departure")]
    protected int? DepartureId { get; set; }
    
    [ForeignKey(nameof(DepartureId))]
    public virtual Time? Departure { get; set; } = null!;

    public virtual ICollection<StopMessage> Messages { get; set; } = new List<StopMessage>();
}
