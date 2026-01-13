import { MetricSeriesType, type MetricSeries } from "@lib/api";

const isTrendAvailable = (metrics: MetricSeries[]): boolean =>
	metrics.some((metric: MetricSeries) => metric.seriesType !== MetricSeriesType.TRANSPORT_TYPES_TOTAL);

const calculateChange = (metrics: MetricSeries[]): { percentage: number; isUp: boolean } => {
	const endValue = metrics.reduce((a, metric: MetricSeries) => {
		const value = Number(metric.summary.endValue);
		return a + (isNaN(value) ? 0 : value);
	}, 0);
	const startValue = metrics.reduce((a, metric: MetricSeries) => {
		const value = Number(metric.summary.startValue);
		return a + (isNaN(value) ? 0 : value);
	}, 0);
	const changedBy = endValue - startValue;

	if (startValue === 0) return { percentage: endValue > 0 ? 100 : 0, isUp: endValue >= 0 };
	const percentage = (changedBy / startValue) * 100;

	return {
		percentage: Math.abs(percentage),
		isUp: changedBy >= 0
	};
};

export { isTrendAvailable, calculateChange };
