import { MetricSeriesType, ScheduleType } from "@lib/api";
import type { StationMetricMapSettings } from "../station-metric-map/station-metric-map";
import { DateTime } from "luxon";

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

const createNetworkTimeSeriesRequest = (settings: StationMetricMapSettings) => ({
	queryType: "NETWORK_STATION_EVENT_QUALITY_TIME_SERIES" as const,
	seriesType: settings.seriesType,
	scheduleType: settings.scheduleType,
	start: settings.dates.start.startOf("day").toISO()!,
	end: settings.dates.end.endOf("day").toISO()!,
	transportTypes: settings.transportTypes
});

const createDefaultNetworkTimeSeriesSettings = (): StationMetricMapSettings => ({
	dates: {
		start: DateTime.now().minus({ days: 7 }).startOf("day"),
		end: DateTime.now().endOf("day")
	},
	seriesType: MetricSeriesType.STATION_EVENT_COUNT,
	scheduleType: ScheduleType.DEPARTURE,
	transportTypes: []
});

const getNetworkTimeSeriesOption = (seriesType: MetricSeriesType): NetworkTimeSeriesOption | undefined =>
	NETWORK_TIME_SERIES_OPTIONS.find((option: NetworkTimeSeriesOption) => option.seriesType === seriesType);

export {
	type NetworkTimeSeriesOption,
	NETWORK_TIME_SERIES_OPTIONS,
	createNetworkTimeSeriesRequest,
	createDefaultNetworkTimeSeriesSettings,
	getNetworkTimeSeriesOption
};
