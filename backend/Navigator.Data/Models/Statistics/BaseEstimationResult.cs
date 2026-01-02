namespace Navigator.Data.Models.Statistics;

public abstract class BaseEstimationResult<T>
{
    public abstract required int StartedWith { get; set; }
    
    public abstract required int Total { get; set; }
    
    public abstract required IEnumerable<T> Values { get; set; }
}