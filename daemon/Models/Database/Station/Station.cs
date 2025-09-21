using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database.Station;

[Index(nameof(EvaNumber), IsUnique = true)]
public class Station
{
	[Key]
	[Column("eva_number")]
	public required int EvaNumber { get; set; }

	[MaxLength(512)]
	[Column("name")]
	public required string Name { get; set; }

	[Column("weight")]
	public double Weight { get; set; } = 0;
	
	[Column("latitude")]
	public required double Latitude { get; set; }

	[Column("longitude")]
	public required double Longitude { get; set; }
	
	[Column("querying_enabled")]
	public bool QueryingEnabled { get; set; } = false;

	[Column("last_queried")]
	public DateTime? LastQueried { get; set; }

	[Column("is_locked")]
	public bool IsLocked { get; set; } = false;

	public virtual ICollection<Ril100> Ril100 { get; set; } = new List<Ril100>();

	public virtual ICollection<TransportOccurence> Products { get; set; } = new List<TransportOccurence>();
}
