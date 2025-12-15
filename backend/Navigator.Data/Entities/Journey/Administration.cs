using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

[Table("journey_administrations", Schema = "core")]
[Index(nameof(AdministrationId), nameof(OperatorCode), nameof(OperatorName), IsUnique = true)]
public class Administration
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(32)]
    [Column("administration_id")]
    public required string AdministrationId { get; set; }

    [MaxLength(32)]
    [Column("operator_code")]
    public required string OperatorCode { get; set; }

    [MaxLength(128)]
    [Column("operator_name")]
    public required string OperatorName { get; set; }
}
