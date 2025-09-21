using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database.Journey;

public class Transport
{
    [Key] 
    [Column("journey_id")] 
    [MaxLength(128)]
    public string JourneyId { get; init; }
    
    [ForeignKey(nameof(JourneyId))]
    public virtual Journey Journey { get; init; }
    
    
    [Column("type")]
    public required TransportType Type { get; init; }
    
    [Column("replacement_type")]
    public TransportType? ReplacementType { get; init; }
    
    [Column("category")]
    [MaxLength(64)]
    public required string Category { get; init; }
    
    [Column("category_internal")]
    [MaxLength(64)]
    public required string CategoryInternal { get; init; }
    
    [Column("journey_description")]
    [MaxLength(128)]
    public required string JourneyDescription { get; init; }
    
    [Column("label")]
    [MaxLength(128)]
    public required string Label { get; init; }
    
    [Column("number")]
    public required int Number { get; init; }
    
    [Column("line")]
    [MaxLength(64)]
    public required string? Line { get; init; }
}