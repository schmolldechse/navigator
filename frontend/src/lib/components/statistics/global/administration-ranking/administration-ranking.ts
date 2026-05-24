import { MetricSeriesType } from "@lib/api";
import type { vBaseMetricRequestAdministrationRankingMetricRequest } from "@lib/api/valibot.gen";
import { toJourneyRankingMetricScopeRequest, type StatisticsScopeSettings } from "../statistics-scope";
import * as v from "valibot";

type AdministrationRankingMetricOption = {
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

type AdministrationRankingSettings = {
	seriesType: MetricSeriesType;
	limit: number;
	offset: number;
};

const ADMINISTRATION_RANKING_METRIC_OPTIONS: AdministrationRankingMetricOption[] = [
	{
		seriesType: MetricSeriesType.ADMINISTRATION_RANKING_COUNT,
		label: "Journey count",
		description: "Recorded journeys per operator, grouped by the journey start hour.",
		valueLabel: "Journeys",
		polarity: "neutral",
		rankingDescription: "Operators with the most recorded journeys appear first."
	},
	{
		seriesType: MetricSeriesType.ADMINISTRATION_RANKING_CANCELLATION_COUNT,
		label: "Cancellations",
		description: "Journeys marked as cancelled for each operator.",
		valueLabel: "Cancelled",
		polarity: "negative",
		rankingDescription: "Operators with the most cancelled journeys appear first."
	},
	{
		seriesType: MetricSeriesType.ADMINISTRATION_RANKING_CANCELLATION_RATE,
		label: "Cancellation Rate",
		description: "Share of recorded journeys that were marked as cancelled.",
		valueLabel: "Rate",
		polarity: "negative",
		rankingDescription: "Operators with the highest cancellation rate appear first.",
		sampleLabels: {
			numerator: "Cancelled journeys",
			denominator: "Recorded journeys"
		}
	},
	{
		seriesType: MetricSeriesType.ADMINISTRATION_RANKING_AVERAGE_DELAY,
		label: "Average Delay",
		description: "Average terminal delay of non-cancelled journeys with a measured final stop delay.",
		valueLabel: "Delay",
		polarity: "negative",
		rankingDescription: "Operators with the highest average terminal delay appear first.",
		sampleLabels: {
			numerator: "Total terminal delay seconds",
			denominator: "Measured journeys"
		}
	},
	{
		seriesType: MetricSeriesType.ADMINISTRATION_RANKING_PUNCTUALITY5_RATE,
		label: "Punctuality 5",
		description: "Share of non-cancelled measured journeys with terminal delay below five minutes.",
		valueLabel: "Rate",
		polarity: "positive",
		rankingDescription: "Operators with the best five-minute punctuality appear first.",
		sampleLabels: {
			numerator: "Punctual journeys",
			denominator: "Measured journeys"
		}
	},
	{
		seriesType: MetricSeriesType.ADMINISTRATION_RANKING_PUNCTUALITY15_RATE,
		label: "Punctuality 15",
		description: "Share of non-cancelled measured journeys with terminal delay below fifteen minutes.",
		valueLabel: "Rate",
		polarity: "positive",
		rankingDescription: "Operators with the best fifteen-minute punctuality appear first.",
		sampleLabels: {
			numerator: "Punctual journeys",
			denominator: "Measured journeys"
		}
	}
];

const createAdministrationRankingRequest = (
	settings: AdministrationRankingSettings,
	scope: StatisticsScopeSettings
): v.InferOutput<typeof vBaseMetricRequestAdministrationRankingMetricRequest> => ({
	queryType: "ADMINISTRATION_RANKING",
	seriesType: settings.seriesType,
	...toJourneyRankingMetricScopeRequest(scope),
	limit: settings.limit,
	offset: settings.offset
});

const createDefaultAdministrationRankingSettings = (): AdministrationRankingSettings => ({
	seriesType: MetricSeriesType.ADMINISTRATION_RANKING_COUNT,
	limit: 10,
	offset: 0
});

const getAdministrationRankingMetricOption = (seriesType: MetricSeriesType): AdministrationRankingMetricOption | undefined =>
	ADMINISTRATION_RANKING_METRIC_OPTIONS.find((option) => option.seriesType === seriesType);

export {
	type AdministrationRankingMetricOption,
	type AdministrationRankingSettings,
	ADMINISTRATION_RANKING_METRIC_OPTIONS,
	createAdministrationRankingRequest,
	createDefaultAdministrationRankingSettings,
	getAdministrationRankingMetricOption
};
