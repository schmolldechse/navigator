import { MetricSeriesType, type MetricSeries } from "@lib/api";
import { toMetricScopeRequest, type StatisticsScopeSettings } from "../statistics-scope";

type NetworkTimeSeriesPromises = {
	eventCount: Promise<MetricSeries>;
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
		label: "Punctuality 5",
		description: "Share of measured events below five minutes delay."
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY15_RATE,
		label: "Punctuality 15",
		description: "Share of measured events below fifteen minutes delay."
	}
];

const createNetworkTimeSeriesRequest = (scope: StatisticsScopeSettings, seriesType: MetricSeriesType) => ({
	queryType: "NETWORK_STATION_EVENT_QUALITY_TIME_SERIES" as const,
	seriesType,
	...toMetricScopeRequest(scope)
});

const createNetworkTimeSeriesRequests = (scope: StatisticsScopeSettings) => ({
	eventCount: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_COUNT),
	punctuality5: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_PUNCTUALITY5_RATE),
	punctuality15: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_PUNCTUALITY15_RATE),
	cancellationRate: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_CANCELLATION_RATE),
	averageDelay: createNetworkTimeSeriesRequest(scope, MetricSeriesType.STATION_EVENT_DELAY_AVERAGE)
});

const getNetworkTimeSeriesOption = (seriesType: MetricSeriesType): NetworkTimeSeriesOption | undefined =>
	NETWORK_TIME_SERIES_OPTIONS.find((option: NetworkTimeSeriesOption) => option.seriesType === seriesType);

export {
	type NetworkTimeSeriesPromises,
	type NetworkTimeSeriesOption,
	NETWORK_TIME_SERIES_OPTIONS,
	createNetworkTimeSeriesRequest,
	createNetworkTimeSeriesRequests,
	getNetworkTimeSeriesOption
};
