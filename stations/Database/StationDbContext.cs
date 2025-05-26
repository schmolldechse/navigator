using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using stations.Models.Database;

namespace stations.Database;

public class StationDbContext : DbContext
{
    public DbSet<Station> Stations { get; set; }
    public DbSet<Coordinates> Coordinates { get; set; }

    private readonly Regex _uriRegex =
        new(@"^postgresql://(?:([^:]+)(?::([^@]+))?@)?([^:/]+)(?::(\d+))?(?:/([^?]+))?(?:\?(.*))?$");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("data");
        modelBuilder.Entity<Station>(entity =>
        {
            entity.ToTable("stations");
            entity.HasKey(e => e.EvaNumber);
            entity.Property(e => e.Ril100).HasColumnType("text[]");
            entity.Property(e => e.Products).HasColumnType("text[]");
        });
        modelBuilder.Entity<Coordinates>(entity =>
        {
            entity.ToTable("station_coordinates");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EvaNumber).IsUnique();
            entity.Property(e => e.Latitude).IsRequired();
            entity.Property(e => e.Longitude).IsRequired();
            entity.HasOne(e => e.Station)
                  .WithOne(s => s.Coordinates)
                  .HasForeignKey<Coordinates>(e => e.EvaNumber)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING") ?? throw new ArgumentNullException("POSTGRES_CONNECTION_STRING", "PostgreSQL connection string not configured");
        
        var parsed = ParseToEfConnectionString(connectionString);
        optionsBuilder.UseNpgsql(parsed);
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