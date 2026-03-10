import { type MetricSeries } from "@lib/api";

const calculateChange = (metrics: MetricSeries[]): { percentage: number; isUp: boolean } => {
	const endValue = metrics.reduce((a, metric: MetricSeries) => {
		const value = Number(metric.dataPoints[metric.dataPoints.length - 1].value);
		return a + (isNaN(value) ? 0 : value);
	}, 0);
	const startValue = metrics.reduce((a, metric: MetricSeries) => {
		const value = Number(metric.dataPoints[0].value);
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

export { calculateChange };
