import type { StationStatisticsMetricRequest } from "@lib/api";
import type { StationMetricScope } from "./station-context.svelte";
import {
	eventRequestBase,
	type BucketMetricScope,
	type StatisticsEventScope,
	type StatisticsGlobalScope
} from "../shared/statistics-dashboard";

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
	scope: StationMetricScope["lineRanking"]
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

export {
	createStationArrivalDepartureComparisonRequest,
	createStationBenchmarkRequest,
	createStationDirectionsRequest,
	createStationEventDetailsRequest,
	createStationEventSummaryRequest,
	createStationLineHourMatrixRequest,
	createStationLineRankingRequest,
	createStationTimeSeriesRequest,
	createStationTransportTypeMixRequest,
	createStationWeekdayHourHeatmapRequest
};
