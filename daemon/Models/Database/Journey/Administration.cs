using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database.Journey;

[Index(nameof(AdministrationId), nameof(OperatorCode), nameof(OperatorName), IsUnique = true, Name = "admin_admin_id_op_code_op_name_uidx")]
public class Administration
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [Column("administration_id")]
    [MaxLength(128)]
    public string? AdministrationId { get; init; }
    
    [Column("operator_code")]
    [MaxLength(64)]
    public string? OperatorCode { get; init; }
    
    [Column("operator_name")]
    [MaxLength(256)]
    public string? OperatorName { get; init; }
}