using System.ComponentModel.DataAnnotations;

namespace Navigator.Data.Models.Journey;

public class JourneyOnDateRequest
{
    [MaxLength(73)]
    public required string Id { get; set; }
    public required DateTime FetchingDate { get; set; }
}
