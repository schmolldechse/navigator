using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Journey.Message;
using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

[Table("journeys", Schema = "core")]
[Index(nameof(Date))]
[Index(nameof(InsertedAt))]
[Index(nameof(JourneyType))]
public class Journey
{
    [Key]
    [Column("id")]
    [MaxLength(82)]
    public required string Id { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }

    [Column("inserted_at")]
    public required DateTime InsertedAt { get; set; }

    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }

    [ForeignKey(nameof(AdministrationId))]
    public Administration? Administration { get; set; }

    [Column("cancelled")]
    public required bool Cancelled { get; set; }

    [Column("journey_type")]
    public required JourneyType JourneyType { get; set; }

    public required virtual JourneyTransport Transport { get; set; }

    public virtual ICollection<JourneyStopPlace> StopPlaces { get; set; } = [];

    public virtual ICollection<JourneyMessage> Messages { get; set; } = [];
}
