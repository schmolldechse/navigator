import {
	ScheduleType,
	StatisticsBucket,
	TransportType,
	type EventMetrics,
	type JourneyMetrics,
	type MetricPage
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

type BucketMetricScope = {
	bucket: StatisticsBucket;
};

type BaseStatisticsFilterDraft = {
	from: string;
	to: string;
	scheduleType: ScheduleType | null;
	transportTypes: TransportType[];
	includeReplacement: boolean;
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

const smallRangeBucketOptions: StatisticsBucketOption[] = statisticsBucketOptions.filter(
	(option) => option.value !== StatisticsBucket.MONTH
);

const defaultStatisticsBucketOptions: StatisticsBucketOption[] = statisticsBucketOptions.filter(
	(option) => option.value === StatisticsBucket.DAY || option.value === StatisticsBucket.WEEK
);

const longRangeBucketOptions: StatisticsBucketOption[] = statisticsBucketOptions.filter(
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

const isLongRange = (scope: Pick<StatisticsGlobalScope, "from" | "to">): boolean => {
	const fromDate = DateTime.fromISO(scope.from, { setZone: true });
	const toDate = DateTime.fromISO(scope.to, { setZone: true });
	if (!fromDate.isValid || !toDate.isValid || toDate <= fromDate) return false;

	return toDate > fromDate.plus({ months: 2 });
};

const getStatisticsBucketOptions = (scope: Pick<StatisticsGlobalScope, "from" | "to">): StatisticsBucketOption[] => {
	if (isSmallRange(scope)) return smallRangeBucketOptions;
	if (isLongRange(scope)) return longRangeBucketOptions;
	return defaultStatisticsBucketOptions;
};

const normalizeBucketForRange = (
	bucket: StatisticsBucket,
	scope: Pick<StatisticsGlobalScope, "from" | "to">
): StatisticsBucket => {
	const options = getStatisticsBucketOptions(scope);
	return options.some((option) => option.value === bucket) ? bucket : StatisticsBucket.DAY;
};

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

const formatStatisticsBucketLabel = (value: Date | string | null | undefined, bucket: StatisticsBucket): string => {
	if (!value) return "-";
	const parsed = value instanceof Date ? DateTime.fromJSDate(value) : DateTime.fromISO(value, { setZone: true });
	if (!parsed.isValid) return String(value);

	switch (bucket) {
		case StatisticsBucket.HOUR:
			return parsed.toFormat("dd LLL yyyy, HH:mm");
		case StatisticsBucket.WEEK: {
			const end = parsed.plus({ days: 6 });
			const startLabel = parsed.year === end.year ? parsed.toFormat("dd LLL") : parsed.toFormat("dd LLL yyyy");
			return `CW ${parsed.weekNumber}/${parsed.weekYear} · ${startLabel} - ${end.toFormat("dd LLL yyyy")}`;
		}
		case StatisticsBucket.MONTH:
			return parsed.toFormat("LLLL yyyy");
		case StatisticsBucket.DAY:
		default:
			return parsed.toFormat("dd LLL yyyy");
	}
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
	SMALL_RANGE_MAX_DAYS,
	defaultStatisticsBucketOptions,
	networkTransportTypeOptions,
	scheduleTypeOptions,
	statisticsBucketOptions,
	transportTypeOptions,
	asLocalDate,
	createPreviousGlobalScope,
	createRangeLabel,
	defaultEventScope,
	defaultGlobalScope,
	eventRequestBase,
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
	formatStatisticsBucketLabel,
	getRangeDays,
	getStatisticsBucketOptions,
	isSmallRange,
	journeyRequestBase,
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
	type BaseStatisticsFilterDraft,
	type ScheduleTypeOption,
	type StatisticsBucketOption,
	type StatisticsEventScope,
	type StatisticsGlobalScope,
	type TransportTypeOption
};
