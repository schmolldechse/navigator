import {
	ScheduleType,
	StatisticsBucket,
	TransportType,
	type EventMetrics,
	type JourneyMetrics,
	type LineStatisticsMetricRequest,
	type MetricPage,
	type NetworkMapHotspotsRequest,
	type NetworkStatisticsMetricRequest,
	type StationStatisticsMetricRequest
} from "@lib/api";
import { DateTime } from "luxon";

type StatisticsGlobalScope = {
	from: string;
	to: string;
	transportTypes: TransportType[];
	includeReplacement: boolean;
};

type StatisticsEventScope = {
	scheduleType: ScheduleType | null;
};

type PagedMetricScope = {
	limit: number;
	offset: number;
	minVolume: number;
};

type BucketMetricScope = {
	bucket: StatisticsBucket;
};

type NetworkHeatmapMetric = "reliability5" | "reliability15" | "cancellation" | "plannedStops";
type NetworkComparisonMetric = "plannedStops" | "cancellationRate" | "reliability5" | "reliability15" | "journeyCompletion";
type NetworkDistributionMode = "histogram" | "cdf";

type NetworkMetricScope = {
	eventTimeSeries: BucketMetricScope;
	journeyTimeSeries: BucketMetricScope;
	weekdayHourHeatmap: {
		metric: NetworkHeatmapMetric;
	};
	transportTypeComparison: {
		metric: NetworkComparisonMetric;
	};
	stationRanking: PagedMetricScope;
	lineRanking: PagedMetricScope;
	mapHotspots: {
		minVolume: number;
		limit: number;
	};
	delayDistribution: {
		mode: NetworkDistributionMode;
	};
};

type StationMetricScope = {
	timeSeries: BucketMetricScope;
	lineRanking: PagedMetricScope;
	eventDetails: Pick<PagedMetricScope, "limit" | "offset">;
};

type StatisticsFilterDraft = {
	from: string;
	to: string;
	scheduleType: ScheduleType | null;
	transportTypes: TransportType[];
	includeReplacement: boolean;
	minVolume: number;
};

type TransportTypeOption = {
	value: TransportType;
	label: string;
	shortLabel: string;
	description?: string;
};

type StatisticsBucketOption = {
	value: StatisticsBucket;
	label: string;
};

type ScheduleTypeOption = {
	value: ScheduleType | null;
	label: string;
};

const DEFAULT_RANGE_DAYS = 30;
const DEFAULT_MIN_VOLUME = 50;
const DEFAULT_MAP_MIN_VOLUME = 1000;
const DEFAULT_MAP_LIMIT = 250;
const DEFAULT_PAGE_LIMIT = 10;
const DEFAULT_EVENT_DETAIL_LIMIT = 30;
const SMALL_RANGE_MAX_DAYS = 7;

const transportTypeOptions: TransportTypeOption[] = [
	{
		value: TransportType.HIGH_SPEED_TRAIN,
		label: "High speed",
		shortLabel: "ICE",
		description: "Long-distance express services."
	},
	{
		value: TransportType.INTERCITY_TRAIN,
		label: "Intercity",
		shortLabel: "IC",
		description: "Long-distance intercity services."
	},
	{
		value: TransportType.INTER_REGIONAL_TRAIN,
		label: "Inter-regional",
		shortLabel: "IR",
		description: "Inter-regional rail services."
	},
	{ value: TransportType.REGIONAL_TRAIN, label: "Regional", shortLabel: "RE/RB", description: "Regional rail services." },
	{ value: TransportType.CITY_TRAIN, label: "City train", shortLabel: "S", description: "Suburban rail services." },
	{ value: TransportType.SUBWAY, label: "Subway", shortLabel: "U" },
	{ value: TransportType.TRAM, label: "Tram", shortLabel: "Tram" },
	{ value: TransportType.BUS, label: "Bus", shortLabel: "Bus" }
];

const networkTransportTypeOptions: TransportTypeOption[] = transportTypeOptions.filter(
	(option) => option.value !== TransportType.SUBWAY && option.value !== TransportType.TRAM && option.value !== TransportType.BUS
);

const statisticsBucketOptions: StatisticsBucketOption[] = [
	{ value: StatisticsBucket.HOUR, label: "Hour" },
	{ value: StatisticsBucket.DAY, label: "Day" },
	{ value: StatisticsBucket.WEEK, label: "Week" },
	{ value: StatisticsBucket.MONTH, label: "Month" }
];

const defaultStatisticsBucketOptions: StatisticsBucketOption[] = statisticsBucketOptions.filter(
	(option) => option.value !== StatisticsBucket.HOUR
);

const scheduleTypeOptions: ScheduleTypeOption[] = [
	{ value: null, label: "All events" },
	{ value: ScheduleType.ARRIVAL, label: "Arrivals" },
	{ value: ScheduleType.DEPARTURE, label: "Departures" }
];

const numberFormat = new Intl.NumberFormat("en-US", { maximumFractionDigits: 0 });
const compactNumberFormat = new Intl.NumberFormat("en-US", {
	notation: "compact",
	maximumFractionDigits: 1
});
const percentFormat = new Intl.NumberFormat("en-US", {
	style: "percent",
	maximumFractionDigits: 1
});
const decimalFormat = new Intl.NumberFormat("en-US", {
	maximumFractionDigits: 1
});

const asLocalDate = (date: DateTime): string => date.toISO({ suppressMilliseconds: true }) ?? "";

const defaultGlobalScope = (): StatisticsGlobalScope => {
	const today = DateTime.now().startOf("day");

	return {
		from: asLocalDate(today.minus({ days: DEFAULT_RANGE_DAYS })),
		to: asLocalDate(today.plus({ days: 1 })),
		transportTypes: [],
		includeReplacement: true
	};
};

const defaultEventScope = (): StatisticsEventScope => ({
	scheduleType: null
});

const defaultNetworkMetricScope = (): NetworkMetricScope => ({
	eventTimeSeries: { bucket: StatisticsBucket.DAY },
	journeyTimeSeries: { bucket: StatisticsBucket.DAY },
	weekdayHourHeatmap: { metric: "reliability5" },
	transportTypeComparison: { metric: "reliability5" },
	stationRanking: { limit: DEFAULT_PAGE_LIMIT, offset: 0, minVolume: DEFAULT_MIN_VOLUME },
	lineRanking: { limit: DEFAULT_PAGE_LIMIT, offset: 0, minVolume: Math.max(5, Math.floor(DEFAULT_MIN_VOLUME / 4)) },
	mapHotspots: { minVolume: DEFAULT_MAP_MIN_VOLUME, limit: DEFAULT_MAP_LIMIT },
	delayDistribution: { mode: "histogram" }
});

const defaultStationMetricScope = (): StationMetricScope => ({
	timeSeries: { bucket: StatisticsBucket.DAY },
	lineRanking: { limit: DEFAULT_PAGE_LIMIT, offset: 0, minVolume: Math.max(5, Math.floor(DEFAULT_MIN_VOLUME / 4)) },
	eventDetails: { limit: DEFAULT_EVENT_DETAIL_LIMIT, offset: 0 }
});

const createFilterDraft = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	networkScope: NetworkMetricScope
): StatisticsFilterDraft => ({
	from: globalScope.from,
	to: globalScope.to,
	scheduleType: eventScope.scheduleType,
	transportTypes: [...globalScope.transportTypes],
	includeReplacement: globalScope.includeReplacement,
	minVolume: networkScope.stationRanking.minVolume
});

const normalizeDateRange = (from: string, to: string): Pick<StatisticsGlobalScope, "from" | "to"> => {
	const fromDate = DateTime.fromISO(from, { setZone: true });
	const toDate = DateTime.fromISO(to, { setZone: true });

	if (fromDate.isValid && toDate.isValid && toDate > fromDate) return { from, to };
	if (fromDate.isValid) return { from, to: asLocalDate(fromDate.plus({ days: 1 })) };

	return defaultGlobalScope();
};

const getRangeDays = ({ from, to }: Pick<StatisticsGlobalScope, "from" | "to">): number => {
	const fromDate = DateTime.fromISO(from, { setZone: true });
	const toDate = DateTime.fromISO(to, { setZone: true });
	if (!fromDate.isValid || !toDate.isValid || toDate <= fromDate) return DEFAULT_RANGE_DAYS;

	return Math.ceil(toDate.diff(fromDate, "days").days);
};

const isSmallRange = (scope: Pick<StatisticsGlobalScope, "from" | "to">): boolean =>
	getRangeDays(scope) <= SMALL_RANGE_MAX_DAYS;

const getStatisticsBucketOptions = (scope: Pick<StatisticsGlobalScope, "from" | "to">): StatisticsBucketOption[] =>
	isSmallRange(scope) ? statisticsBucketOptions : defaultStatisticsBucketOptions;

const normalizeBucketForRange = (
	bucket: StatisticsBucket,
	scope: Pick<StatisticsGlobalScope, "from" | "to">
): StatisticsBucket => (bucket === StatisticsBucket.HOUR && !isSmallRange(scope) ? StatisticsBucket.DAY : bucket);

const createPreviousGlobalScope = (scope: StatisticsGlobalScope): StatisticsGlobalScope => {
	const range = normalizeDateRange(scope.from, scope.to);
	const fromDate = DateTime.fromISO(range.from, { setZone: true });
	const toDate = DateTime.fromISO(range.to, { setZone: true });
	const days = Math.max(1, Math.ceil(toDate.diff(fromDate, "days").days));

	return {
		...scope,
		from: asLocalDate(fromDate.minus({ days })),
		to: asLocalDate(fromDate)
	};
};

const eventRequestBase = (globalScope: StatisticsGlobalScope, eventScope: StatisticsEventScope) => ({
	from: globalScope.from,
	to: globalScope.to,
	scheduleType: eventScope.scheduleType,
	transportTypes: globalScope.transportTypes.length > 0 ? [...globalScope.transportTypes] : undefined,
	includeReplacement: globalScope.includeReplacement
});

const journeyRequestBase = (globalScope: StatisticsGlobalScope) => ({
	from: globalScope.from,
	to: globalScope.to,
	transportTypes: globalScope.transportTypes.length > 0 ? [...globalScope.transportTypes] : undefined,
	includeReplacement: globalScope.includeReplacement
});

const createNetworkEventSummaryRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope
): NetworkStatisticsMetricRequest =>
	({
		...eventRequestBase(globalScope, eventScope),
		type: "EVENT_SUMMARY"
	}) as NetworkStatisticsMetricRequest;

const createNetworkJourneySummaryRequest = (globalScope: StatisticsGlobalScope): NetworkStatisticsMetricRequest =>
	({
		...journeyRequestBase(globalScope),
		type: "JOURNEY_SUMMARY"
	}) as NetworkStatisticsMetricRequest;

const createNetworkEventTimeSeriesRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	scope: BucketMetricScope
): NetworkStatisticsMetricRequest =>
	({
		...eventRequestBase(globalScope, eventScope),
		type: "EVENT_TIME_SERIES",
		bucket: scope.bucket
	}) as NetworkStatisticsMetricRequest;

const createNetworkJourneyTimeSeriesRequest = (
	globalScope: StatisticsGlobalScope,
	scope: BucketMetricScope
): NetworkStatisticsMetricRequest =>
	({
		...journeyRequestBase(globalScope),
		type: "JOURNEY_TIME_SERIES",
		bucket: scope.bucket
	}) as NetworkStatisticsMetricRequest;

const createNetworkWeekdayHourHeatmapRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope
): NetworkStatisticsMetricRequest =>
	({
		...eventRequestBase(globalScope, eventScope),
		type: "WEEKDAY_HOUR_HEATMAP"
	}) as NetworkStatisticsMetricRequest;

const createNetworkEventDelayDistributionRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope
): NetworkStatisticsMetricRequest =>
	({
		...eventRequestBase(globalScope, eventScope),
		type: "EVENT_DELAY_DISTRIBUTION"
	}) as NetworkStatisticsMetricRequest;

const createNetworkTransportTypeComparisonRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope
): NetworkStatisticsMetricRequest =>
	({
		...eventRequestBase(globalScope, eventScope),
		type: "TRANSPORT_TYPE_COMPARISON"
	}) as NetworkStatisticsMetricRequest;

const createNetworkStationRankingRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	scope: PagedMetricScope
): NetworkStatisticsMetricRequest =>
	({
		...eventRequestBase(globalScope, eventScope),
		type: "STATION_RANKING",
		limit: scope.limit,
		offset: scope.offset,
		minVolume: scope.minVolume
	}) as NetworkStatisticsMetricRequest;

const createNetworkLineRankingRequest = (
	globalScope: StatisticsGlobalScope,
	scope: PagedMetricScope
): NetworkStatisticsMetricRequest =>
	({
		...journeyRequestBase(globalScope),
		type: "LINE_RANKING",
		limit: scope.limit,
		offset: scope.offset,
		minVolume: scope.minVolume
	}) as NetworkStatisticsMetricRequest;

const createNetworkMapHotspotsRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	scope: NetworkMetricScope["mapHotspots"]
): NetworkMapHotspotsRequest =>
	({
		...eventRequestBase(globalScope, eventScope),
		minVolume: scope.minVolume,
		limit: scope.limit
	}) as NetworkMapHotspotsRequest;

const createStationEventBase = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number
) => ({
	...eventRequestBase(globalScope, eventScope),
	stationEvaNumber
});

const createStationEventSummaryRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "EVENT_SUMMARY"
	}) as StationStatisticsMetricRequest;

const createStationBenchmarkRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "BENCHMARK"
	}) as StationStatisticsMetricRequest;

const createStationTimeSeriesRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number,
	scope: BucketMetricScope
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "TIME_SERIES",
		bucket: scope.bucket
	}) as StationStatisticsMetricRequest;

const createStationArrivalDepartureComparisonRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "ARRIVAL_DEPARTURE_COMPARISON"
	}) as StationStatisticsMetricRequest;

const createStationWeekdayHourHeatmapRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "WEEKDAY_HOUR_HEATMAP"
	}) as StationStatisticsMetricRequest;

const createStationLineRankingRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number,
	scope: PagedMetricScope
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "LINE_RANKING",
		limit: scope.limit,
		offset: scope.offset,
		minVolume: scope.minVolume
	}) as StationStatisticsMetricRequest;

const createStationDirectionsRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "DIRECTIONS"
	}) as StationStatisticsMetricRequest;

const createStationTransportTypeMixRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "TRANSPORT_TYPE_MIX"
	}) as StationStatisticsMetricRequest;

const createStationLineHourMatrixRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "LINE_HOUR_MATRIX"
	}) as StationStatisticsMetricRequest;

const createStationEventDetailsRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope,
	stationEvaNumber: number,
	scope: StationMetricScope["eventDetails"]
): StationStatisticsMetricRequest =>
	({
		...createStationEventBase(globalScope, eventScope, stationEvaNumber),
		type: "EVENT_DETAILS",
		limit: scope.limit,
		offset: scope.offset
	}) as StationStatisticsMetricRequest;

const createLineMinVolume = (minVolume: number): number => Math.max(5, Math.floor(minVolume / 4));

const expectMetricResult = <TResult = unknown>(response: { result: { type?: string } }, type: string): TResult => {
	if (response.result.type && response.result.type !== type) {
		throw new Error(`Expected ${type}, received ${response.result.type}.`);
	}

	return response.result as TResult;
};

const toNumber = (value: number | string | null | undefined): number | null => {
	if (value === null || value === undefined || value === "") return null;

	const numberValue = Number(value);
	return Number.isFinite(numberValue) ? numberValue : null;
};

const pageValue = (value: number | string): number => Number(value) || 0;

const pageTotalPages = (page: MetricPage): number => {
	const limit = Math.max(1, pageValue(page.limit));
	return Math.ceil(pageValue(page.totalItems) / limit);
};

const pageHasMore = (page: MetricPage): boolean => pageValue(page.offset) + pageValue(page.limit) < pageValue(page.totalItems);

const formatCount = (value: number | string | null | undefined, compact = false): string => {
	const numberValue = toNumber(value);
	if (numberValue === null) return "-";

	return compact ? compactNumberFormat.format(numberValue) : numberFormat.format(numberValue);
};

const formatRate = (value: number | string | null | undefined): string => {
	const numberValue = toNumber(value);
	if (numberValue === null) return "-";

	return percentFormat.format(numberValue);
};

const formatRateDelta = (value: number | string | null | undefined): string => {
	const numberValue = toNumber(value);
	if (numberValue === null) return "-";

	return `${numberValue >= 0 ? "+" : ""}${(numberValue * 100).toFixed(1)} pp`;
};

const formatSeconds = (value: number | string | null | undefined): string => {
	const numberValue = toNumber(value);
	if (numberValue === null) return "-";
	const absValue = Math.abs(numberValue);

	if (absValue < 60) return `${decimalFormat.format(numberValue)} s`;
	return `${decimalFormat.format(numberValue / 60)} min`;
};

const formatMinutes = (value: number | string | null | undefined, compact = false): string => {
	const numberValue = toNumber(value);
	if (numberValue === null) return "-";

	return `${compact ? compactNumberFormat.format(numberValue) : decimalFormat.format(numberValue)} min`;
};

const formatMetric = (
	value: number | string | null | undefined,
	kind: "count" | "rate" | "rateDelta" | "seconds" | "minutes",
	compact = false
) => {
	switch (kind) {
		case "count":
			return formatCount(value, compact);
		case "rate":
			return formatRate(value);
		case "rateDelta":
			return formatRateDelta(value);
		case "seconds":
			return formatSeconds(value);
		case "minutes":
			return formatMinutes(value, compact);
	}
};

const rateToPercent = (value: number | string | null | undefined): number | null => {
	const numberValue = toNumber(value);
	return numberValue === null ? null : numberValue * 100;
};

const transportTypeLabel = (transportType: TransportType | string | null | undefined): string => {
	const option = transportTypeOptions.find((item) => item.value === transportType);
	if (option) return option.label;

	return String(transportType ?? "Unknown")
		.toLowerCase()
		.split("_")
		.map((part) => part.charAt(0).toUpperCase() + part.slice(1))
		.join(" ");
};

const transportTypeShortLabel = (transportType: TransportType | string | null | undefined): string => {
	const option = transportTypeOptions.find((item) => item.value === transportType);
	return option?.shortLabel ?? transportTypeLabel(transportType);
};

const formatLocalDateTime = (value: string | null | undefined): string => {
	if (!value) return "-";
	const parsed = DateTime.fromISO(value, { setZone: true });
	if (!parsed.isValid) return value;

	return parsed.toFormat("dd LLL, HH:mm");
};

const formatDate = (value: Date | string | null | undefined): string => {
	if (!value) return "-";
	const parsed = value instanceof Date ? DateTime.fromJSDate(value) : DateTime.fromISO(value, { setZone: true });
	if (!parsed.isValid) return String(value);

	return parsed.toFormat("dd LLL yyyy");
};

const createRangeLabel = ({ from, to }: Pick<StatisticsGlobalScope, "from" | "to">): string => {
	const fromDate = DateTime.fromISO(from, { setZone: true });
	const toDate = DateTime.fromISO(to, { setZone: true }).minus({ days: 1 });
	if (!fromDate.isValid || !toDate.isValid) return `${from} to ${to}`;

	return `${fromDate.toFormat("dd LLL yyyy")} - ${toDate.toFormat("dd LLL yyyy")}`;
};

const eventReliability = (metrics: EventMetrics): number | null => toNumber(metrics.customerReliability5Rate);
const eventCancellationRate = (metrics: EventMetrics): number | null => toNumber(metrics.cancellationRate);
const journeyCompletionRate = (metrics: JourneyMetrics): number | null => toNumber(metrics.journeyCompletionRate);

export {
	DEFAULT_MAP_LIMIT,
	DEFAULT_MAP_MIN_VOLUME,
	DEFAULT_MIN_VOLUME,
	DEFAULT_PAGE_LIMIT,
	SMALL_RANGE_MAX_DAYS,
	defaultStatisticsBucketOptions,
	networkTransportTypeOptions,
	scheduleTypeOptions,
	statisticsBucketOptions,
	transportTypeOptions,
	asLocalDate,
	createFilterDraft,
	createLineMinVolume,
	createNetworkEventDelayDistributionRequest,
	createNetworkEventSummaryRequest,
	createNetworkEventTimeSeriesRequest,
	createNetworkJourneySummaryRequest,
	createNetworkJourneyTimeSeriesRequest,
	createNetworkLineRankingRequest,
	createNetworkMapHotspotsRequest,
	createNetworkStationRankingRequest,
	createNetworkTransportTypeComparisonRequest,
	createNetworkWeekdayHourHeatmapRequest,
	createPreviousGlobalScope,
	createRangeLabel,
	createStationArrivalDepartureComparisonRequest,
	createStationBenchmarkRequest,
	createStationDirectionsRequest,
	createStationEventDetailsRequest,
	createStationEventSummaryRequest,
	createStationLineHourMatrixRequest,
	createStationLineRankingRequest,
	createStationTimeSeriesRequest,
	createStationTransportTypeMixRequest,
	createStationWeekdayHourHeatmapRequest,
	defaultEventScope,
	defaultGlobalScope,
	defaultNetworkMetricScope,
	defaultStationMetricScope,
	eventCancellationRate,
	eventReliability,
	expectMetricResult,
	formatCount,
	formatDate,
	formatLocalDateTime,
	formatMetric,
	formatMinutes,
	formatRate,
	formatRateDelta,
	formatSeconds,
	getRangeDays,
	getStatisticsBucketOptions,
	isSmallRange,
	journeyCompletionRate,
	normalizeBucketForRange,
	normalizeDateRange,
	pageHasMore,
	pageTotalPages,
	pageValue,
	rateToPercent,
	toNumber,
	transportTypeLabel,
	transportTypeShortLabel,
	type BucketMetricScope,
	type NetworkComparisonMetric,
	type NetworkDistributionMode,
	type NetworkHeatmapMetric,
	type NetworkMetricScope,
	type PagedMetricScope,
	type ScheduleTypeOption,
	type StatisticsBucketOption,
	type StatisticsEventScope,
	type StatisticsFilterDraft,
	type StatisticsGlobalScope,
	type StationMetricScope,
	type TransportTypeOption
};
