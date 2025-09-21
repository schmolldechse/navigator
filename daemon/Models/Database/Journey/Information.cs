using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database.Journey;

public class Information
{
    [Key]
    [Column("id")]
    public int Id { get; init; }
    
    
    [Column("scheduled_stop_place_id")] 
    public int ScheduleAtStopPlaceId { get; init; }
    
    [ForeignKey(nameof(ScheduleAtStopPlaceId))]
    public virtual ScheduleAtStopPlace ScheduleAtStopPlace { get; init; }
    
    
    [Column("type")]
    public required InformationType Type { get; init; }
    
    [Column("key")]
    [MaxLength(64)]
    public required string Key { get; init; }
    
    [Column("text")]
    [MaxLength(2048)]
    public required string Text { get; init; }
    
    [Column("text_short")]
    [MaxLength(2048)]
    public string? TextShort { get; init; }
    
    [Column("disruption_communication_id")]
    public string? DisruptionCommunicationId { get; init; }
    
    [Column("disruption_id")]
    public string? DisruptionId { get; init; }

    public Information Clone()
    {
        return (Information)MemberwiseClone();
    }
}