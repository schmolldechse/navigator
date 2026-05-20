import { MetricSeriesType, MetricUnit, ScheduleType, TransportType } from "@lib/api";
import { DateTime } from "luxon";

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

const formatCompactNumber = (value: number, maximumFractionDigits: number) =>
	value.toLocaleString(undefined, {
		maximumFractionDigits
	});

const formatHeatmapMetricValue = (value: number, unit: MetricUnit) => {
	if (!Number.isFinite(value)) return { value: "N/A", unit: "" };

	if (unit === MetricUnit.SECONDS) {
		if (Math.abs(value) >= 60) return { value: formatCompactNumber(value / 60, 1), unit: "min" };
		return { value: formatCompactNumber(value, 0), unit: "s" };
	}

	if (unit === MetricUnit.PERCENT) return { value: formatCompactNumber(value, Math.abs(value) < 10 ? 1 : 0), unit: "%" };
	if (unit === MetricUnit.COUNT) return { value: value.toLocaleString(), unit: "" };
	if (unit === MetricUnit.BYTES) return { value: value.toLocaleString(), unit: "B" };

	return { value: formatCompactNumber(value, Math.abs(value) < 10 ? 2 : 0), unit };
};

export {
	type HeatmapMetricOption,
	HEATMAP_METRIC_OPTIONS,
	type HeatmapSettings,
	createDefaultHeatmapSettings,
	getHeatmapMetricOption,
	formatHeatmapMetricValue
};
