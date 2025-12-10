using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

[Table("journey_transports", Schema = "core")]
[Index(nameof(TransportType))]
[Index(nameof(ReplacementTransportType))]
[Index(nameof(Category))]
[Index(nameof(JourneyDescription))]
[Index(nameof(Number))]
public class JourneyTransport
{
    [Key]
    [Column("journey_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required Guid Id { get; set; }

    [ForeignKey(nameof(Id))]
    public Journey? Journey { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("replacement_transport_type")]
    public TransportType? ReplacementTransportType { get; set; }

    [MaxLength(64)]
    [Column("category")]
    public required string Category { get; set; }

    [MaxLength(64)]
    [Column("category_internal")]
    public required string CategoryInternal { get; set; }

    [MaxLength(64)]
    [Column("journey_description")]
    public required string JourneyDescription { get; set; }

    [MaxLength(64)]
    [Column("label")]
    public required string Label { get; set; }

    [Column("line")]
    public string? Line { get; set; }

    [Column("number")]
    public required int Number { get; set; }
}
