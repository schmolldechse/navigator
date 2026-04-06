using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

public class JourneyTransport
{
    [Column("journey_id")]
    public required string JourneyId { get; set; }

    public virtual Journey? Journey { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("replacement_transport_type")]
    public TransportType? ReplacementTransportType { get; set; }

    [Column("category")]
    public required string Category { get; set; }

    [Column("category_internal")]
    public required string CategoryInternal { get; set; }

    [Column("journey_description")]
    public required string JourneyDescription { get; set; }

    [Column("label")]
    public required string Label { get; set; }

    [Column("line")]
    public string? Line { get; set; }

    [Column("number")]
    public required int Number { get; set; }
}
