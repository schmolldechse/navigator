﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database;

[Index(nameof(Id), IsUnique = true)]
public class Journey
{
	[Key]
	[Column("journey_id")]
	[MaxLength(82)] // 8 (date) + 1 (dash) + 36 (UUID) [ +1 (dash) + 36 (UUID) ] = 45 | 82
	public required string Id { get; set; }

	[Column("journey_date")]
	public virtual DateOnly JourneyDate
	{
		get => DateOnly.ParseExact(Id.Substring(0, 8), "yyyyMMdd", null);
		private set { }
	}

	public virtual LineInformation LineInformation { get; set; } = null!;

	public virtual Operator Operator { get; set; } = null!;

	[NotMapped]
	public virtual Stop? Origin => ViaStops?.FirstOrDefault();

	[NotMapped]
	public virtual Stop? Destination => ViaStops?.LastOrDefault();

	public virtual ICollection<Stop> ViaStops { get; set; } = new List<Stop>();

	public virtual ICollection<JourneyMessage> Messages { get; set; } = new List<JourneyMessage>();
}
