import { MetricSeriesType, ScheduleType, TransportType } from "@lib/api";
import type { vBaseMetricRequestStationEventQualitySummaryMetricRequest } from "@lib/api/valibot.gen";
import { DateTime } from "luxon";
import * as v from "valibot";

type StationMetricMapOption = {
	seriesType: MetricSeriesType;
	label: string;
	description: string;
	valueLabel: string;
	polarity: "positive" | "negative" | "neutral";
	visualization: "density" | "points";
	domain: [number, number] | "data";
	sampleLabels?: {
		numerator: string;
		denominator: string;
	};
};

const STATION_METRIC_MAP_OPTIONS: StationMetricMapOption[] = [
	{
		seriesType: MetricSeriesType.STATION_EVENT_COUNT,
		label: "Station events",
		description: "Number of recorded station events in the selected range.",
		valueLabel: "Events",
		polarity: "neutral",
		visualization: "density",
		domain: "data"
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_CANCELLATION_COUNT,
		label: "Cancellations",
		description: "Cancelled station events.",
		valueLabel: "Cancelled",
		polarity: "negative",
		visualization: "density",
		domain: "data"
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_CANCELLATION_RATE,
		label: "Cancellation Rate",
		description: "Share of station events that were cancelled.",
		valueLabel: "Rate",
		polarity: "negative",
		visualization: "points",
		domain: [0, 100],
		sampleLabels: {
			numerator: "Cancelled events",
			denominator: "Station events"
		}
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_DELAY_AVERAGE,
		label: "Average Delay",
		description: "Average delay in seconds for measured events.",
		valueLabel: "Delay",
		polarity: "negative",
		visualization: "points",
		domain: "data",
		sampleLabels: {
			numerator: "Total delay seconds",
			denominator: "Measured events"
		}
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY5_RATE,
		label: "Punctuality 5",
		description: "Share of measured events below five minutes delay.",
		valueLabel: "Rate",
		polarity: "positive",
		visualization: "points",
		domain: [0, 100],
		sampleLabels: {
			numerator: "Punctual events",
			denominator: "Measured events"
		}
	},
	{
		seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY15_RATE,
		label: "Punctuality 15",
		description: "Share of measured events below fifteen minutes delay.",
		valueLabel: "Rate",
		polarity: "positive",
		visualization: "points",
		domain: [0, 100],
		sampleLabels: {
			numerator: "Punctual events",
			denominator: "Measured events"
		}
	}
];

type StationMetricMapSettings = {
	dates: {
		start: DateTime;
		end: DateTime;
	};
	seriesType: MetricSeriesType;
	scheduleType: ScheduleType;
	transportTypes: TransportType[];
};

const createStationMetricMapRequest = (
	settings: StationMetricMapSettings
): v.InferOutput<typeof vBaseMetricRequestStationEventQualitySummaryMetricRequest> => ({
	queryType: "STATION_EVENT_QUALITY_SUMMARY",
	seriesType: settings.seriesType,
	scheduleType: settings.scheduleType,
	start: settings.dates.start.startOf("day").toISO()!,
	end: settings.dates.end.endOf("day").toISO()!,
	transportTypes: settings.transportTypes
});

const createDefaultStationMetricMapSettings = (): StationMetricMapSettings => ({
	dates: {
		start: DateTime.now().minus({ days: 7 }).startOf("day"),
		end: DateTime.now().endOf("day")
	},
	seriesType: MetricSeriesType.STATION_EVENT_COUNT,
	scheduleType: ScheduleType.DEPARTURE,
	transportTypes: []
});

const getStationMetricMapOption = (seriesType: MetricSeriesType): StationMetricMapOption | undefined =>
	STATION_METRIC_MAP_OPTIONS.find((option: StationMetricMapOption) => option.seriesType === seriesType);

export {
	type StationMetricMapOption,
	STATION_METRIC_MAP_OPTIONS,
	type StationMetricMapSettings,
	createStationMetricMapRequest,
	createDefaultStationMetricMapSettings,
	getStationMetricMapOption
};
