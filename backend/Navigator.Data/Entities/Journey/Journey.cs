using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

public class Journey
{
    [Column("id")]
    public required string Id { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }

    [Column("inserted_at")]
    public required DateTime InsertedAt { get; set; }

    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }

    public virtual Administration? Administration { get; set; }

    [Column("cancelled")]
    public required bool Cancelled { get; set; }

    [Column("journey_type")]
    public required JourneyType JourneyType { get; set; }

    public required virtual JourneyTransport Transport { get; set; }

    public virtual ICollection<JourneyStopPlace> StopPlaces { get; set; } = [];

}
