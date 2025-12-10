using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

[Table("journey_stop_place_informations", Schema = "core")]
[Index(nameof(InformationType))]
[Index(nameof(InformationKey))]
public class JourneyStopPlaceInformation
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("scheduled_stop_place_id")]
    public required Guid ScheduledStopPlaceId { get; set; }

    [ForeignKey(nameof(ScheduledStopPlaceId))]
    public virtual JourneyScheduledStopPlace? ScheduledStopPlace { get; set; }

    [Column("information_type")]
    public required InformationType InformationType { get; set; }

    [MaxLength(64)]
    [Column("key")]
    public required string InformationKey { get; set; }

    [MaxLength(2048)]
    [Column("text")]
    public required string Text { get; set; }

    [MaxLength(2048)]
    [Column("text_short")]
    public string? TextShort { get; set; }

    [Column("disruption_communication_id")]
    public Guid? DisruptionCommunicationId { get; set; }

    [Column("disruption_id")]
    public Guid? DisruptionId { get; set; }
}
