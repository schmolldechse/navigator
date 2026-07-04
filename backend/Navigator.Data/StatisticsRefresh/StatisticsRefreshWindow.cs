namespace Navigator.Data.StatisticsRefresh;

public readonly record struct StatisticsRefreshWindow(DateTime Start, DateTime End)
{
    public void Validate()
    {
        if (Start.Kind != DateTimeKind.Utc) throw new ArgumentException("Window start must be UTC.", nameof(Start));
        if (End.Kind != DateTimeKind.Utc) throw new ArgumentException("Window end must be UTC.", nameof(End));
        if (End <= Start) throw new ArgumentException("Window end must be after start.", nameof(End));
    }
}
