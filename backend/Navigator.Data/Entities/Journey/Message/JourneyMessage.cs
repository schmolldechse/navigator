using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey.Message;

[Table("journey_messages", Schema = "core")]
public class JourneyMessage
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("journey_id")]
    [MaxLength(82)]
    public required string JourneyId { get; set; }

    [ForeignKey(nameof(JourneyId))]
    public virtual Journey? Journey { get; set; }

    [Column("message_type")]
    public required MessageType Type { get; set; }

    [Column("code")]
    [MaxLength(64)]
    public string? Code { get; set; }

    /// <summary>
    /// The main content (De).
    /// </summary>
    [Column("text")]
    [MaxLength(2048)]
    public string? Text { get; set; }

    /// <summary>
    /// Short text content (De). Only populated for Notes and Disruptions.
    /// </summary>
    [Column("text_short")]
    [MaxLength(2048)]
    public string? TextShort { get; set; }

    /// --------------------------------------------------------------
    /// Disruption
    /// --------------------------------------------------------------

    /// <summary>
    /// Only applicable when Type = DISRUPTION
    /// </summary>
    [Column("disruption_cause")]
    [MaxLength(128)]
    public string? DisruptionCause { get; set; }

    /// <summary>
    /// Only applicable when Type = DISRUPTION
    /// </summary>
    [Column("disruption_effect")]
    [MaxLength(128)]
    public string? DisruptionEffect { get; set; }

    /// --------------------------------------------------------------
    /// Note
    /// --------------------------------------------------------------

    /// <summary>
    /// Only applicable when Type = Note
    /// </summary>
    [Column("note_category")]
    [MaxLength(128)]
    public string? NoteCategory { get; set; }

    public virtual ICollection<JourneyMessageReference> References { get; set; } = [];

    public virtual ICollection<JourneyStopPlaceMessage> JourneyStopPlaceMessages { get; set; } = [];
}
