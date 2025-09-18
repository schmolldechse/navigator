using System.CommandLine;
using daemon.Database;
using daemon.Manager;
using daemon.Service;
using daemon.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace daemon;

class Program
{
	static async Task<int> Main(string[] args)
	{
		Option<bool> debugOption = new("--debug")
		{
			Description = "Enable debug logging",
			DefaultValueFactory = _ => false
		};
		
		Option<GatheringMode> skipGatheringOption = new("--skipGathering") 
		{
			Description = "Skip station gathering. 'none' = gather from API + merge files, 'api' = skip API calls, 'full' = skip all gathering (default: api)",
			DefaultValueFactory = _ => GatheringMode.Api
		};

		var rootCommand = new RootCommand("Navigator Daemon application");
		rootCommand.Options.Add(debugOption);
		rootCommand.Options.Add(skipGatheringOption);
		
		rootCommand.SetAction(async parseResult =>
		{
			await RunApplication(parseResult.GetRequiredValue(debugOption), parseResult.GetRequiredValue(skipGatheringOption));
		});
		return await rootCommand.Parse(args).InvokeAsync();
	}

	private static async Task RunApplication(bool debug, GatheringMode skipGathering)
	{
		var services = new ServiceCollection();
		services.AddLogging(builder => builder.AddSimpleConsole(options =>
		{
			options.TimestampFormat = "[HH:mm:ss] ";
			options.IncludeScopes = false;
		}).SetMinimumLevel(debug ? LogLevel.Debug : LogLevel.Information));

		services.AddHttpClient();
		services.AddSingleton(new ManualResetEventSlim(false));
		services.AddSingleton<ProxyRotator>();

		services.AddSingleton<ApiService>();
		services.AddSingleton<StationDiscoveryService>();
		services.AddSingleton<StationMergingService>();
		
		services.AddDbContext<NavigatorDbContext>(ServiceLifetime.Scoped);
		
		services.AddSingleton<DaemonManager>();
		services.AddSingleton<GatheringRisIdsDaemon>();
		services.AddSingleton<GatheringJourneyDaemon>();
		
		var serviceProvider = services.BuildServiceProvider();
		var logger = serviceProvider.GetService<ILogger<Program>>();
		logger!.LogInformation("Navigator daemon application is starting...");
		
		Console.CancelKeyPress += (sender, eventArgs) =>
		{
			eventArgs.Cancel = true;
			logger!.LogInformation("Stopping navigator daemon application...");

			var shutdownEvent = serviceProvider.GetRequiredService<ManualResetEventSlim>();
			shutdownEvent.Set();
		};
		
		if (skipGathering != GatheringMode.Full)
			await RunStationGathering(serviceProvider, skipApi: skipGathering == GatheringMode.Api);
		RunDaemon(serviceProvider);
	}

	private static async Task RunStationGathering(ServiceProvider serviceProvider, bool skipApi = false)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
		if (!skipApi)
		{
			logger.LogInformation("Starting gathering of stations from Deutsche Bahn API...");
			var stationDiscoveryService = serviceProvider.GetRequiredService<StationDiscoveryService>();
			await stationDiscoveryService.DiscoverStations();
		}
		else logger.LogInformation("Skipping gathering of stations as requested.");

		// merge files
		var stationMergingService = serviceProvider.GetRequiredService<StationMergingService>();
		var stadaStations = await stationMergingService.LoadStadaStationsAsync(cancellationToken: CancellationToken.None);

		var stations = await stationMergingService.MergeFiles(stadaStations, CancellationToken.None);
		logger.LogInformation("Merged {Count} stations.", stations.Count);

		// calculate weights
		stations = stations.Select(station => { 
			station.Weight = stationMergingService.CalculateWeight(station, stadaStations.GetValueOrDefault(station.EvaNumber)?.PriceCategory ?? -1); 
			return station; 
		}).ToList();
		logger.LogInformation("Calculated weights for stations.");

		// filter out stations already in the database
		var dbContext = serviceProvider.GetRequiredService<NavigatorDbContext>();
		var existingEvaNumbers = dbContext.Stations.Select(s => s.EvaNumber).ToHashSet();
		var newStations = stations.Where(s => !existingEvaNumbers.Contains(s.EvaNumber)).ToList();
		logger.LogInformation("Filtered to {Count} new stations (not already in database)", newStations.Count);

		dbContext.Stations.AddRange(newStations);
		var amount = await dbContext.SaveChangesAsync();
		logger.LogInformation("Inserted {Amount} entries", amount);

		logger.LogWarning("Please change --skipGathering to 'full' to avoid calling this process again.");
	}

	private static void RunDaemon(ServiceProvider serviceProvider)
	{
		var shutdownEvent = serviceProvider.GetRequiredService<ManualResetEventSlim>();
		using var manager = serviceProvider.GetRequiredService<DaemonManager>();

		manager.AddDaemon(serviceProvider.GetRequiredService<GatheringRisIdsDaemon>());
		manager.AddDaemon(serviceProvider.GetRequiredService<GatheringJourneyDaemon>());
		manager.StartAll();

		shutdownEvent.Wait();
		// await manager.StopAll();
	}
}

public enum GatheringMode
{
	None,
	Api, // skip Api calls
	Full, // skips Api calls + inserting ~297k stations into PostgresSQL
}