using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey.Message;

public class JourneyMessage
{
    [Column("id")]
    public virtual Guid Id { get; set; }

    [Column("journey_id")]
    public required string JourneyId { get; set; }

    public virtual Journey? Journey { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }

    [Column("message_type")]
    public required MessageType Type { get; set; }

    [Column("code")]
    public string? Code { get; set; }

    /// <summary>
    /// The main content (De).
    /// </summary>
    [Column("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Short text content (De). Only populated for Notes and Disruptions.
    /// </summary>
    [Column("text_short")]
    public string? TextShort { get; set; }

    /// --------------------------------------------------------------
    /// Disruption
    /// --------------------------------------------------------------

    /// <summary>
    /// Only applicable when Type = DISRUPTION
    /// </summary>
    [Column("disruption_cause")]
    public string? DisruptionCause { get; set; }

    /// <summary>
    /// Only applicable when Type = DISRUPTION
    /// </summary>
    [Column("disruption_effect")]
    public string? DisruptionEffect { get; set; }

    /// --------------------------------------------------------------
    /// Note
    /// --------------------------------------------------------------

    /// <summary>
    /// Only applicable when Type = Note
    /// </summary>
    [Column("note_category")]
    public string? NoteCategory { get; set; }

    public required virtual ICollection<JourneyStopPlaceMessage> JourneyStopPlaceMessages { get; set; } = [];
}
