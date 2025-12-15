namespace Navigator.Data.Models.Journey;

public class JourneyOnDateRequest
{
    public required Guid Id { get; set; }
    public required DateTime FetchingDate { get; set; }
}
