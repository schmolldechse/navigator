using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace daemon.Models.Database;

public class Operator
{
	[Column("operator_code")]
	[MaxLength(128)]
	public required string Code { get; set; }

	[Column("operator_name")]
	[MaxLength(512)]
	public required string Name { get; set; }

	public static Operator CreateOperator(JsonElement element, string journeyId) =>
		new()
		{
			Code =
				element.GetProperty("operatorCode").GetString()
				?? throw new ArgumentNullException(nameof(element), $"Failed to parse operator code for {journeyId}"),
			Name =
				element.GetProperty("operatorName").GetString()
				?? throw new ArgumentNullException(nameof(element), $"Failed to parse operator name for {journeyId}"),
		};
}
