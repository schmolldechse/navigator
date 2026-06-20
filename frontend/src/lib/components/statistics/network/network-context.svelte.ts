import { createContext } from "svelte";
import { StatisticsBucket } from "@lib/api";
import {
	defaultEventScope,
	defaultGlobalScope,
	normalizeBucketForRange,
	normalizeDateRange,
	type BaseStatisticsFilterDraft,
	type StatisticsEventScope,
	type StatisticsGlobalScope
} from "../shared/statistics-dashboard";

type NetworkHeatmapMetric = "reliability5" | "reliability15" | "cancellation" | "plannedStops";
type NetworkDistributionMode = "histogram" | "cdf";
type NetworkMapMetric = "reliability5" | "operative5" | "cancellation" | "averagePositiveDelay" | "plannedStops";
type NetworkPunctualityPerspective = "customer" | "operative";
type NetworkTransportComparisonMode = "gap" | "outcomes";

type NetworkMetricScope = {
	eventTimeSeries: { bucket: StatisticsBucket; perspective: NetworkPunctualityPerspective };
	journeyTimeSeries: { bucket: StatisticsBucket };
	weekdayHourHeatmap: { metric: NetworkHeatmapMetric };
	mapHotspots: { metric: NetworkMapMetric };
	delayDistribution: { mode: NetworkDistributionMode };
	transportTypeComparison: { mode: NetworkTransportComparisonMode };
};

type NetworkFilterDraft = BaseStatisticsFilterDraft;

const defaultNetworkMetricScope = (): NetworkMetricScope => ({
	eventTimeSeries: { bucket: StatisticsBucket.DAY, perspective: "customer" },
	journeyTimeSeries: { bucket: StatisticsBucket.DAY },
	weekdayHourHeatmap: { metric: "reliability5" },
	mapHotspots: { metric: "reliability5" },
	delayDistribution: { mode: "histogram" },
	transportTypeComparison: { mode: "gap" }
});

const createNetworkFilterDraft = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope
): NetworkFilterDraft => ({
	from: globalScope.from,
	to: globalScope.to,
	scheduleType: eventScope.scheduleType,
	transportTypes: [...globalScope.transportTypes],
	includeReplacement: globalScope.includeReplacement
});

class NetworkStatisticsContext {
	global: StatisticsGlobalScope = $state(defaultGlobalScope());
	event: StatisticsEventScope = $state(defaultEventScope());
	network: NetworkMetricScope = $state(defaultNetworkMetricScope());

	get filterDraft(): NetworkFilterDraft {
		return createNetworkFilterDraft(this.global, this.event);
	}

	applyFilters = (draft: NetworkFilterDraft) => {
		const range = normalizeDateRange(draft.from, draft.to);

		this.global.from = range.from;
		this.global.to = range.to;
		this.global.transportTypes = [...draft.transportTypes];
		this.global.includeReplacement = draft.includeReplacement;
		this.event.scheduleType = draft.scheduleType;
		this.network.eventTimeSeries.bucket = normalizeBucketForRange(this.network.eventTimeSeries.bucket, this.global);
		this.network.journeyTimeSeries.bucket = normalizeBucketForRange(this.network.journeyTimeSeries.bucket, this.global);
	};

	resetFilters = () => {
		Object.assign(this.global, defaultGlobalScope());
		Object.assign(this.event, defaultEventScope());
		Object.assign(this.network, defaultNetworkMetricScope());
	};

	setEventBucket = (bucket: StatisticsBucket) => {
		this.network.eventTimeSeries.bucket = normalizeBucketForRange(bucket, this.global);
	};

	setEventPerspective = (perspective: NetworkPunctualityPerspective) => {
		this.network.eventTimeSeries.perspective = perspective;
	};

	setJourneyBucket = (bucket: StatisticsBucket) => {
		this.network.journeyTimeSeries.bucket = normalizeBucketForRange(bucket, this.global);
	};

	setHeatmapMetric = (metric: NetworkHeatmapMetric) => {
		this.network.weekdayHourHeatmap.metric = metric;
	};

	setMapMetric = (metric: NetworkMapMetric) => {
		this.network.mapHotspots.metric = metric;
	};

	setDistributionMode = (mode: NetworkDistributionMode) => {
		this.network.delayDistribution.mode = mode;
	};

	setTransportComparisonMode = (mode: NetworkTransportComparisonMode) => {
		this.network.transportTypeComparison.mode = mode;
	};
}

const [getNetworkStatisticsContext, setNetworkStatisticsContext] = createContext<NetworkStatisticsContext>();

export {
	NetworkStatisticsContext,
	getNetworkStatisticsContext,
	setNetworkStatisticsContext,
	type NetworkDistributionMode,
	type NetworkFilterDraft,
	type NetworkHeatmapMetric,
	type NetworkMapMetric,
	type NetworkPunctualityPerspective,
	type NetworkTransportComparisonMode
};
