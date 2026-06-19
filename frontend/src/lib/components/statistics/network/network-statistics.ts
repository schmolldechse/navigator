import type { NetworkMapHotspotsRequest, NetworkStatisticsMetricRequest } from "@lib/api";
import {
	eventRequestBase,
	journeyRequestBase,
	type BucketMetricScope,
	type StatisticsEventScope,
	type StatisticsGlobalScope
} from "../shared/statistics-dashboard";

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

const createNetworkMapHotspotsRequest = (
	globalScope: StatisticsGlobalScope,
	eventScope: StatisticsEventScope
): NetworkMapHotspotsRequest =>
	({
		...eventRequestBase(globalScope, eventScope),
		minVolume: 0
	}) as NetworkMapHotspotsRequest;

export {
	createNetworkEventDelayDistributionRequest,
	createNetworkEventSummaryRequest,
	createNetworkEventTimeSeriesRequest,
	createNetworkJourneySummaryRequest,
	createNetworkJourneyTimeSeriesRequest,
	createNetworkMapHotspotsRequest,
	createNetworkTransportTypeComparisonRequest,
	createNetworkWeekdayHourHeatmapRequest
};
