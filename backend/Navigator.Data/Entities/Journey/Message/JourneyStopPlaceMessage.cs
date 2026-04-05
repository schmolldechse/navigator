using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey.Message;

public class JourneyStopPlaceMessage
{
    [Column("journey_stop_place_id")]
    public Guid StopPlaceId { get; set; }

    public virtual JourneyStopPlace? StopPlace { get; set; }

    [Column("journey_message_id")]
    public Guid MessageId { get; set; }

    public virtual JourneyMessage? Message { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }
}
