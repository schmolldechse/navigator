namespace Navigator.Data.Models.Statistics;

public class BaseSnapshot<T>
{
    public required decimal StartedWith { get; set; }

    public required decimal Total { get; set; }

    public required IEnumerable<T> Values { get; set; }
}