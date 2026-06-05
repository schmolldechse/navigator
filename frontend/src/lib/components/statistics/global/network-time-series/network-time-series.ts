import { MetricSeriesType, type MetricSeries } from "@lib/api";
import { loadMetric } from "@lib/remote/metrics.remote";
import { toMetricScopeRequest, type StatisticsScopeSettings } from "../statistics-scope";

type NetworkTimeSeriesPromises = {
	eventCount: Promise<MetricSeries>;
	cancellationCount: Promise<MetricSeries>;
	punctuality5: Promise<MetricSeries>;
	punctuality15: Promise<MetricSeries>;
	cancellationRate: Promise<MetricSeries>;
	averageDelay: Promise<MetricSeries>;
};

type NetworkTimeSeriesOption = {
	seriesType: MetricSeriesType;
	label: string;
	description: string;
};

const NETWORK_TIME_SERIES_OPTIONS: NetworkTimeSeriesOption[] = [
	{
		seriesType: MetricSeriesType.STATION_EVENT_COUNT,
		label: "Station events",
		description: "Number of recorded station events in the selected range."
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_CANCELLATION_COUNT,
		label: "Cancellations",
		description: "Cancelled station events."
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_CANCELLATION_RATE,
		label: "Cancellation Rate",
		description: "Share of station events that were cancelled."
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_DELAY_AVERAGE,
		label: "Average Delay",
		description: "Average delay in seconds for measured events."
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY5_RATE,
		label: "Punctual <= 5:59 min",
		description: "Share of measured events with less than six minutes delay."
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY15_RATE,
		label: "Punctual <= 14:59 min",
		description: "Share of measured events with less than fifteen minutes delay."
	}
];

const createNetworkTimeSeriesRequest = (scope: StatisticsScopeSettings, seriesType: MetricSeriesType, evaNumber?: number) =>
	evaNumber
		? {
				queryType: "STATION_EVENT_QUALITY_TIME_SERIES" as const,
				seriesType,
				...toMetricScopeRequest(scope),
				evaNumbers: [evaNumber],
				includeRil100: true
			}
		: {
				queryType: "NETWORK_STATION_EVENT_QUALITY_TIME_SERIES" as const,
				seriesType,
				...toMetricScopeRequest(scope)
			};

const createNetworkTimeSeriesRequests = (scope: StatisticsScopeSettings, evaNumber?: number) => ({
	eventCount: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_COUNT, evaNumber),
	cancellationCount: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_CANCELLATION_COUNT, evaNumber),
	punctuality5: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_PUNCTUALITY5_RATE, evaNumber),
	punctuality15: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_PUNCTUALITY15_RATE, evaNumber),
	cancellationRate: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_CANCELLATION_RATE, evaNumber),
	averageDelay: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_DELAY_AVERAGE, evaNumber)
});

const createNetworkTimeSeriesPromises = (
	scope: StatisticsScopeSettings,
	userIp?: string,
	evaNumber?: number
): NetworkTimeSeriesPromises => {
	const requests = createNetworkTimeSeriesRequests(scope, evaNumber);

	return {
		eventCount: loadMetric({ request: requests.eventCount, userIp }),
		cancellationCount: loadMetric({ request: requests.cancellationCount, userIp }),
		punctuality5: loadMetric({ request: requests.punctuality5, userIp }),
		punctuality15: loadMetric({ request: requests.punctuality15, userIp }),
		cancellationRate: loadMetric({ request: requests.cancellationRate, userIp }),
		averageDelay: loadMetric({ request: requests.averageDelay, userIp })
	};
};

const getNetworkTimeSeriesOption = (seriesType: MetricSeriesType): NetworkTimeSeriesOption | undefined =>
	NETWORK_TIME_SERIES_OPTIONS.find((option: NetworkTimeSeriesOption) => option.seriesType === seriesType);

export {
	type NetworkTimeSeriesPromises,
	type NetworkTimeSeriesOption,
	NETWORK_TIME_SERIES_OPTIONS,
	createNetworkTimeSeriesRequest,
	createNetworkTimeSeriesRequests,
	createNetworkTimeSeriesPromises,
	getNetworkTimeSeriesOption
};
