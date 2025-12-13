using Microsoft.Extensions.Logging;
using Navigator.Data.Models.Ris;
using Navigator.Data.Repository.StationRepository;
using Navigator.Preflight.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Navigator.Preflight.Infrastructure.Discovery;

public class StationDiscovery(
    ILogger<StationDiscovery> logger,
    IStationRepository stationRepository
) : IStationDiscovery
{
    private readonly string _tempPath = Path.Combine(Path.GetTempPath(), "navigator", "station_discovery");

    private const int _maxResult = 10_000, _maxDepth = 8, _maxRequests = 1_000;
    private readonly BoundingBox _defaultBoundingBox = new(
        North: 70.0,
        West: -10.0,
        South: 35.0,
        East: 40.0
    );

    public async Task StartDiscoveringAsync()
    {
        if (!Directory.Exists(_tempPath)) Directory.CreateDirectory(_tempPath);
        if (await CheckManifestFile())
        {
            logger.LogInformation("Skipping discovery process.");
            return;
        }

        logger.LogInformation("Starting StationDiscovery process. This thakes a while.");
        var watch = new Stopwatch();
        watch.Start();

        logger.LogInformation("Fetching stations from StaDa...");
        var stadaStations = await stationRepository.GetStaDaAsync();
        logger.LogInformation("Fetched {Count} stations from StaDa. Took {Time} ms", stadaStations.Count(), watch.ElapsedMilliseconds);

        watch.Restart();
        await File.WriteAllTextAsync(
            Path.Combine(_tempPath, "stada.json"),
            JsonSerializer.Serialize(stadaStations),
            encoding: Encoding.UTF8
        );
        logger.LogInformation("Saved StaDa stations. Took {Time} ms", watch.ElapsedMilliseconds);

        watch.Restart();
        logger.LogInformation("Fetching stations from RIS...");
        await ProcessQuarterBoundingBox(_defaultBoundingBox, depth: 0, requestCount: 0);
        logger.LogInformation("Finished fetching stations from RIS. Took {Time} ms", watch.ElapsedMilliseconds);

        watch.Stop();

        await WriteManifestFile();
    }

    private async Task<int> ProcessQuarterBoundingBox(BoundingBox boundingBox, int depth, int requestCount)
    {
        if (depth > _maxDepth || requestCount > _maxRequests)
        {
            logger.LogInformation("Done at BoundingBox {BoundingBox}.", boundingBox);
            return requestCount;
        }

        await Task.Delay(Random.Shared.Next(100, 5_000));

        var center = GeoCalculations.GetBoundingBoxCenter(boundingBox);
        var radius = GeoCalculations.CalculateRadius(boundingBox, center);

        var stations = await GatherRisStations(depth, boundingBox, center, radius, grouped: false);
        requestCount++;

        if (stations < _maxResult)
        {
            await GatherRisStations(depth, boundingBox, center, radius, grouped: true);
            requestCount++;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                var quarterBoundingBox = GeoCalculations.GetBoundingBoxQuarter(boundingBox, i);
                requestCount = await ProcessQuarterBoundingBox(quarterBoundingBox, depth + 1, requestCount);
            }
        }

        return requestCount;
    }

    private async Task<int> GatherRisStations(int depth, BoundingBox boundingBox, (double latitude, double longitude) center, int radius, bool grouped)
    {
        var risStations = await stationRepository.GetRisStationsByCoordinatesAsync(new()
        {
            Latitude = center.latitude,
            Longitude = center.longitude,
            Radius = radius,
            GroupBy = grouped ? RisStations.StopPlaceSearchGroupByKey.SALES : RisStations.StopPlaceSearchGroupByKey.NONE,
            Limit = _maxResult
        });

        await File.WriteAllTextAsync(
            Path.Combine(_tempPath, depth + "_" + boundingBox.ToString() + "_" + (grouped ? "_grouped" : "_single") + ".json"),
            JsonSerializer.Serialize(risStations),
            encoding: Encoding.UTF8
        );
        return risStations.Count();
    }

    private async Task WriteManifestFile() => await File.WriteAllTextAsync(
        Path.Combine(_tempPath, "manifest.json"),
        JsonSerializer.Serialize(new { done = true }),
        encoding: Encoding.UTF8);

    private async Task<bool> CheckManifestFile()
    {
        var manifestPath = Path.Combine(_tempPath, "manifest.json");
        if (!File.Exists(manifestPath)) return false;

        var manifest = JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync(manifestPath));
        return manifest.TryGetProperty("done", out var doneProperty) && doneProperty.GetBoolean();
    }
}
