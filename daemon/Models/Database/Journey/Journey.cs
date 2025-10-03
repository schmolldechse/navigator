using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database.Journey;

public class Journey
{
    [Key]
    [MaxLength(128)]
    [Column("journey_id")]
    public required string Id { get; init; }

    [Column("date")]
    public required DateOnly Date { get; init; }

    [Column("inserted_at")] 
    public required DateTime InsertedAt { get; init; }

    [Column("type")] 
    public required JourneyType Type { get; init; } = JourneyType.REGULAR;
    
    
    [Column("administration_index")]
    public int AdministrationIndex { get; init; }

    [ForeignKey(nameof(AdministrationIndex))] 
    public required Administration Administration { get; init; }
    
    
    [Column("cancelled")]
    public required bool Cancelled { get; init; } = false;
    
    
    public virtual Transport? Transport { get; init; }
    
    public virtual ICollection<ScheduleAtStopPlace> ViaStops { get; init; } = new List<ScheduleAtStopPlace>();
}
