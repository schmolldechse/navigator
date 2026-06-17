import { createContext } from "svelte";
import type { StatisticsBucket } from "@lib/api";
import {
	createFilterDraft,
	createLineMinVolume,
	defaultEventScope,
	defaultGlobalScope,
	defaultNetworkMetricScope,
	defaultStationMetricScope,
	normalizeBucketForRange,
	normalizeDateRange,
	type NetworkComparisonMetric,
	type NetworkDistributionMode,
	type NetworkHeatmapMetric,
	type NetworkMetricScope,
	type PagedMetricScope,
	type StatisticsEventScope,
	type StatisticsFilterDraft,
	type StatisticsGlobalScope,
	type StationMetricScope
} from "./statistics-dashboard";

class StatisticsDashboardContext {
	global: StatisticsGlobalScope = $state(defaultGlobalScope());
	event: StatisticsEventScope = $state(defaultEventScope());
	network: NetworkMetricScope = $state(defaultNetworkMetricScope());
	station: StationMetricScope = $state(defaultStationMetricScope());

	get filterDraft(): StatisticsFilterDraft {
		return createFilterDraft(this.global, this.event, this.network);
	}

	applyFilters = (draft: StatisticsFilterDraft) => {
		const range = normalizeDateRange(draft.from, draft.to);

		this.global.from = range.from;
		this.global.to = range.to;
		this.global.transportTypes = [...draft.transportTypes];
		this.global.includeReplacement = draft.includeReplacement;
		this.event.scheduleType = draft.scheduleType;

		this.network.stationRanking.minVolume = draft.minVolume;
		this.network.lineRanking.minVolume = createLineMinVolume(draft.minVolume);
		this.station.lineRanking.minVolume = createLineMinVolume(draft.minVolume);
		this.network.eventTimeSeries.bucket = normalizeBucketForRange(this.network.eventTimeSeries.bucket, this.global);
		this.network.journeyTimeSeries.bucket = normalizeBucketForRange(this.network.journeyTimeSeries.bucket, this.global);
		this.station.timeSeries.bucket = normalizeBucketForRange(this.station.timeSeries.bucket, this.global);

		this.resetPagedOffsets();
	};

	resetFilters = () => {
		Object.assign(this.global, defaultGlobalScope());
		Object.assign(this.event, defaultEventScope());
		Object.assign(this.network, defaultNetworkMetricScope());
		Object.assign(this.station, defaultStationMetricScope());
	};

	setNetworkEventBucket = (bucket: StatisticsBucket) => {
		this.network.eventTimeSeries.bucket = normalizeBucketForRange(bucket, this.global);
	};

	setNetworkJourneyBucket = (bucket: StatisticsBucket) => {
		this.network.journeyTimeSeries.bucket = normalizeBucketForRange(bucket, this.global);
	};

	setNetworkHeatmapMetric = (metric: NetworkHeatmapMetric) => {
		this.network.weekdayHourHeatmap.metric = metric;
	};

	setNetworkComparisonMetric = (metric: NetworkComparisonMetric) => {
		this.network.transportTypeComparison.metric = metric;
	};

	setNetworkDistributionMode = (mode: NetworkDistributionMode) => {
		this.network.delayDistribution.mode = mode;
	};

	setStationTimeSeriesBucket = (bucket: StatisticsBucket) => {
		this.station.timeSeries.bucket = normalizeBucketForRange(bucket, this.global);
	};

	setNetworkStationRankingOffset = (offset: number) => {
		this.network.stationRanking.offset = normalizeOffset(offset, this.network.stationRanking);
	};

	setNetworkLineRankingOffset = (offset: number) => {
		this.network.lineRanking.offset = normalizeOffset(offset, this.network.lineRanking);
	};

	setStationLineRankingOffset = (offset: number) => {
		this.station.lineRanking.offset = normalizeOffset(offset, this.station.lineRanking);
	};

	setStationEventDetailsOffset = (offset: number) => {
		this.station.eventDetails.offset = Math.max(0, offset);
	};

	private resetPagedOffsets = () => {
		this.network.stationRanking.offset = 0;
		this.network.lineRanking.offset = 0;
		this.station.lineRanking.offset = 0;
		this.station.eventDetails.offset = 0;
	};
}

const normalizeOffset = (offset: number, scope: Pick<PagedMetricScope, "limit">): number => {
	const limit = Math.max(1, scope.limit);
	return Math.max(0, Math.floor(offset / limit) * limit);
};

const [getStatisticsDashboardContext, setStatisticsDashboardContext] = createContext<StatisticsDashboardContext>();

export { StatisticsDashboardContext, getStatisticsDashboardContext, setStatisticsDashboardContext };
