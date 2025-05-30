using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class Operator
{
    [Column("operator_code")]
    [MaxLength(128)]
    public required string Code { get; set; }
    
    [Column("operator_name")]
    [MaxLength(512)]
    public required string Name { get; set; }
}