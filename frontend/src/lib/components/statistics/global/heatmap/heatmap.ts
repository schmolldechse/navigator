import { MetricSeriesType, ScheduleType, TransportType } from "@lib/api";
import { formatMetricValue } from "@lib/components/statistics/metric-format";
import type { vBaseMetricRequestStationEventQualitySummaryMetricRequest } from "@lib/api/valibot.gen";
import { DateTime } from "luxon";
import * as v from "valibot";

type HeatmapMetricOption = {
	seriesType: MetricSeriesType;
	label: string;
	description: string;
	valueLabel: string;
};

const HEATMAP_METRIC_OPTIONS: HeatmapMetricOption[] = [
	{
		seriesType: MetricSeriesType.STATION_EVENT_COUNT,
		label: "Journey count",
		description: "Number of recorded station events in the selected range.",
		valueLabel: "Events"
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_CANCELLATION_COUNT,
		label: "Cancellations",
		description: "Cancelled station events.",
		valueLabel: "Cancelled"
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_CANCELLATION_RATE,
		label: "Cancellation Rate",
		description: "Share of station events that were cancelled.",
		valueLabel: "Rate"
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_DELAY_AVERAGE,
		label: "Average Delay",
		description: "Average delay in seconds for measured events.",
		valueLabel: "Delay"
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY5_RATE,
		label: "Punctuality 5",
		description: "Share of measured events below five minutes delay.",
		valueLabel: "Rate"
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY15_RATE,
		label: "Punctuality 15",
		description: "Share of measured events below fifteen minutes delay.",
		valueLabel: "Rate"
	}
];

type HeatmapSettings = {
	dates: {
		start: DateTime;
		end: DateTime;
	};
	seriesType: MetricSeriesType;
	scheduleType: ScheduleType;
	transportTypes: TransportType[];
};

const createHeatmapRequest = (
	settings: HeatmapSettings
): v.InferOutput<typeof vBaseMetricRequestStationEventQualitySummaryMetricRequest> => ({
	queryType: "STATION_EVENT_QUALITY_SUMMARY",
	seriesType: settings.seriesType,
	scheduleType: settings.scheduleType,
	start: settings.dates.start.startOf("day").toISO()!,
	end: settings.dates.end.endOf("day").toISO()!,
	transportTypes: settings.transportTypes
});

const createDefaultHeatmapSettings = (): HeatmapSettings => ({
	dates: {
		start: DateTime.now().minus({ days: 7 }).startOf("day"),
		end: DateTime.now().endOf("day")
	},
	seriesType: MetricSeriesType.STATION_EVENT_COUNT,
	scheduleType: ScheduleType.DEPARTURE,
	transportTypes: []
});

const getHeatmapMetricOption = (seriesType: MetricSeriesType): HeatmapMetricOption | undefined =>
	HEATMAP_METRIC_OPTIONS.find((option: HeatmapMetricOption) => option.seriesType === seriesType);

export {
	type HeatmapMetricOption,
	HEATMAP_METRIC_OPTIONS,
	type HeatmapSettings,
	createHeatmapRequest,
	createDefaultHeatmapSettings,
	getHeatmapMetricOption,
	formatMetricValue as formatHeatmapMetricValue
};
