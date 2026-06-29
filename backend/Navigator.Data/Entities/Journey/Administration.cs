using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

public class Administration
{
    [Column("id")]
    public virtual Guid Id { get; set; }

    [Column("administration_id")]
    public required string AdministrationId { get; set; }

    [Column("operator_code")]
    public required string OperatorCode { get; set; }

    [Column("operator_name")]
    public required string OperatorName { get; set; }
}
