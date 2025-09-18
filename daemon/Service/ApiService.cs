using System.Text.Json;
using daemon.Models.Core;
using Microsoft.Extensions.Logging;

namespace daemon.Service;

public class ApiService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<ApiService> _logger;

	private const string RisApiUrl =
		"https://apis.deutschebahn.com/db-api-marketplace/apis/ris-stations/v1/stop-places/by-position?latitude={0}&longitude={1}&radius={2}&groupBy={3}&onlyActive=false&limit=10000";
	private const string StadaApiUrl = "https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/stations";

	public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
	{
		_httpClient = httpClient;
		_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");

		_httpClient.DefaultRequestHeaders.Add("DB-Client-Id", Environment.GetEnvironmentVariable("STATIONS_CLIENT_ID") ?? "STATIONS_CLIENT_ID environment variable not set.");
		_httpClient.DefaultRequestHeaders.Add("DB-Api-Key", Environment.GetEnvironmentVariable("STATIONS_API_KEY") ?? "STATIONS_API_KEY environment variable not set.");
		_httpClient.DefaultRequestHeaders.Add("X-Correlation-Id", Guid.NewGuid() + "_" + Guid.NewGuid());
	}

	public async Task<JsonElement> GetRisStations(
		GeoPosition geoPosition,
		int radius,
		bool grouped,
		CancellationToken token = default
	)
	{
		var groupBy = grouped ? "SALES" : "NONE";

		var request = await _httpClient.GetAsync(
			string.Format(
				RisApiUrl,
				geoPosition.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
				geoPosition.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
				radius,
				groupBy
			),
			token
		);
		request.EnsureSuccessStatusCode();

		using var document = JsonDocument.Parse(await request.Content.ReadAsStringAsync(token));
		return document.RootElement.Clone();
	}

	public async Task<JsonElement> GetStadaStations(CancellationToken token = default)
	{
		var request = await _httpClient.GetAsync(StadaApiUrl, token);
		request.EnsureSuccessStatusCode();

		using var document = JsonDocument.Parse(await request.Content.ReadAsStringAsync(token));
		return document.RootElement.Clone();
	}
}
