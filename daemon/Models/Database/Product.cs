using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class Product
{
	[Key]
	[Column("id")]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[Column("eva_number")]
	public int EvaNumber { get; set; }

	[ForeignKey(nameof(EvaNumber))]
	public virtual Station Station { get; set; } = null!;

	[Column("name")]
	[MaxLength(32)]
	public required string ProductName { get; set; }

	[Column("querying_enabled")]
	public bool QueryingEnabled { get; set; }

	private static readonly Dictionary<string, string[]> ProductDict = new()
	{
		{ "Hochgeschwindigkeitszuege", new[] { "HIGH_SPEED_TRAIN", "nationalExpress" } },
		{ "IntercityUndEurocityZuege", new[] { "INTERCITY_TRAIN", "national" } },
		{ "InterregioUndSchnellzuege", new[] { "INTER_REGIONAL_TRAIN", "regionalExpress" } },
		{ "NahverkehrsonstigeZuege", new[] { "REGIONAL_TRAIN", "regional" } },
		{ "Sbahnen", new[] { "CITY_TRAIN", "suburban" } },
		{ "Busse", new[] { "BUS" } },
		{ "Schiffe", new[] { "FERRY" } },
		{ "UBahn", new[] { "SUBWAY" } },
		{ "Strassenbahn", new[] { "TRAM" } },
		{ "AnrufpflichtigeVerkehre", new[] { "SHUTTLE", "taxi" } },
		{ "Unknown", new string[] { } },
	};

	public static string MapProduct(string? transport)
	{
		if (string.IsNullOrEmpty(transport))
			return "Unknown";
		foreach (var pair in ProductDict)
		{
			if (pair.Value.Any(value => string.Equals(value, transport, StringComparison.OrdinalIgnoreCase)))
				return pair.Key;
		}
		return "Unknown";
	}
}
