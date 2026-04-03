using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey.Message;

[Table("journey_message_references", Schema = "core")]
public class JourneyMessageReference
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("message_id")]
    public Guid MessageId { get; set; }

    [ForeignKey(nameof(MessageId))]
    public required JourneyMessage Message { get; set; }

    [Column("message_reference_type")]
    public required MessageReferenceType ReferenceType { get; set; }

    [Column("url")]
    public string? Url { get; set; }

    [Column("label")]
    public string? Label { get; set; }
}
