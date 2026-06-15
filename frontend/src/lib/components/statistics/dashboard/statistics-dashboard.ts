import {
	ScheduleType,
	StatisticsBucket,
	TransportType,
	type EventMetrics,
	type JourneyMetrics,
	type NetworkMapHotspotsRequest,
	type NetworkStatisticsMetricRequest,
	type StationStatisticsMetricRequest,
	type StatisticsMetricResponse,
	type StatisticsMetricResult
} from "@lib/api";
import { DateTime } from "luxon";

type StatisticsFilterState = {
	from: string;
	to: string;
	bucket: StatisticsBucket;
	scheduleType: ScheduleType | null;
	transportTypes: TransportType[];
	includeReplacement: boolean;
	minVolume: number;
};

type TransportTypeOption = {
	value: TransportType;
	label: string;
	shortLabel: string;
};

type StatisticsBucketOption = {
	value: StatisticsBucket;
	label: string;
};

type ScheduleTypeOption = {
	value: ScheduleType | null;
	label: string;
};

const SERVICE_ZONE = "Europe/Berlin";
const DEFAULT_RANGE_DAYS = 30;
const DEFAULT_MIN_VOLUME = 50;

const transportTypeOptions: TransportTypeOption[] = [
	{ value: TransportType.HIGH_SPEED_TRAIN, label: "High speed", shortLabel: "ICE" },
	{ value: TransportType.INTERCITY_TRAIN, label: "Intercity", shortLabel: "IC" },
	{ value: TransportType.INTER_REGIONAL_TRAIN, label: "Inter-regional", shortLabel: "IR" },
	{ value: TransportType.REGIONAL_TRAIN, label: "Regional", shortLabel: "RE/RB" },
	{ value: TransportType.CITY_TRAIN, label: "City train", shortLabel: "S" },
	{ value: TransportType.SUBWAY, label: "Subway", shortLabel: "U" },
	{ value: TransportType.TRAM, label: "Tram", shortLabel: "Tram" },
	{ value: TransportType.BUS, label: "Bus", shortLabel: "Bus" }
];

const statisticsBucketOptions: StatisticsBucketOption[] = [
	{ value: StatisticsBucket.DAY, label: "Day" },
	{ value: StatisticsBucket.WEEK, label: "Week" },
	{ value: StatisticsBucket.MONTH, label: "Month" }
];

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

const asLocalDate = (date: DateTime): string => date.setZone(SERVICE_ZONE).toISODate() ?? "";

const isIsoDate = (value: string | null): value is string =>
	!!value && /^\d{4}-\d{2}-\d{2}$/.test(value) && DateTime.fromISO(value, { zone: SERVICE_ZONE }).isValid;

const defaultFilters = (): StatisticsFilterState => {
	const today = DateTime.now().setZone(SERVICE_ZONE).startOf("day");

	return {
		from: asLocalDate(today.minus({ days: DEFAULT_RANGE_DAYS })),
		to: asLocalDate(today.plus({ days: 1 })),
		bucket: StatisticsBucket.DAY,
		scheduleType: null,
		transportTypes: [],
		includeReplacement: true,
		minVolume: DEFAULT_MIN_VOLUME
	};
};

const parseEnumValue = <T extends string>(value: string | null, values: readonly T[], fallback: T): T =>
	value && values.includes(value as T) ? (value as T) : fallback;

const parseOptionalEnumValue = <T extends string>(value: string | null, values: readonly T[]): T | null =>
	value && values.includes(value as T) ? (value as T) : null;

const parseTransportTypes = (searchParams: URLSearchParams): TransportType[] => {
	const rawValues = searchParams
		.getAll("transportType")
		.flatMap((value) => value.split(","))
		.map((value) => value.trim())
		.filter(Boolean);
	const allowedValues = new Set(Object.values(TransportType));

	return [...new Set(rawValues)].filter((value): value is TransportType => allowedValues.has(value as TransportType));
};

const normalizeDateRange = (from: string, to: string): Pick<StatisticsFilterState, "from" | "to"> => {
	const fromDate = DateTime.fromISO(from, { zone: SERVICE_ZONE });
	const toDate = DateTime.fromISO(to, { zone: SERVICE_ZONE });

	if (toDate > fromDate) return { from, to };

	return {
		from,
		to: asLocalDate(fromDate.plus({ days: 1 }))
	};
};

const createStatisticsFiltersFromSearchParams = (searchParams: URLSearchParams): StatisticsFilterState => {
	const fallback = defaultFilters();
	const range = normalizeDateRange(
		isIsoDate(searchParams.get("from")) ? searchParams.get("from")! : fallback.from,
		isIsoDate(searchParams.get("to")) ? searchParams.get("to")! : fallback.to
	);
	const minVolume = Number(searchParams.get("minVolume"));

	return {
		...fallback,
		...range,
		bucket: parseEnumValue(searchParams.get("bucket"), Object.values(StatisticsBucket), fallback.bucket),
		scheduleType: parseOptionalEnumValue(searchParams.get("scheduleType"), Object.values(ScheduleType)),
		transportTypes: parseTransportTypes(searchParams),
		includeReplacement: searchParams.get("includeReplacement") !== "false",
		minVolume: Number.isInteger(minVolume) && minVolume >= 0 ? minVolume : fallback.minVolume
	};
};

const createSearchParamsFromStatisticsFilters = (filters: StatisticsFilterState): URLSearchParams => {
	const searchParams = new URLSearchParams();

	searchParams.set("from", filters.from);
	searchParams.set("to", filters.to);
	searchParams.set("bucket", filters.bucket);
	if (filters.scheduleType) searchParams.set("scheduleType", filters.scheduleType);
	for (const transportType of filters.transportTypes) searchParams.append("transportType", transportType);
	searchParams.set("includeReplacement", String(filters.includeReplacement));
	searchParams.set("minVolume", String(filters.minVolume));

	return searchParams;
};

const eventRequestBase = (filters: StatisticsFilterState) => ({
	from: filters.from,
	to: filters.to,
	scheduleType: filters.scheduleType,
	transportTypes: filters.transportTypes.length > 0 ? filters.transportTypes : undefined,
	includeReplacement: filters.includeReplacement
});

const journeyRequestBase = (filters: StatisticsFilterState) => ({
	from: filters.from,
	to: filters.to,
	transportTypes: filters.transportTypes.length > 0 ? filters.transportTypes : undefined,
	includeReplacement: filters.includeReplacement
});

const createNetworkDashboardRequests = (filters: StatisticsFilterState) => ({
	eventKpis: {
		...eventRequestBase(filters),
		type: "EVENT_KPIS"
	} as NetworkStatisticsMetricRequest,
	journeyKpis: {
		...journeyRequestBase(filters),
		type: "JOURNEY_KPIS"
	} as NetworkStatisticsMetricRequest,
	eventTimeSeries: {
		...eventRequestBase(filters),
		type: "EVENT_TIME_SERIES",
		bucket: filters.bucket
	} as NetworkStatisticsMetricRequest,
	journeyTimeSeries: {
		...journeyRequestBase(filters),
		type: "JOURNEY_TIME_SERIES",
		bucket: filters.bucket
	} as NetworkStatisticsMetricRequest,
	weekdayHourHeatmap: {
		...eventRequestBase(filters),
		type: "WEEKDAY_HOUR_HEATMAP"
	} as NetworkStatisticsMetricRequest,
	transportTypeComparison: {
		...eventRequestBase(filters),
		type: "TRANSPORT_TYPE_COMPARISON"
	} as NetworkStatisticsMetricRequest,
	stationRanking: {
		...eventRequestBase(filters),
		type: "STATION_RANKING",
		limit: 10,
		offset: 0,
		minVolume: filters.minVolume
	} as NetworkStatisticsMetricRequest,
	lineRanking: {
		...journeyRequestBase(filters),
		type: "LINE_RANKING",
		limit: 10,
		offset: 0,
		minVolume: Math.max(5, Math.floor(filters.minVolume / 4))
	} as NetworkStatisticsMetricRequest,
	mapHotspots: {
		...eventRequestBase(filters),
		minVolume: filters.minVolume
	} as NetworkMapHotspotsRequest
});

const createStationDashboardRequests = (filters: StatisticsFilterState, stationEvaNumber: number) => {
	const stationEventBase = {
		...eventRequestBase(filters),
		stationEvaNumber
	};

	return {
		eventKpis: {
			...stationEventBase,
			type: "EVENT_KPIS"
		} as StationStatisticsMetricRequest,
		benchmark: {
			...stationEventBase,
			type: "BENCHMARK"
		} as StationStatisticsMetricRequest,
		timeSeries: {
			...stationEventBase,
			type: "TIME_SERIES",
			bucket: filters.bucket
		} as StationStatisticsMetricRequest,
		arrivalDepartureComparison: {
			...stationEventBase,
			type: "ARRIVAL_DEPARTURE_COMPARISON"
		} as StationStatisticsMetricRequest,
		weekdayHourHeatmap: {
			...stationEventBase,
			type: "WEEKDAY_HOUR_HEATMAP"
		} as StationStatisticsMetricRequest,
		lineRanking: {
			...stationEventBase,
			type: "LINE_RANKING",
			limit: 10,
			offset: 0,
			minVolume: Math.max(5, Math.floor(filters.minVolume / 4))
		} as StationStatisticsMetricRequest,
		directions: {
			...stationEventBase,
			type: "DIRECTIONS"
		} as StationStatisticsMetricRequest,
		transportTypeMix: {
			...stationEventBase,
			type: "TRANSPORT_TYPE_MIX"
		} as StationStatisticsMetricRequest,
		lineHourMatrix: {
			...stationEventBase,
			type: "LINE_HOUR_MATRIX"
		} as StationStatisticsMetricRequest,
		eventDetails: {
			...stationEventBase,
			type: "EVENT_DETAILS",
			limit: 30,
			offset: 0
		} as StationStatisticsMetricRequest
	};
};

const expectMetricResult = <TResult = StatisticsMetricResult>(response: StatisticsMetricResponse, type: string): TResult => {
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

	return parsed.setZone(SERVICE_ZONE).toFormat("dd LLL, HH:mm");
};

const formatLocalTime = (value: string | null | undefined): string => {
	if (!value) return "-";
	const parsed = DateTime.fromISO(value, { setZone: true });
	if (!parsed.isValid) return value;

	return parsed.setZone(SERVICE_ZONE).toFormat("HH:mm");
};

const createRangeLabel = ({ from, to }: Pick<StatisticsFilterState, "from" | "to">): string => {
	const fromDate = DateTime.fromISO(from, { zone: SERVICE_ZONE });
	const toDate = DateTime.fromISO(to, { zone: SERVICE_ZONE }).minus({ days: 1 });
	if (!fromDate.isValid || !toDate.isValid) return `${from} to ${to}`;

	return `${fromDate.toFormat("dd LLL yyyy")} - ${toDate.toFormat("dd LLL yyyy")}`;
};

const eventReliability = (metrics: EventMetrics): number | null => toNumber(metrics.customerReliability5Rate);
const eventCancellationRate = (metrics: EventMetrics): number | null => toNumber(metrics.cancellationRate);
const journeyCompletionRate = (metrics: JourneyMetrics): number | null => toNumber(metrics.journeyCompletionRate);

export {
	SERVICE_ZONE,
	scheduleTypeOptions,
	statisticsBucketOptions,
	transportTypeOptions,
	createNetworkDashboardRequests,
	createRangeLabel,
	createSearchParamsFromStatisticsFilters,
	createStationDashboardRequests,
	createStatisticsFiltersFromSearchParams,
	defaultFilters,
	eventCancellationRate,
	eventReliability,
	expectMetricResult,
	formatCount,
	formatLocalDateTime,
	formatLocalTime,
	formatMetric,
	formatMinutes,
	formatRate,
	formatRateDelta,
	formatSeconds,
	journeyCompletionRate,
	rateToPercent,
	toNumber,
	transportTypeLabel,
	transportTypeShortLabel,
	type ScheduleTypeOption,
	type StatisticsBucketOption,
	type StatisticsFilterState,
	type TransportTypeOption
};
