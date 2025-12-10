using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

[Table("journeys", Schema = "core")]
[Index(nameof(Date))]
[Index(nameof(Cancelled))]
public class Journey
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required Guid Id { get; set; }

    [Column("date")]
    public required DateTime Date { get; set; }

    [Column("inserted_at")]
    public required DateTime InsertedAt { get; set; }

    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }

    [ForeignKey(nameof(AdministrationId))]
    public Administration? Administration { get; set; }

    [Column("cancelled")]
    public required bool Cancelled { get; set; }

    public required virtual JourneyTransport Transport { get; set; }

    public virtual ICollection<JourneyScheduledStopPlace> ScheduledStopPlaces { get; set; } = [];
}
