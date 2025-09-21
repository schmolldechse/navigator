using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database.Journey;

[Index(nameof(Id), IsUnique = true)]
public class Journey
{
    [Key]
    [Column("journey_id")]
    [MaxLength(128)]
    public required string Id { get; init; }

    [Column("date")]
    public virtual DateOnly Date => DateOnly.ParseExact(Id[..8], "yyyyMMdd", null);
    
    [Column("inserted_at")]
    public virtual DateTime InsertedAt { get; init; } = DateTime.UtcNow;

    [Column("type")] 
    public required JourneyType Type { get; init; } = JourneyType.REGULAR;
    
    
    [Column("administration_index")]
    public int AdministrationIndex { get; init; }

    [ForeignKey(nameof(AdministrationIndex))] 
    public required Administration Administration { get; init; }
    
    
    public virtual required Transport Transport { get; init; }
    
    public virtual ICollection<ScheduleAtStopPlace> ViaStops { get; init; } = new List<ScheduleAtStopPlace>();
}
