using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using stations.Api;
using stations.Models;

namespace stations;

public class StationDiscovery
{
    private readonly ApiService _apiService;
    private readonly ILogger<StationDiscovery> _logger;

    private readonly BoundingBox _defaultBoundingBox = new(North: 70.0, West: -10.0, South: 35.0, East: 40.0);

    private const int LimitResults = 10_000;
    private const int MaxDepth = 8;
    private const int MaxRequests = 1000;

    private readonly Random _random = new();

    private readonly string _tempFolder = Path.Combine(Path.GetTempPath(), "navigator", "stations");
    
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public StationDiscovery(ApiService apiService, ILogger<StationDiscovery> logger)
    {
        _apiService = apiService;
        _logger = logger;

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        if (!Directory.Exists(_tempFolder)) Directory.CreateDirectory(_tempFolder);
    }

    public async Task DiscoverStations(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching STADA stations...");
        var stada = _apiService.GetStadaStations(cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(_tempFolder, "stada.json"), JsonSerializer.Serialize(stada, _jsonSerializerOptions), Encoding.UTF8, cancellationToken);
        
        _logger.LogDebug("Fetching RIS stations...");
        await DiscoverRisStations(_defaultBoundingBox, 0, 0, cancellationToken);
        
        _logger.LogDebug("Station discovery completed.");
    }

    private async Task<int> DiscoverRisStations(BoundingBox boundingBox, int depth, int requestCount,
        CancellationToken cancellationToken)
    {
        if (depth > MaxDepth)
            throw new ArgumentException($"Maximum depth {MaxDepth} exceeded");

        if (requestCount > MaxRequests)
            throw new ArgumentException($"Request count {MaxRequests} exceeded");

        await Task.Delay(_random.Next(100, 5000), cancellationToken);

        var center = GeoCalculations.GetBoundingBoxCenter(boundingBox);
        var radius = GeoCalculations.CalculateRadius(boundingBox, center);

        var single = await SaveStations(depth, boundingBox, center, radius, false, cancellationToken);
        requestCount++;
        
        var stopPlacesCount = GetStopPlacesCount(single);
        if (stopPlacesCount < LimitResults)
        {
            await SaveStations(depth, boundingBox, center, radius, true, cancellationToken);
            requestCount++;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                var quarterBoundingBox = GeoCalculations.GetBoundingBoxQuarter(boundingBox, i);
                requestCount = await DiscoverRisStations(quarterBoundingBox, depth + 1, requestCount, cancellationToken);
            }
        }
        
        _logger.LogDebug($"Total requests made: {requestCount}");
        return requestCount;
    }

    private async Task<JsonElement> SaveStations(int depth, BoundingBox boundingBox, GeoPosition center, int radius,
        bool grouped,
        CancellationToken cancellationToken)
    {
        string path = Path.Combine(_tempFolder,
            depth + "_" + boundingBox.ToString() + "_" + (grouped ? "_grouped" : "_single") + ".json");
        var results = await _apiService.GetRisStations(center, radius, grouped, cancellationToken);

        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(results, _jsonSerializerOptions), Encoding.UTF8, cancellationToken);
        _logger.LogDebug($"{results.GetProperty("stopPlaces").GetArrayLength()} stations found");

        return results;
    }

    private int GetStopPlacesCount(JsonElement jsonElement)
    {
        if (jsonElement.TryGetProperty("stopPlaces", out var stopPlacesArray) &&
            stopPlacesArray.ValueKind == JsonValueKind.Array)
        {
            return stopPlacesArray.GetArrayLength();
        }
        return 0;
    }
}

