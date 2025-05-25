using System.Text.Json;
using Microsoft.Extensions.Logging;
using stations.Models;

namespace stations.Api;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AppConfiguration _appConfiguration;

    private readonly ILogger<ApiService> _logger;

    private const string _risApiUrl = "https://apis.deutschebahn.com/db-api-marketplace/apis/ris-stations/v1/stop-places/by-position?latitude={0}&longitude={1}&radius={2}&groupBy={3}&onlyActive=false&limit=10000";
    private const string _stadaApiUrl = "https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/stations";
    
    public ApiService(HttpClient httpClient, AppConfiguration appConfiguration, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _appConfiguration = appConfiguration;
        _logger = logger;
        
        _httpClient.DefaultRequestHeaders.Add("DB-Client-Id", _appConfiguration.ClientId);
        _httpClient.DefaultRequestHeaders.Add("DB-Api-Key", _appConfiguration.ClientSecret);
        _httpClient.DefaultRequestHeaders.Add("X-Correlation-Id", Guid.NewGuid() + "_" + Guid.NewGuid());
    }

    public async Task<JsonElement> GetRisStations(GeoPosition geoPosition, int radius, bool grouped,
        CancellationToken token = default)
    {
        var groupBy = grouped ? "SALES" : "NONE";
        
        var request = await _httpClient.GetAsync(string.Format(_risApiUrl, geoPosition.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture), geoPosition.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture), radius, groupBy), token);
        request.EnsureSuccessStatusCode();
        
        using var document = JsonDocument.Parse(await request.Content.ReadAsStringAsync(token));
        return document.RootElement.Clone();
    }

    public async Task<JsonElement> GetStadaStations(CancellationToken token = default)
    {
        var request = await _httpClient.GetAsync(_stadaApiUrl, token);
        request.EnsureSuccessStatusCode();
        
        using var document = JsonDocument.Parse(await request.Content.ReadAsStringAsync(token));
        return document.RootElement.Clone();
    }
}

