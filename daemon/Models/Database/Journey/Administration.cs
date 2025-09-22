using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database.Journey;

public class Administration
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [MaxLength(64)]
    [Column("administration_id")]
    public required string AdministrationId { get; init; }
    
    [MaxLength(64)]
    [Column("operator_code")]
    public required string OperatorCode { get; init; }
    
    [MaxLength(256)]
    [Column("operator_name")]
    public required string OperatorName { get; init; }
    
    public virtual ICollection<Journey> Journeys { get; set; } = new List<Journey>();
}