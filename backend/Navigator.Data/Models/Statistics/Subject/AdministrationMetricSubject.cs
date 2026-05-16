using System.ComponentModel.DataAnnotations;

namespace Navigator.Data.Models.Statistics.Subject;

public class AdministrationMetricSubject
{
    [MaxLength(32)]
    public required string AdministrationId { get; set; }

    [MaxLength(32)]
    public required string OperatorCode { get; set; }

    [MaxLength(128)]
    public required string OperatorName { get; set; }
}
