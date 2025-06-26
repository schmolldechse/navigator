using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace daemon.Models.Database;

public class LineInformation
{
	[Column("product_type")]
	[MaxLength(32)]
	public required string ProductType { get; set; }

	[Column("product_name")]
	[MaxLength(32)]
	public required string ProductName { get; set; }

	[Column("journey_number")]
	[MaxLength(32)]
	public required string JourneyNumber { get; set; }

	[Column("journey_name")]
	[MaxLength(32)]
	public required string JourneyName { get; set; }

	public static LineInformation CreateLineInformation(JsonElement element, string journeyId) =>
		new()
		{
			ProductType = Product.MapProduct(
				element.GetProperty("type").GetString()
					?? throw new ArgumentNullException(nameof(element), $"Failed to parse productType for {journeyId}")
			),
			ProductName =
				element.GetProperty("category").GetString()
				?? throw new ArgumentNullException(nameof(element), $"Failed to parse productName for {journeyId}"),
			JourneyName =
				element.GetProperty("name").GetString()
				?? throw new ArgumentNullException(nameof(element), $"Failed to parse journeyName for {journeyId}"),
			JourneyNumber =
				element.GetProperty("no").ToString()
				?? throw new ArgumentNullException(nameof(element), $"Failed to parse journeyNumber for {journeyId}"),
		};
}
