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
    public DbSet<Time> StopTime { get; set; }
    public DbSet<StopMessage> StopMessages { get; set; }
    
    private readonly Regex _uriRegex =
        new(@"^postgresql://(?:([^:]+)(?::([^@]+))?@)?([^:/]+)(?::(\d+))?(?:/([^?]+))?(?:\?(.*))?$");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var schema = Environment.GetEnvironmentVariable("POSTGRES_CORE_SCHEMA") ?? throw new ArgumentNullException("POSTGRES_CORE_SCHEMA", "Core schema not configured");
        modelBuilder.HasDefaultSchema(schema);
        
        // stations
        modelBuilder.Entity<Station>(entity =>
        {
            entity.ToTable("stations");
            entity.HasKey(e => e.EvaNumber);
            entity.OwnsOne(e => e.Coordinates);
        });
        
        // station_products
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("station_products");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Station)
                .WithMany(s => s.Products)
                .HasForeignKey(e => e.EvaNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // station_ril100
        modelBuilder.Entity<Ril100>(entity =>
        {
            entity.ToTable("station_ril100");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Station)
                .WithMany(s => s.Ril100)
                .HasForeignKey(e => e.EvaNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // ris_ids
        modelBuilder.Entity<IdentifiedRisId>(entity =>
        {
            entity.ToTable("ris_ids");
            entity.HasKey(e => e.Id);
        });
        
        // journeys
        modelBuilder.Entity<Journey>(entity =>
        {
            entity.ToTable("journeys");
            entity.HasKey(e => e.Id); 
            entity.ToTable(t => t.HasCheckConstraint("CK_Trip_JourneyId_Format", 
               "journey_id ~ '^\\d{8}-[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$'"));
            
            entity.OwnsOne(j => j.LineInformation);
            entity.OwnsOne(j => j.Operator);
        });
            
        // journey_messages
        modelBuilder.Entity<JourneyMessage>(entity =>
        {
            entity.ToTable("journey_messages");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Journey)
                .WithMany(j => j.Messages)
                .HasForeignKey(m => m.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // journey_via-stops
        modelBuilder.Entity<Stop>(entity =>
        {
            entity.ToTable("journey_via-stops");
            entity.HasKey(e => e.Id);

            entity.HasOne(s => s.Arrival)
                .WithMany()
                .HasForeignKey("ArrivalId")
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(s => s.Departure)
                .WithMany()
                .HasForeignKey("DepartureId")
                .OnDelete(DeleteBehavior.SetNull);
            
            // ensure unique EvaNumber per Journey
            entity.HasIndex(e => new { e.JourneyId, e.EvaNumber }).IsUnique();
        });
        
        // journey_stop_messages
        modelBuilder.Entity<StopMessage>(entity =>
        {
            entity.ToTable("journey_stop_messages");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Stop)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.StopId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Time>(entity =>
        {
            entity.ToTable("journey_stop_times");
            entity.HasKey(e => e.Id);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = ParseToEfConnectionString(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING") ??
                                                        throw new ArgumentNullException("POSTGRES_CONNECTION_STRING",
                                                            "Postgres connection string not configured"));
        optionsBuilder.UseNpgsql(connectionString);
    }

    private string ParseToEfConnectionString(string connectionString)
    {
        var match = _uriRegex.Match(connectionString);
        if (!match.Success) throw new FormatException("Invalid PostgreSQL connection string");
        
        var user = match.Groups[1].Value;
        var password = match.Groups[2].Value;
        var host = match.Groups[3].Value;
        var port = match.Groups[4].Value ?? "5432"; // Default PostgreSQL port
        var database = match.Groups[5].Value;

        var builder = new StringBuilder();
        builder.Append($"Host={host};");
        if (int.TryParse(port, out _)) builder.Append($"Port={port};");
        if (!string.IsNullOrEmpty(user)) builder.Append($"Username={user};");
        if (!string.IsNullOrEmpty(password)) builder.Append($"Password={password};");
        if (!string.IsNullOrEmpty(database)) builder.Append($"Database={database};");
        
        // parse any additional parameters
        if (match.Groups[6].Success)
        {
            var query = match.Groups[6].Value;
            var queryParams = System.Web.HttpUtility.ParseQueryString(query);
            foreach (string key in queryParams)
            {
                if (string.IsNullOrEmpty(key)) continue;
                builder.Append($"{key}={queryParams[key]};");
            }
        }
        return builder.ToString().TrimEnd(';');
    }
}
