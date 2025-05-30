using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class LineInformation
{
    [Column("product_type")] [MaxLength(32)] public required string ProductType { get; set; }
    
    [Column("product_name")] [MaxLength(32)] public required string ProductName { get; set; }
    
    [Column("journey_number")] [MaxLength(32)] public required string JourneyNumber { get; set; }
    
    [Column("journey_name")] [MaxLength(32)] public required string JourneyName { get; set; }
}