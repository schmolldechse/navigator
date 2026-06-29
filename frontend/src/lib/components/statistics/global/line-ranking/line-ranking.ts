import { MetricSeriesType } from "@lib/api";
import type { vBaseMetricRequestLineRankingMetricRequest } from "@lib/api/valibot.gen";
import { toJourneyRankingMetricScopeRequest, type StatisticsScopeSettings } from "../statistics-scope";
import * as v from "valibot";

type LineRankingMetricOption = {
	seriesType: MetricSeriesType;
	label: string;
	description: string;
	valueLabel: string;
	polarity: "positive" | "negative" | "neutral";
	rankingDescription: string;
	sampleLabels?: {
		numerator: string;
		denominator: string;
	};
};

type LineRankingSettings = {
	seriesType: MetricSeriesType;
	journeyDescription: string;
	number: string;
	limit: number;
	offset: number;
};

const LINE_RANKING_METRIC_OPTIONS: LineRankingMetricOption[] = [
	{
		seriesType: MetricSeriesType.LINE_RANKING_COUNT,
		label: "Journey count",
		description: "Recorded journeys per line and route.",
		valueLabel: "Journeys",
		polarity: "neutral",
		rankingDescription: "Lines with the most recorded journeys appear first."
	},
	{
		seriesType: MetricSeriesType.LINE_RANKING_CANCELLATION_COUNT,
		label: "Cancellations",
		description: "Journeys marked as cancelled for each line and route.",
		valueLabel: "Cancelled",
		polarity: "negative",
		rankingDescription: "Lines with the most cancelled journeys appear first."
	},
	{
		seriesType: MetricSeriesType.LINE_RANKING_CANCELLATION_RATE,
		label: "Cancellation Rate",
		description: "Share of recorded journeys that were marked as cancelled.",
		valueLabel: "Rate",
		polarity: "negative",
		rankingDescription: "Lines with the highest cancellation rate appear first.",
		sampleLabels: {
			numerator: "Cancelled journeys",
			denominator: "Recorded journeys"
		}
	},
	{
		seriesType: MetricSeriesType.LINE_RANKING_AVERAGE_DELAY,
		label: "Average Delay",
		description: "Average terminal delay of non-cancelled journeys with a measured final stop delay.",
		valueLabel: "Delay",
		polarity: "negative",
		rankingDescription: "Lines with the highest average terminal delay appear first.",
		sampleLabels: {
			numerator: "Total terminal delay seconds",
			denominator: "Measured journeys"
		}
	},
	{
		seriesType: MetricSeriesType.LINE_RANKING_PUNCTUALITY5_RATE,
		label: "Punctual <= 5:59 min",
		description: "Share of non-cancelled measured events with delay below six minutes.",
		valueLabel: "Rate",
		polarity: "positive",
		rankingDescription: "Lines with the best <= 5:59 min punctuality appear first.",
		sampleLabels: {
			numerator: "Punctual events",
			denominator: "Measured events"
		}
	},
	{
		seriesType: MetricSeriesType.LINE_RANKING_PUNCTUALITY15_RATE,
		label: "Punctual <= 14:59 min",
		description: "Share of non-cancelled measured journeys with terminal delay below fifteen minutes.",
		valueLabel: "Rate",
		polarity: "positive",
		rankingDescription: "Lines with the best <= 14:59 min punctuality appear first.",
		sampleLabels: {
			numerator: "Punctual journeys",
			denominator: "Measured journeys"
		}
	}
];

const createLineRankingRequest = (
	settings: LineRankingSettings,
	scope: StatisticsScopeSettings,
	evaNumbers?: number[]
): v.InferOutput<typeof vBaseMetricRequestLineRankingMetricRequest> => ({
	queryType: "LINE_RANKING",
	seriesType: settings.seriesType,
	...toJourneyRankingMetricScopeRequest(scope),
	scheduleType: evaNumbers?.length ? scope.scheduleType : undefined,
	evaNumbers,
	includeRil100: Boolean(evaNumbers?.length),
	journeyDescription: settings.journeyDescription.trim() || undefined,
	number: settings.number.trim() || undefined,
	limit: settings.limit,
	offset: settings.offset
});

const createDefaultLineRankingSettings = (): LineRankingSettings => ({
	seriesType: MetricSeriesType.LINE_RANKING_COUNT,
	journeyDescription: "",
	number: "",
	limit: 10,
	offset: 0
});

const getLineRankingMetricOption = (seriesType: MetricSeriesType): LineRankingMetricOption | undefined =>
	LINE_RANKING_METRIC_OPTIONS.find((option) => option.seriesType === seriesType);

export {
	type LineRankingMetricOption,
	type LineRankingSettings,
	LINE_RANKING_METRIC_OPTIONS,
	createLineRankingRequest,
	createDefaultLineRankingSettings,
	getLineRankingMetricOption
};
