import { MetricSeriesType, MetricUnit, TransportType, type MetricSeries, type MetricSample } from "@lib/api";

type MetricPolarity = "positive" | "negative" | "neutral";

type AggregatedMetric = {
	value: number | null;
	unit: MetricUnit;
	sample?: MetricSample | null;
};

type TransportFamily = {
	id: string;
	label: string;
	transportTypes: TransportType[];
	color: string;
};

type TransportBreakdownRow = {
	family: TransportFamily;
	eventCount: number;
	value: number | null;
	unit: MetricUnit;
	sample?: MetricSample | null;
};

const TRANSPORT_FAMILIES: TransportFamily[] = [
	{
		id: "long-distance",
		label: "Long Distance",
		transportTypes: [TransportType.HIGH_SPEED_TRAIN, TransportType.INTERCITY_TRAIN],
		color: "var(--color-accent)"
	},
	{
		id: "regional",
		label: "Regional",
		transportTypes: [TransportType.INTER_REGIONAL_TRAIN, TransportType.REGIONAL_TRAIN],
		color: "rgb(20 184 166)"
	},
	{
		id: "suburban",
		label: "S-Bahn",
		transportTypes: [TransportType.CITY_TRAIN],
		color: "rgb(34 197 94)"
	},
	{
		id: "other",
		label: "Other",
		transportTypes: [],
		color: "rgb(148 163 184)"
	}
];

const RATE_OR_AVERAGE_SERIES = new Set<MetricSeriesType>([
	MetricSeriesType.STATION_EVENT_CANCELLATION_RATE,
	MetricSeriesType.STATION_EVENT_DELAY_AVERAGE,
	MetricSeriesType.STATION_EVENT_PUNCTUALITY5_RATE,
	MetricSeriesType.STATION_EVENT_PUNCTUALITY15_RATE
]);

const getMetricPolarity = (seriesType: MetricSeriesType): MetricPolarity => {
	if (
		[MetricSeriesType.STATION_EVENT_PUNCTUALITY5_RATE, MetricSeriesType.STATION_EVENT_PUNCTUALITY15_RATE].includes(seriesType)
	)
		return "positive";
	if (
		[
			MetricSeriesType.STATION_EVENT_CANCELLATION_COUNT,
			MetricSeriesType.STATION_EVENT_CANCELLATION_RATE,
			MetricSeriesType.STATION_EVENT_DELAY_AVERAGE
		].includes(seriesType)
	)
		return "negative";
	return "neutral";
};

const metricSupportsSampleWeighting = (seriesType: MetricSeriesType) => RATE_OR_AVERAGE_SERIES.has(seriesType);

const numberValue = (value: unknown): number => {
	const next = Number(value);
	return Number.isFinite(next) ? next : 0;
};

const aggregateMetricSeries = (
	metric: MetricSeries,
	predicate?: (dataPoint: MetricSeries["dataPoints"][number]) => boolean
) => {
	const dataPoints = predicate ? metric.dataPoints.filter(predicate) : metric.dataPoints;

	if (metricSupportsSampleWeighting(metric.seriesType)) {
		const sample = dataPoints.reduce(
			(total, dataPoint) => ({
				numerator: total.numerator + numberValue(dataPoint.sample?.numerator),
				denominator: total.denominator + numberValue(dataPoint.sample?.denominator)
			}),
			{ numerator: 0, denominator: 0 }
		);

		return {
			value:
				sample.denominator > 0
					? (metric.unit === MetricUnit.PERCENT ? 100 : 1) * (sample.numerator / sample.denominator)
					: null,
			unit: metric.unit,
			sample
		} satisfies AggregatedMetric;
	}

	return {
		value: dataPoints.reduce((sum: number, dataPoint) => sum + numberValue(dataPoint.value), 0),
		unit: metric.unit
	} satisfies AggregatedMetric;
};

const findTransportFamily = (transportType: TransportType): TransportFamily =>
	TRANSPORT_FAMILIES.find((family) => family.transportTypes.includes(transportType)) ??
	TRANSPORT_FAMILIES[TRANSPORT_FAMILIES.length - 1];

const createTransportBreakdownRows = (metric: MetricSeries, eventCountMetric: MetricSeries): TransportBreakdownRow[] =>
	TRANSPORT_FAMILIES.map((family) => {
		const matchesFamily = (dataPoint: MetricSeries["dataPoints"][number]) => {
			if (!("transportType" in dataPoint)) return false;
			const transportType = dataPoint.transportType as TransportType;
			return findTransportFamily(transportType).id === family.id;
		};

		const aggregate = aggregateMetricSeries(metric, matchesFamily);
		const eventAggregate = aggregateMetricSeries(eventCountMetric, matchesFamily);

		return {
			family,
			eventCount: eventAggregate.value ?? 0,
			value: aggregate.value,
			unit: aggregate.unit,
			sample: aggregate.sample
		};
	}).filter((row) => row.eventCount > 0 || row.value !== null);

export {
	type AggregatedMetric,
	type MetricPolarity,
	type TransportBreakdownRow,
	type TransportFamily,
	TRANSPORT_FAMILIES,
	aggregateMetricSeries,
	createTransportBreakdownRows,
	getMetricPolarity,
	metricSupportsSampleWeighting,
	numberValue
};
