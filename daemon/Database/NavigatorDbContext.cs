using System.Text.RegularExpressions;
using daemon.Models.Database.Journey;
using daemon.Models.Database.RISIdentifier;
using daemon.Models.Database.Station;
using Microsoft.EntityFrameworkCore;

namespace daemon.Database;

public class NavigatorDbContext : DbContext
{
	// stations
	public DbSet<Station> Stations { get; set; }
	public DbSet<TransportOccurence> Products { get; set; }
	public DbSet<Ril100> Ril100 { get; set; }

	// ris_ids
	public DbSet<IdentifiedRisId> RisIds { get; set; }

	// journeys
	public DbSet<Journey> Journeys { get; set; }
	public DbSet<Administration> Administrations { get; set; }
	public DbSet<Transport> Transports { get; set; }
	public DbSet<ScheduleAtStopPlace> ScheduledStopPlaces { get; set; }
	public DbSet<Information> Informations { get; set; }

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
		});

		// station_products
		modelBuilder.Entity<TransportOccurence>(entity =>
		{
			entity.ToTable("station_transports");
			entity
				.HasOne(product => product.Station)
				.WithMany(station => station.Products)
				.HasForeignKey(product => product.EvaNumber)
				.OnDelete(DeleteBehavior.Cascade);
			entity.Property(product => product.TransportType).HasConversion<string>();
		});

		// station_ril100
		modelBuilder.Entity<Ril100>(entity =>
		{
			entity.ToTable("station_ril100");
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
			entity.Property(risId => risId.TransportProduct).HasConversion<string>();
			entity.Property(risId => risId.ReplacementTransportProduct).HasConversion<string>();
		});
		
		// journeys
		modelBuilder.Entity<Journey>(entity =>
		{
			entity.ToTable("journeys");
			entity.Property(journey => journey.Type).HasConversion<string>();
			
			entity.HasOne(journey => journey.Administration)
				.WithMany(administration => administration.Journeys)
				.HasForeignKey(journey => journey.AdministrationIndex);

			entity.HasOne(journey => journey.Transport)
				.WithOne(transport => transport.Journey)
				.HasForeignKey<Transport>(transport => transport.JourneyId);

			entity.HasMany(journey => journey.ViaStops)
				.WithOne(stop => stop.Journey)
				.HasForeignKey(stop => stop.JourneyId);
		});
		
		// journey_administrations
		modelBuilder.Entity<Administration>(entity =>
		{
			entity.ToTable("journey_administrations");
		});
		
		// journey_transports
		modelBuilder.Entity<Transport>(entity =>
		{
			entity.ToTable("journey_transports");
			entity.Property(transport => transport.Type).HasConversion<string>();
			entity.Property(transport => transport.ReplacementType).HasConversion<string>();
		});
		
		// journey_scheduled_stop_places
		modelBuilder.Entity<ScheduleAtStopPlace>(entity =>
		{
			entity.ToTable("journey_scheduled_stop_places");
			entity.Property(stop => stop.Type).HasConversion<string>();

			entity.HasMany(stop => stop.Information)
				.WithOne(info => info.ScheduledStopPlace)
				.HasForeignKey(info => info.ScheduleAtStopPlaceId);
		});
		
		// journey_stop_place_informations
		modelBuilder.Entity<Information>(entity =>
		{
			entity.ToTable("journey_stop_place_informations");
			entity.Property(info => info.Type).HasConversion<string>();
		});
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING") ?? throw new ArgumentNullException("POSTGRES_CONNECTION_STRING environment variable is not set.");
		optionsBuilder.UseNpgsql(connectionString);
	}
}
