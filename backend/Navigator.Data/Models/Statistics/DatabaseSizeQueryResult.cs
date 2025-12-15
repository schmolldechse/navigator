using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Statistics;

public class DatabaseSizeQueryResult
{
    public long SizeInBytes { get; set; }
}
