namespace Navigator.Data.Models.Station;

public class ShuffledStationRequest
{
    public required bool OnlyIncludeActive { get; set; } = true;
    public required DateTime LastSeen { get; set; } = DateTime.UtcNow;
}
