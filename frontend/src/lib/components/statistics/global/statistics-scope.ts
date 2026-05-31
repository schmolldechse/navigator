import { ScheduleType, TransportType } from "@lib/api";
import { DateTime } from "luxon";
import type { Component } from "svelte";
import LongDistance from "@lib/components/icons/transport-types/LongDistance.svelte";
import Regional from "@lib/components/icons/transport-types/Regional.svelte";
import Suburban from "@lib/components/icons/transport-types/Suburban.svelte";

type StatisticsScopeSettings = {
	dates: {
		start: DateTime;
		end: DateTime;
	};
	scheduleType: ScheduleType;
	transportTypes: TransportType[];
	includeReplacementTransport: boolean;
};

type TransportOption = {
	id: string;
	label: string;
	icon?: Component<{ class?: string }>;
	transportTypes: TransportType[];
};

type ScheduleOption = {
	id: ScheduleType;
	label: string;
};

const TRANSPORT_OPTIONS: TransportOption[] = [
	{
		id: "long-distance",
		label: "Long Distance",
		icon: LongDistance,
		transportTypes: [TransportType.HIGH_SPEED_TRAIN, TransportType.INTERCITY_TRAIN]
	},
	{
		id: "regional",
		label: "Regional",
		icon: Regional,
		transportTypes: [TransportType.REGIONAL_TRAIN, TransportType.INTER_REGIONAL_TRAIN]
	},
	{
		id: "suburban",
		label: "S-Bahn",
		icon: Suburban,
		transportTypes: [TransportType.CITY_TRAIN]
	}
];

const SCHEDULE_OPTIONS: ScheduleOption[] = [
	{
		id: ScheduleType.ARRIVAL,
		label: "Arrivals"
	},
	{
		id: ScheduleType.DEPARTURE,
		label: "Departures"
	}
];

const createDefaultStatisticsScopeSettings = (): StatisticsScopeSettings => ({
	dates: {
		start: DateTime.now().minus({ days: 7 }).startOf("day"),
		end: DateTime.now().endOf("day")
	},
	scheduleType: ScheduleType.DEPARTURE,
	transportTypes: [],
	includeReplacementTransport: true
});

const parseDate = (value: string | null, fallback: DateTime): DateTime => {
	if (!value) return fallback;

	const parsed = DateTime.fromISO(value);
	return parsed.isValid ? parsed : fallback;
};

const parseTransportTypes = (value: string | null): TransportType[] => {
	if (!value) return [];

	const validTransportTypes = new Set(Object.values(TransportType));
	return value
		.split(",")
		.map((part: string) => part.trim())
		.filter((part: string): part is TransportType => validTransportTypes.has(part as TransportType));
};

const parseBoolean = (value: string | null, fallback: boolean): boolean => {
	if (value === "true") return true;
	if (value === "false") return false;
	return fallback;
};

const createStatisticsScopeSettingsFromSearchParams = (searchParams: URLSearchParams): StatisticsScopeSettings => {
	const fallback = createDefaultStatisticsScopeSettings();
	const scheduleTypeValue = searchParams.get("scheduleType");
	const validScheduleTypes = new Set(Object.values(ScheduleType));
	const start = parseDate(searchParams.get("start"), fallback.dates.start).startOf("day");
	const end = parseDate(searchParams.get("end"), fallback.dates.end).endOf("day");

	return {
		dates: start <= end ? { start, end } : fallback.dates,
		scheduleType: validScheduleTypes.has(scheduleTypeValue as ScheduleType)
			? (scheduleTypeValue as ScheduleType)
			: fallback.scheduleType,
		transportTypes: parseTransportTypes(searchParams.get("transportTypes")),
		includeReplacementTransport: parseBoolean(
			searchParams.get("includeReplacementTransport"),
			fallback.includeReplacementTransport
		)
	};
};

const updateStatisticsScopeSearchParams = (searchParams: URLSearchParams, scope: StatisticsScopeSettings): URLSearchParams => {
	const next = new URLSearchParams(searchParams);

	next.set("start", scope.dates.start.toISODate()!);
	next.set("end", scope.dates.end.toISODate()!);
	next.set("scheduleType", scope.scheduleType);
	next.set("includeReplacementTransport", String(scope.includeReplacementTransport));

	if (scope.transportTypes.length > 0) next.set("transportTypes", [...scope.transportTypes].sort().join(","));
	else next.delete("transportTypes");

	return next;
};

const cloneScopeSettings = (source: StatisticsScopeSettings): StatisticsScopeSettings => ({
	dates: {
		start: source.dates.start,
		end: source.dates.end
	},
	scheduleType: source.scheduleType,
	transportTypes: [...source.transportTypes],
	includeReplacementTransport: source.includeReplacementTransport
});

const areScopeSettingsEqual = (left: StatisticsScopeSettings, right: StatisticsScopeSettings): boolean =>
	left.dates.start.toISODate() === right.dates.start.toISODate() &&
	left.dates.end.toISODate() === right.dates.end.toISODate() &&
	left.scheduleType === right.scheduleType &&
	left.includeReplacementTransport === right.includeReplacementTransport &&
	[...left.transportTypes].sort().join("|") === [...right.transportTypes].sort().join("|");

const toMetricScopeRequest = (scope: StatisticsScopeSettings) => ({
	start: scope.dates.start.startOf("day").toISO()!,
	end: scope.dates.end.endOf("day").toISO()!,
	scheduleType: scope.scheduleType,
	transportTypes: [...(scope.transportTypes ?? [])].sort(),
	includeReplacementTransport: scope.includeReplacementTransport
});

const toJourneyRankingMetricScopeRequest = (scope: StatisticsScopeSettings) => ({
	start: scope.dates.start.startOf("day").toISO()!,
	end: scope.dates.end.endOf("day").toISO()!,
	transportTypes: scope.transportTypes,
	includeReplacementTransport: scope.includeReplacementTransport
});

export {
	type StatisticsScopeSettings,
	type TransportOption,
	type ScheduleOption,
	TRANSPORT_OPTIONS,
	SCHEDULE_OPTIONS,
	createDefaultStatisticsScopeSettings,
	createStatisticsScopeSettingsFromSearchParams,
	cloneScopeSettings,
	areScopeSettingsEqual,
	updateStatisticsScopeSearchParams,
	toMetricScopeRequest,
	toJourneyRankingMetricScopeRequest
};
