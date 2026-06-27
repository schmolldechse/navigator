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
import type { MetricPerspective } from "../shared/options/MetricPerspectiveControl.svelte";
import type { MetricThreshold } from "../shared/options/MetricThresholdControl.svelte";

type NetworkHeatmapMetric = "reliability5" | "reliability15" | "operative5" | "operative15" | "cancellation" | "plannedStops";
type NetworkDistributionMode = "histogram" | "cdf";
type NetworkDelaySeverityView = "delayed" | "all";
type NetworkHourlyProfileDayGroup = "all" | "weekday" | "weekend";
type NetworkMapMetric =
	| "reliability5"
	| "reliability15"
	| "operative5"
	| "operative15"
	| "cancellation"
	| "averagePositiveDelay"
	| "plannedStops";
type NetworkPunctualityPerspective = MetricPerspective;
type NetworkTransportComparisonThreshold = MetricThreshold;

type NetworkMetricScope = {
	eventTimeSeries: { bucket: StatisticsBucket; perspective: NetworkPunctualityPerspective };
	journeyTimeSeries: { bucket: StatisticsBucket };
	weekdayHourHeatmap: { metric: NetworkHeatmapMetric };
	delaySeverity: { view: NetworkDelaySeverityView };
	hourlyProfile: {
		dayGroup: NetworkHourlyProfileDayGroup;
		perspective: NetworkPunctualityPerspective;
		threshold: NetworkTransportComparisonThreshold;
	};
	mapHotspots: { metric: NetworkMapMetric };
	delayDistribution: { mode: NetworkDistributionMode };
	transportTypeComparison: { threshold: NetworkTransportComparisonThreshold };
};

type NetworkFilterDraft = BaseStatisticsFilterDraft;

const defaultNetworkMetricScope = (): NetworkMetricScope => ({
	eventTimeSeries: { bucket: StatisticsBucket.DAY, perspective: "customer" },
	journeyTimeSeries: { bucket: StatisticsBucket.DAY },
	weekdayHourHeatmap: { metric: "reliability5" },
	delaySeverity: { view: "delayed" },
	hourlyProfile: { dayGroup: "all", perspective: "customer", threshold: "under6" },
	mapHotspots: { metric: "reliability5" },
	delayDistribution: { mode: "histogram" },
	transportTypeComparison: { threshold: "under6" }
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

	setDelaySeverityView = (view: NetworkDelaySeverityView) => {
		this.network.delaySeverity.view = view;
	};

	setHourlyProfileDayGroup = (dayGroup: NetworkHourlyProfileDayGroup) => {
		this.network.hourlyProfile.dayGroup = dayGroup;
	};

	setHourlyProfilePerspective = (perspective: NetworkPunctualityPerspective) => {
		this.network.hourlyProfile.perspective = perspective;
	};

	setHourlyProfileThreshold = (threshold: NetworkTransportComparisonThreshold) => {
		this.network.hourlyProfile.threshold = threshold;
	};

	setMapMetric = (metric: NetworkMapMetric) => {
		this.network.mapHotspots.metric = metric;
	};

	setDistributionMode = (mode: NetworkDistributionMode) => {
		this.network.delayDistribution.mode = mode;
	};

	setTransportComparisonThreshold = (threshold: NetworkTransportComparisonThreshold) => {
		this.network.transportTypeComparison.threshold = threshold;
	};
}

const [getNetworkStatisticsContext, setNetworkStatisticsContext] = createContext<NetworkStatisticsContext>();

export {
	NetworkStatisticsContext,
	getNetworkStatisticsContext,
	setNetworkStatisticsContext,
	type NetworkDistributionMode,
	type NetworkDelaySeverityView,
	type NetworkFilterDraft,
	type NetworkHeatmapMetric,
	type NetworkHourlyProfileDayGroup,
	type NetworkMapMetric,
	type NetworkPunctualityPerspective,
	type NetworkTransportComparisonThreshold
};
