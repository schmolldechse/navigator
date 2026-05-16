namespace Navigator.Data.Models.Statistics;

public class MetricPage
{
    public required int Offset { get; set; }
    public required int Limit { get; set; }
    public required bool HasMore { get; set; }
    public required int TotalItems { get; set; }
    public required int TotalPages { get; set; }

    public static MetricPage Create(int offset, int limit, int totalItems) => new()
    {
        Offset = offset,
        Limit = limit,
        HasMore = offset + limit < totalItems,
        TotalItems = totalItems,
        TotalPages = (int)Math.Ceiling((decimal)totalItems / limit)
    };
}
