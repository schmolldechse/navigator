using System.Text.Json;
using daemon.Database;
using daemon.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace daemon.Manager;

public class GatheringRisIdsDaemon : Daemon
{
    private readonly ILogger<GatheringRisIdsDaemon> _logger;
    private readonly NavigatorDbContext _dbContext;

    private readonly HttpClient _httpClient = new();

    private readonly string _apiUrl =
        "https://regio-guide.de/@prd/zupo-travel-information/api/public/ri/board/{0}/{1}?timeStart={2}&timeEnd={3}&expandTimeFrame=TIME_START&modeOfTransport=HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,CITY_TRAIN,BUS,FERRY,SUBWAY,TRAM,SHUTTLE,UNKNOWN";

    public GatheringRisIdsDaemon(ILogger<GatheringRisIdsDaemon> logger, NavigatorDbContext dbContext)
        : base("Gathering RIS IDs", TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(300), logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext), "Database context cannot be null");
    }

    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var date = DateTime.UtcNow;

        var randomStation = await _dbContext.Stations
            .Where(s => s.QueryingEnabled == true && (s.LastQueried == null || s.LastQueried < date.Date.AddDays(-1)))
            .Include(s => s.Products)
            .OrderBy(s => Guid.NewGuid())
            .FirstOrDefaultAsync(cancellationToken);
        if (randomStation == null) return;

        if (randomStation.LastQueried == null)
        {
            // the API above saves RIS id's up to 7 days in the past
            var startDate = date.AddDays(-7);
            _logger.LogInformation(
                "{Name} (evaNumber: {EvaNumber}) has not been queried yet! Set 'last_queried to' {StartDate}",
                randomStation.Name, randomStation.EvaNumber, startDate);

            await ProcessStation(randomStation, startDate, cancellationToken);
        }
        else if (randomStation.LastQueried < date.Date)
        {
            var newLastQueried = DateTime.SpecifyKind(new(
                DateOnly.FromDateTime(randomStation.LastQueried.Value.Date.AddDays(1)),
                TimeOnly.FromTimeSpan(date.TimeOfDay)
            ), DateTimeKind.Utc);

            await ProcessStation(randomStation, newLastQueried, cancellationToken);
        }
    }

    private async Task ProcessStation(Station station, DateTime date, CancellationToken cancellationToken)
    {
        var lookupDate = DateOnly.FromDateTime(date.Date);
        var lookupDateWithDuration = DateOnly.FromDateTime(date.Date.AddHours(12));

        var results = Task.WhenAll([
                CallApi(station.EvaNumber, lookupDate),
                CallApi(station.EvaNumber, lookupDate, false),
                CallApi(station.EvaNumber, lookupDateWithDuration),
                CallApi(station.EvaNumber, lookupDateWithDuration, false)
            ]).GetAwaiter().GetResult().ToArray()
            .SelectMany(x => x)
            .DistinctBy(x => x.Id)
            .ToList();

        var enabledProducts = station.Products
            .Where(p => p.QueryingEnabled)
            .Select(p => p.ProductName)
            .ToHashSet();
        // filter out RIS IDs that don't match enabled products
        var filteredByProduct = results.Where(risId => enabledProducts.Contains(risId.Product)).ToList();

        // sort out already existing RIS IDs
        var existingRisIds = _dbContext.RisIds.Select(risId => risId.Id).ToHashSet();
        var newIds = filteredByProduct.Where(risId => !existingRisIds.Contains(risId.Id)).ToList();

        // insert RIS IDs
        _dbContext.RisIds.AddRange(newIds);

        // update last_queried
        station.LastQueried = date;
        var amount = await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Retrieved a total of {Count} RIS IDs for station {StationName} (evaNumber: {EvaNumber}): Filtered out by product: {Filtered}, Already in DB: {FilteredByDB}, Inserted: {Inserted}",
            results.Count, station.Name, station.EvaNumber, (results.Count - filteredByProduct.Count), (filteredByProduct.Count - newIds.Count),
            (amount - 1));
    }

    private async Task<List<IdentifiedRisId>> CallApi(int evaNumber, DateOnly date,
        bool isDeparture = true)
    {
        var boardType = isDeparture ? "departure" : "arrival";

        var timeStart = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var timeEnd = timeStart.AddMinutes(720);

        var response = await _httpClient.GetAsync(string.Format(_apiUrl, boardType, evaNumber,
            Uri.EscapeDataString(timeStart.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")),
            Uri.EscapeDataString(timeEnd.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))));
        if (!response.IsSuccessStatusCode) return new();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var content = (await JsonDocument.ParseAsync(stream)).RootElement;

        var items = content.GetProperty("items");
        if (items.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException("Expected 'items' to be an array");
        if (items.GetArrayLength() == 0) return new();

        return items.EnumerateArray().Select(item =>
        {
            if (!item.TryGetProperty("train", out JsonElement trainElement))
                throw new InvalidOperationException("Expected 'train' property in item");
            return new IdentifiedRisId()
            {
                Id = TryParse(trainElement.GetProperty("journeyId")),
                Product = Product.MapProduct(trainElement.GetProperty("type").GetString()) ??
                          throw new InvalidOperationException("Expected 'product' to be a non-null string"),
                DiscoveryDate = DateTime.UtcNow
            };
        }).ToList();
    }

    private Guid TryParse(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException("Expected JSON element to be a string");

        string fullTripId = jsonElement.GetString() ??
                            throw new InvalidOperationException("Expected JSON element to be a non-null string");

        // Format: yyyyMMdd-{UUID}
        string datePart = fullTripId.Substring(0, 8);
        if (!DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out _))
            throw new FormatException("Input does not start with a valid date");

        string uuidPart = fullTripId.Substring(9);
        if (!Guid.TryParse(uuidPart, out Guid guid))
            throw new FormatException("Input does not contain a valid UUID after the date");
        return guid;
    }

    public override void Dispose()
    {
        _httpClient.Dispose();
        base.Dispose();
    }
}
