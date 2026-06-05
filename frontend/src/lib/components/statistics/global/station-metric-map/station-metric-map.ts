import { MetricSeriesType } from "@lib/api";
import type { vBaseMetricRequestStationEventQualitySummaryMetricRequest } from "@lib/api/valibot.gen";
import { toMetricScopeRequest, type StatisticsScopeSettings } from "../statistics-scope";
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
		label: "Punctual <= 5:59 min",
		description: "Share of measured events with less than six minutes delay.",
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
		label: "Punctual <= 14:59 min",
		description: "Share of measured events with less than fifteen minutes delay.",
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
	seriesType: MetricSeriesType;
};

const createStationMetricMapRequest = (
	settings: StationMetricMapSettings,
	scope: StatisticsScopeSettings
): v.InferOutput<typeof vBaseMetricRequestStationEventQualitySummaryMetricRequest> => ({
	queryType: "STATION_EVENT_QUALITY_SUMMARY",
	seriesType: settings.seriesType,
	...toMetricScopeRequest(scope)
});

const createDefaultStationMetricMapSettings = (): StationMetricMapSettings => ({
	seriesType: MetricSeriesType.STATION_EVENT_COUNT
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
