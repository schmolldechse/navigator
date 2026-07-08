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

type PagedMetricScope = {
	limit: number;
	offset: number;
	minVolume: number;
};

type StationMetricScope = {
	rankingMinVolume: number;
	timeSeries: { bucket: StatisticsBucket };
	lineRanking: PagedMetricScope;
	eventDetails: Pick<PagedMetricScope, "limit" | "offset">;
};

type StationFilterDraft = BaseStatisticsFilterDraft & {
	minVolume: number;
};

const DEFAULT_MIN_VOLUME = 50;
const DEFAULT_PAGE_LIMIT = 10;
const DEFAULT_EVENT_DETAIL_LIMIT = 30;

const createLineMinVolume = (minVolume: number): number => Math.max(5, Math.floor(minVolume / 4));

const defaultStationMetricScope = (): StationMetricScope => ({
	rankingMinVolume: DEFAULT_MIN_VOLUME,
	timeSeries: { bucket: StatisticsBucket.DAY },
	lineRanking: { limit: DEFAULT_PAGE_LIMIT, offset: 0, minVolume: createLineMinVolume(DEFAULT_MIN_VOLUME) },
	eventDetails: { limit: DEFAULT_EVENT_DETAIL_LIMIT, offset: 0 }
});

const createStationFilterDraft = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationScope: StationMetricScope
): StationFilterDraft => ({
	from: globalScope.from,
	to: globalScope.to,
	scheduleType: eventScope.scheduleType,
	transportTypes: [...globalScope.transportTypes],
	includeReplacement: globalScope.includeReplacement,
	minVolume: stationScope.rankingMinVolume
});

class StationStatisticsContext {
	global: StatisticsGlobalScope = $state(defaultGlobalScope());
	event: StatisticsEventScope = $state(defaultEventScope());
	station: StationMetricScope = $state(defaultStationMetricScope());

	get filterDraft(): StationFilterDraft {
		return createStationFilterDraft(this.global, this.event, this.station);
	}

	applyFilters = (draft: StationFilterDraft) => {
		const range = normalizeDateRange(draft.from, draft.to);

		this.global.from = range.from;
		this.global.to = range.to;
		this.global.transportTypes = [...draft.transportTypes];
		this.global.includeReplacement = draft.includeReplacement;
		this.event.scheduleType = draft.scheduleType;
		this.station.rankingMinVolume = draft.minVolume;
		this.station.lineRanking.minVolume = createLineMinVolume(draft.minVolume);
		this.station.timeSeries.bucket = normalizeBucketForRange(this.station.timeSeries.bucket, this.global);
		this.resetPagedOffsets();
	};

	resetFilters = () => {
		Object.assign(this.global, defaultGlobalScope());
		Object.assign(this.event, defaultEventScope());
		Object.assign(this.station, defaultStationMetricScope());
	};

	setTimeSeriesBucket = (bucket: StatisticsBucket) => {
		this.station.timeSeries.bucket = normalizeBucketForRange(bucket, this.global);
	};

	setLineRankingOffset = (offset: number) => {
		this.station.lineRanking.offset = normalizeOffset(offset, this.station.lineRanking);
	};

	setEventDetailsOffset = (offset: number) => {
		this.station.eventDetails.offset = Math.max(0, offset);
	};

	private resetPagedOffsets = () => {
		this.station.lineRanking.offset = 0;
		this.station.eventDetails.offset = 0;
	};
}

const normalizeOffset = (offset: number, scope: Pick<PagedMetricScope, "limit">): number => {
	const limit = Math.max(1, scope.limit);
	return Math.max(0, Math.floor(offset / limit) * limit);
};

const [getStationStatisticsContext, setStationStatisticsContext] = createContext<StationStatisticsContext>();

export {
	StationStatisticsContext,
	getStationStatisticsContext,
	setStationStatisticsContext,
	type StationFilterDraft,
	type StationMetricScope
};
