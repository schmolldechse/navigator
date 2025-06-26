using System.Text;
using System.Text.RegularExpressions;
using daemon.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace daemon.Database;

public class NavigatorDbContext : DbContext
{
	// stations
	public DbSet<Station> Stations { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Ril100> Ril100 { get; set; }

	// ris_ids
	public DbSet<IdentifiedRisId> RisIds { get; set; }

	// trips
	public DbSet<Journey> Journeys { get; set; }
	public DbSet<JourneyMessage> JourneyMessages { get; set; }
	public DbSet<Stop> ViaStops { get; set; }
	public DbSet<StopMessage> StopMessages { get; set; }

	private readonly Regex _uriRegex = new(
		@"^postgresql://(?:([^:]+)(?::([^@]+))?@)?([^:/]+)(?::(\d+))?(?:/([^?]+))?(?:\?(.*))?$"
	);

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("core");

		// stations
		modelBuilder.Entity<Station>(entity =>
		{
			entity.ToTable("stations");

			entity.HasKey(station => station.EvaNumber);
			entity.OwnsOne(station => station.Coordinates);
		});

		// station_products
		modelBuilder.Entity<Product>(entity =>
		{
			entity.ToTable("station_products");

			entity.HasKey(product => product.Id);
			entity
				.HasOne(product => product.Station)
				.WithMany(station => station.Products)
				.HasForeignKey(product => product.EvaNumber)
				.OnDelete(DeleteBehavior.Cascade);
		});

		// station_ril100
		modelBuilder.Entity<Ril100>(entity =>
		{
			entity.ToTable("station_ril100");

			entity.HasKey(ril => ril.Id);
			entity
				.HasOne(ril => ril.Station)
				.WithMany(station => station.Ril100)
				.HasForeignKey(ril => ril.EvaNumber)
				.OnDelete(DeleteBehavior.Cascade);
		});

		// ris_ids
		modelBuilder.Entity<IdentifiedRisId>(entity =>
		{
			entity.ToTable("ris_ids");

			entity.HasKey(risId => risId.Id);
		});

		// journeys
		modelBuilder.Entity<Journey>(entity =>
		{
			entity.ToTable("journeys");

			entity.HasKey(journey => journey.Id);
			entity.OwnsOne(journey => journey.LineInformation);
			entity.OwnsOne(journey => journey.Operator);

			// indexes
			entity.HasIndex(journey => journey.JourneyDate).HasDatabaseName("idx_journey_date");
		});

		// journey_messages
		modelBuilder.Entity<JourneyMessage>(entity =>
		{
			entity.ToTable("journey_messages");

			entity.HasKey(message => message.Id);
			entity
				.HasOne(message => message.Journey)
				.WithMany(journey => journey.Messages)
				.HasForeignKey(message => message.JourneyId)
				.OnDelete(DeleteBehavior.Cascade);

			// indexes
			entity.HasIndex(message => message.JourneyDate).HasDatabaseName("idx_messages_journeydate");
		});

		// journey_via-stops
		modelBuilder.Entity<Stop>(entity =>
		{
			entity.ToTable("journey_via-stops");
			entity.HasKey(stop => stop.Id);

			entity
				.HasOne(stop => stop.Journey)
				.WithMany(journey => journey.ViaStops)
				.HasForeignKey(stop => stop.JourneyId)
				.OnDelete(DeleteBehavior.Cascade);

			// flatten Arrival properties
			entity.OwnsOne(
				stop => stop.Arrival,
				arrival =>
				{
					arrival.Property(time => time.PlannedTime).HasColumnName("arrival_planned_time");
					arrival.Property(time => time.ActualTime).HasColumnName("arrival_actual_time");
					arrival.Property(time => time.Delay).HasColumnName("arrival_delay");
					arrival.Property(time => time.PlannedPlatform).HasColumnName("arrival_planned_platform");
					arrival.Property(time => time.ActualPlatform).HasColumnName("arrival_actual_platform");
				}
			);

			// flatten Departure properties
			entity.OwnsOne(
				stop => stop.Departure,
				departure =>
				{
					departure.Property(time => time.PlannedTime).HasColumnName("departure_planned_time");
					departure.Property(time => time.ActualTime).HasColumnName("departure_actual_time");
					departure.Property(time => time.Delay).HasColumnName("departure_delay");
					departure.Property(time => time.PlannedPlatform).HasColumnName("departure_planned_platform");
					departure.Property(time => time.ActualPlatform).HasColumnName("departure_actual_platform");
				}
			);

			// indexes
			entity.HasIndex(stop => stop.EvaNumber).HasDatabaseName("idx_via-stops_evanumber");

			entity.HasIndex(stop => stop.JourneyDate).HasDatabaseName("idx_via-stops_journeydate");

			entity
				.HasIndex(stop => new { stop.EvaNumber, stop.JourneyId })
				.HasDatabaseName("idx_via-stops_evanumber_journeyid");

			entity
				.HasIndex(stop => new { stop.EvaNumber, stop.JourneyDate })
				.HasDatabaseName("idx_via-stops_evanumber_journeydate");
		});

		// journey_stop_messages
		modelBuilder.Entity<StopMessage>(entity =>
		{
			entity.ToTable("journey_stop_messages");

			entity.HasKey(message => message.Id);
			entity
				.HasOne(message => message.Stop)
				.WithMany(stop => stop.Messages)
				.HasForeignKey(message => message.StopId)
				.OnDelete(DeleteBehavior.Cascade);
		});
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = ParseToEfConnectionString(
			Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
				?? throw new ArgumentNullException("POSTGRES_CONNECTION_STRING", "Postgres connection string not configured")
		);
		optionsBuilder.UseNpgsql(connectionString);
	}

	private string ParseToEfConnectionString(string connectionString)
	{
		var match = _uriRegex.Match(connectionString);
		if (!match.Success)
			throw new FormatException("Invalid PostgreSQL connection string");

		var user = match.Groups[1].Value;
		var password = match.Groups[2].Value;
		var host = match.Groups[3].Value;
		var port = match.Groups[4].Value ?? "5432"; // Default PostgreSQL port
		var database = match.Groups[5].Value;

		var builder = new StringBuilder();
		builder.Append($"Host={host};");
		if (int.TryParse(port, out _))
			builder.Append($"Port={port};");
		if (!string.IsNullOrEmpty(user))
			builder.Append($"Username={user};");
		if (!string.IsNullOrEmpty(password))
			builder.Append($"Password={password};");
		if (!string.IsNullOrEmpty(database))
			builder.Append($"Database={database};");

		// parse any additional parameters
		if (match.Groups[6].Success)
		{
			var query = match.Groups[6].Value;
			var queryParams = System.Web.HttpUtility.ParseQueryString(query);
			foreach (string key in queryParams)
			{
				if (string.IsNullOrEmpty(key))
					continue;
				builder.Append($"{key}={queryParams[key]};");
			}
		}
		return builder.ToString().TrimEnd(';');
	}
}
