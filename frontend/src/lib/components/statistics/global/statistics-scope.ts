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
	cloneScopeSettings,
	areScopeSettingsEqual,
	toMetricScopeRequest,
	toJourneyRankingMetricScopeRequest
};
