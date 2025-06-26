using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class Coordinates
{
	[Column("latitude")]
	public required double Latitude { get; set; }

	[Column("longitude")]
	public required double Longitude { get; set; }
}
