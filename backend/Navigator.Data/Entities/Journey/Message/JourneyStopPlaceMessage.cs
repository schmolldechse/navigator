using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey.Message;

[Table("journey_stop_place_messages", Schema = "core")]
[PrimaryKey(nameof(StopPlaceId), nameof(MessageId))]
[Index(nameof(MessageId))]
[Index(nameof(StopPlaceId))]
public class JourneyStopPlaceMessage
{
    [Column("journey_stop_place_id")]
    public Guid StopPlaceId { get; set; }

    [ForeignKey(nameof(StopPlaceId))]
    public virtual JourneyStopPlace? StopPlace { get; set; }

    [Column("journey_message_id")]
    public Guid MessageId { get; set; }

    [ForeignKey(nameof(MessageId))]
    public virtual JourneyMessage? Message { get; set; }
}
