using System.Data;
using System.Text.Json;
using daemon.Database;
using daemon.Models.Database;
using daemon.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace daemon.Manager;

public class GatheringRisIdsDaemon(
    ILogger<GatheringRisIdsDaemon> logger,
    IServiceProvider serviceProvider,
    ProxyRotator proxyRotator) : Daemon("Gathering RIS IDs", TimeSpan.FromSeconds(600), logger)
{
    private readonly string _apiUrl = "https://apis.deutschebahn.com/db/apis/ris-boards/v1/public/{0}/{1}?timeStart={2}&timeEnd={3}";

    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NavigatorDbContext>();

        Station? randomStation = null;
        await using (var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            randomStation = await dbContext
                .Stations.Where(station => !station.IsLocked)
                .Where(station => station.QueryingEnabled)
                .Where(station => station.LastQueried == null || station.LastQueried < DateTime.UtcNow.Date.AddDays(-1))
                .Include(station => station.Products)
                .OrderBy(_ => Guid.NewGuid())
                .FirstOrDefaultAsync(cancellationToken);
            if (randomStation == null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return;
            }

            randomStation.IsLocked = true;
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        try
        {
            // RIS::Boards gives us the opportunity to query up boards to 7 days in the past.
            if (randomStation.LastQueried == null)
            {
                var lastQueried = DateTime.UtcNow.AddDays(-7);
                logger.LogInformation(
                    "{Name} (evaNumber: {EvaNumber}) has not been queried yet! Set 'last_queried to' {StartDate}",
                    randomStation.Name,
                    randomStation.EvaNumber,
                    lastQueried
                );

                await ProcessStation(randomStation, lastQueried, dbContext, cancellationToken);
            }
            // Check if the station was last queried before today's midnight date.
            else if (randomStation.LastQueried.Value.Date < DateTime.UtcNow.Date)
            {
                var lastQueried = new DateTime(
                    DateOnly.FromDateTime(randomStation.LastQueried.Value.Date.AddDays(1)),
                    TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay),
                    DateTimeKind.Utc
                );
                logger.LogInformation(
                    "Querying {Name} (evaNumber: {EvaNumber}) for date {Date}",
                    randomStation.Name,
                    randomStation.EvaNumber,
                    lastQueried
                );

                await ProcessStation(randomStation, lastQueried, dbContext, cancellationToken);
            }
        }
        finally
        {
            await using var unlockTransaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            randomStation.IsLocked = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            await unlockTransaction.CommitAsync(cancellationToken);
        }
    }

    private async Task ProcessStation(
        Station station,
        DateTime date,
        NavigatorDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var apiResults = await Task.WhenAll(
            CallApi(station.EvaNumber, date.Date),
            CallApi(station.EvaNumber, date.Date, false),
            CallApi(station.EvaNumber, date.Date.AddHours(12)),
            CallApi(station.EvaNumber, date.Date.AddHours(12), false));
        var results = apiResults.SelectMany(identifiedRisId => identifiedRisId)
            .DistinctBy(identifiedRisId => identifiedRisId.Id)
            .ToList();

        // filter out RIS IDs that don't match enabled products
        var enabledProducts = station.Products
            .Where(product => product.QueryingEnabled)
            .Select(product => product.ProductName)
            .ToHashSet();
        var filteredByProduct = results
            .Where(identifiedRisId => enabledProducts.Contains(identifiedRisId.TransportProduct)).ToList();
        
        var discoveryDate = DateTime.UtcNow;
        filteredByProduct.ForEach(risId => risId.DiscoveryDate = discoveryDate);
        
        // upsert RIS IDs
        station.LastQueried = date;

        if (!filteredByProduct.Any())
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("No RIS IDs found to insert/ update for {StationName} (evaNumber: {EvaNumber}). No action need to be taken.", station.Name, station.EvaNumber);
            return;
        }

        var existingRisIds = await dbContext.RisIds
            .Where(identifiedRisId => filteredByProduct.Select(gatheredRisId => gatheredRisId.Id).Contains(identifiedRisId.Id))
            .ToDictionaryAsync(identifiedRisId => identifiedRisId.Id, cancellationToken);

        // separate new & existing
        var newRisIds = filteredByProduct.Where(risId => !existingRisIds.ContainsKey(risId.Id)).ToList();
        if (newRisIds.Any()) await dbContext.RisIds.AddRangeAsync(newRisIds, cancellationToken);

        var existingToUpdate = filteredByProduct.Where(risId => existingRisIds.ContainsKey(risId.Id)).ToList();
        existingToUpdate.ForEach(risId => risId.Active = true);

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Retrieved a total of {Count} RIS IDs for {StationName} (evaNumber: {EvaNumber}): Filtered out by product: {Filtered}, Already in DB (updated `active` bool): {Updated}, Inserted: {Inserted}",
            results.Count,
            station.Name,
            station.EvaNumber,
            (results.Count - filteredByProduct.Count),
            existingToUpdate.Count,
            newRisIds.Count
        );
    }

    private async Task<List<IdentifiedRisId>> CallApi(int evaNumber, DateTime timeStart, bool isDeparture = true)
    {
        var boardType = isDeparture ? "departures" : "arrivals";
        var timeEnd = timeStart.AddMinutes(720);

        var httpClient = proxyRotator.GetRandomProxy();
        var url = string.Format(_apiUrl, boardType, evaNumber,
            Uri.EscapeDataString(timeStart.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")),
            Uri.EscapeDataString(timeEnd.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Add("Accept", "application/vnd.de.db.ris+json");
        httpRequest.Headers.Add("DB-Client-Id",
            Environment.GetEnvironmentVariable("BOARDS_CLIENT_ID") ??
            throw new InvalidOperationException("BOARDS_CLIENT_ID environment variable is not set."));
        httpRequest.Headers.Add("DB-Api-Key",
            Environment.GetEnvironmentVariable("BOARDS_API_KEY") ??
            throw new InvalidOperationException("BOARDS_API_KEY environment variable is not set."));

        var response = await httpClient.SendAsync(httpRequest);
        if (!response.IsSuccessStatusCode) return [];

        await using var stream = await response.Content.ReadAsStreamAsync();
        var content = (await JsonDocument.ParseAsync(stream)).RootElement;

        var boards = content.GetProperty(boardType);
        if (boards.ValueKind != JsonValueKind.Array) throw new InvalidOperationException($"Expected '{boardType}' to be an array");
        if (boards.GetArrayLength() == 0) return [];

        return boards.EnumerateArray().Select(boardEntry =>
        {
            if (!boardEntry.TryGetProperty("journeyID", out var _))
                throw new InvalidOperationException($"Expected journeyID property in {boardType} entry");

            var transportElement = boardEntry.GetProperty("transport");
            string? replacementProduct = null;
            if (transportElement.TryGetProperty("replacementTransport", out var replacementTransportElement) &&
                replacementTransportElement.ValueKind == JsonValueKind.Object &&
                replacementTransportElement.TryGetProperty("realType", out var realTypeElement))
                replacementProduct = realTypeElement.GetString();

            return new IdentifiedRisId()
            {
                Id = TryParse(boardEntry.GetProperty("journeyID")),
                TransportProduct = boardEntry.GetProperty("transport").GetProperty("type").GetString()!,
                ReplacementTransportProduct = replacementProduct,
                DiscoveryDate = DateTime.UtcNow,
                Active = true
            };
        }).ToList();
    }

    private string TryParse(JsonElement journeyIdElement)
    {
        if (journeyIdElement.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException($"Expected 'journeyID' property to be a string but got {journeyIdElement.ValueKind}");

        // Format: yyyyMMdd-{UUID}[-UUID]
        // {} - everything inside the curly brackets is necessary
        // [] - everything inside the square brackets is optional
        string datePart = journeyIdElement.GetString()!.Substring(0, 8);
        if (!DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out _))
            throw new FormatException("Expected 'journeyID' to start with a valid date in the format 'yyyyMMdd'");
        return journeyIdElement.GetString()!.Substring(9);
    }
}