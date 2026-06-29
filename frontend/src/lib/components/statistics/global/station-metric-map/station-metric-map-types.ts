import type { BaseStation, MetricSample, MetricSeries } from "@lib/api";

type StationMetricMapPoint = {
	station: BaseStation;
	value: number;
	sample?: MetricSample | null;
};

type StationMetricMapSeries = {
	points: StationMetricMapPoint[];
} & Pick<MetricSeries, "unit" | "seriesType">;

type StationMetricMapMarkerData = {
	station: BaseStation;
	value: number;
	sample?: MetricSample | null;
} & Pick<MetricSeries, "unit" | "seriesType">;

export type { StationMetricMapPoint, StationMetricMapSeries, StationMetricMapMarkerData };
