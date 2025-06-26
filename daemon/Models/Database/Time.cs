using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json;

namespace daemon.Models.Database;

public class Time
{
	[Key]
	[Column("id")]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[Column("planned_time")]
	public required DateTime PlannedTime { get; set; }

	[Column("actual_time")]
	public required DateTime ActualTime { get; set; }

	[Column("delay")]
	public required int Delay { get; set; }

	[Column("planned_platform")]
	[MaxLength(32)]
	public required string PlannedPlatform { get; set; }

	[Column("actual_platform")]
	[MaxLength(32)]
	public required string ActualPlatform { get; set; }

	public static Time CreateTime(JsonElement timeElement, JsonElement trackElement)
	{
		var plannedTime = DateTime.Parse(
			timeElement.GetProperty("target").GetString() ?? string.Empty,
			null,
			DateTimeStyles.AdjustToUniversal
		);
		if (
			!DateTime.TryParse(
				timeElement.GetProperty("predicted").GetString() ?? string.Empty,
				null,
				DateTimeStyles.AdjustToUniversal,
				out var actualTime
			)
		)
			actualTime = plannedTime;

		var plannedPlatform = string.Empty;
		if (
			trackElement.TryGetProperty("prediction", out var plannedPlatformElement)
			&& plannedPlatformElement.ValueKind != JsonValueKind.Null
		)
			plannedPlatform = plannedPlatformElement.GetString() ?? string.Empty;

		string actualPlatform = string.Empty;
		if (
			trackElement.TryGetProperty("target", out var actualPlatformElement)
			&& actualPlatformElement.ValueKind != JsonValueKind.Null
		)
			actualPlatform = actualPlatformElement.GetString() ?? string.Empty;
		else
			actualPlatform = plannedPlatform;

		return new()
		{
			PlannedTime = plannedTime,
			ActualTime = actualTime,
			Delay = (int)(actualTime - plannedTime).TotalSeconds,
			PlannedPlatform = plannedPlatform,
			ActualPlatform = actualPlatform,
		};
	}
}
