using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database.Journey;

public class ScheduleAtStopPlace
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [Column("journey_id")] 
    [MaxLength(128)]
    public string JourneyId { get; init; }
    
    [ForeignKey(nameof(JourneyId))]
    public virtual Journey Journey { get; init; }
    
    
    [Column("date")]
    public required DateOnly Date { get; init; }
    
    [Column("type")]
    public required ScheduleType Type { get; init; }
    
    
    [Column("station_name")]
    public required string Name { get; init; }

    [Column("station_eva_number")]
    public required int EvaNumber { get; init; }


    [Column("cancelled")] 
    public required bool Cancelled { get; init; } = false;
    
    [Column("additional")]
    public required bool Additional { get; init; } = false;
    
    [Column("demand")]
    public required bool Demand { get; init; } = false;
    
    [Column("no_passenger_change")]
    public required bool NoPassengerChange { get; init; } = false;
    
    
    [Column("planned_time")]
    public required DateTime PlannedTime { get; init; }
    
    [Column("actual_time")]
    public required DateTime ActualTime { get; init; }
    
    [Column("delay")]
    public required int Delay { get; init; }
    
    [Column("planned_platform")]
    [MaxLength(32)]
    public required string PlannedPlatform { get; init; }
    
    [Column("actual_platform")]
    [MaxLength(32)]
    public required string ActualPlatform { get; init; }
    
    public virtual ICollection<Information> Information { get; init; } = new List<Information>();
}