using System.ComponentModel.DataAnnotations;
using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.Subject;

public class LineMetricSubject
{
    public required int Number { get; set; }

    [MaxLength(64)]
    public required string JourneyDescription { get; set; }

    public required TransportType TransportType { get; set; }
}
