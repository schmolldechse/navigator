import { MetricSeriesType } from "@lib/api";
import { formatMetricValue } from "@lib/components/statistics/metric-format";
import type { vBaseMetricRequestAdministrationRankingMetricRequest } from "@lib/api/valibot.gen";
import { DateTime } from "luxon";
import * as v from "valibot";

type AdministrationRankingMetricOption = {
	seriesType: MetricSeriesType;
	label: string;
	description: string;
	valueLabel: string;
	sampleLabels?: {
		numerator: string;
		denominator: string;
	};
};

type AdministrationRankingSettings = {
	dates: {
		start: DateTime;
		end: DateTime;
	};
	seriesType: MetricSeriesType;
	limit: number;
	offset: number;
};

const ADMINISTRATION_RANKING_METRIC_OPTIONS: AdministrationRankingMetricOption[] = [
	{
		seriesType: MetricSeriesType.ADMINISTRATION_RANKING_COUNT,
		label: "Journey count",
		description: "Recorded journeys per operator, grouped by the journey start hour.",
		valueLabel: "Journeys"
	},
	{
		seriesType: MetricSeriesType.ADMINISTRATION_RANKING_CANCELLATION_COUNT,
		label: "Cancellations",
		description: "Journeys marked as cancelled for each operator.",
		valueLabel: "Cancelled"
	},
	{
		seriesType: MetricSeriesType.ADMINISTRATION_RANKING_CANCELLATION_RATE,
		label: "Cancellation Rate",
		description: "Share of recorded journeys that were marked as cancelled.",
		valueLabel: "Rate",
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
		sampleLabels: {
			numerator: "Punctual journeys",
			denominator: "Measured journeys"
		}
	}
];

const createAdministrationRankingRequest = (
	settings: AdministrationRankingSettings
): v.InferOutput<typeof vBaseMetricRequestAdministrationRankingMetricRequest> => ({
	queryType: "ADMINISTRATION_RANKING",
	seriesType: settings.seriesType,
	start: settings.dates.start.startOf("day").toISO()!,
	end: settings.dates.end.endOf("day").toISO()!,
	limit: settings.limit,
	offset: settings.offset
});

const createDefaultAdministrationRankingSettings = (): AdministrationRankingSettings => ({
	dates: {
		start: DateTime.now().minus({ days: 7 }).startOf("day"),
		end: DateTime.now().endOf("day")
	},
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
	getAdministrationRankingMetricOption,
	formatMetricValue as formatAdministrationRankingValue
};
