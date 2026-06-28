import { TransportType } from "@lib/api";
import Bus from "@lib/components/icons/transport-types/Bus.svelte";
import Ferry from "@lib/components/icons/transport-types/Ferry.svelte";
import LongDistance from "@lib/components/icons/transport-types/LongDistance.svelte";
import Regional from "@lib/components/icons/transport-types/Regional.svelte";
import Suburban from "@lib/components/icons/transport-types/Suburban.svelte";
import Subway from "@lib/components/icons/transport-types/Subway.svelte";
import Taxi from "@lib/components/icons/transport-types/Taxi.svelte";
import Tram from "@lib/components/icons/transport-types/Tram.svelte";
import type { Component } from "svelte";
import type { ClassValue } from "svelte/elements";

type TransportIconComponent = Component<{
	type?: "rounded-corners";
	class?: ClassValue;
}>;

type TransportTypeIconGroup = {
	key: string;
	label: string;
	shortLabel: string;
	Icon: TransportIconComponent;
};

const transportTypeIconGroups: Partial<Record<TransportType, TransportTypeIconGroup>> = {
	[TransportType.HIGH_SPEED_TRAIN]: {
		key: "long-distance",
		label: "Long-distance rail",
		shortLabel: "Long distance",
		Icon: LongDistance
	},
	[TransportType.INTERCITY_TRAIN]: {
		key: "long-distance",
		label: "Long-distance rail",
		shortLabel: "Long distance",
		Icon: LongDistance
	},
	[TransportType.INTER_REGIONAL_TRAIN]: {
		key: "regional",
		label: "Regional rail",
		shortLabel: "Regional",
		Icon: Regional
	},
	[TransportType.REGIONAL_TRAIN]: {
		key: "regional",
		label: "Regional rail",
		shortLabel: "Regional",
		Icon: Regional
	},
	[TransportType.CITY_TRAIN]: {
		key: "suburban",
		label: "Suburban rail",
		shortLabel: "Suburban",
		Icon: Suburban
	},
	[TransportType.SUBWAY]: {
		key: "subway",
		label: "Subway",
		shortLabel: "Subway",
		Icon: Subway
	},
	[TransportType.TRAM]: {
		key: "tram",
		label: "Tram",
		shortLabel: "Tram",
		Icon: Tram
	},
	[TransportType.BUS]: {
		key: "bus",
		label: "Bus",
		shortLabel: "Bus",
		Icon: Bus
	},
	[TransportType.FERRY]: {
		key: "ferry",
		label: "Ferry",
		shortLabel: "Ferry",
		Icon: Ferry
	},
	[TransportType.TAXI]: {
		key: "taxi",
		label: "Taxi",
		shortLabel: "Taxi",
		Icon: Taxi
	}
};

const transportTypeGroupOrder = ["long-distance", "regional", "suburban", "subway", "tram", "bus", "ferry", "taxi"];

const toTransportType = (transportType: TransportType | string): TransportType | undefined =>
	Object.values(TransportType).includes(transportType as TransportType) ? (transportType as TransportType) : undefined;

const getTransportTypeIconGroup = (transportType: TransportType | string): TransportTypeIconGroup | undefined => {
	const normalizedTransportType = toTransportType(transportType);
	return normalizedTransportType ? transportTypeIconGroups[normalizedTransportType] : undefined;
};

const getTransportTypeIconGroups = (transportTypes: Array<TransportType | string>): TransportTypeIconGroup[] => {
	const groups = new Map<string, TransportTypeIconGroup>();

	for (const transportType of transportTypes) {
		const group = getTransportTypeIconGroup(transportType);
		if (!group || groups.has(group.key)) continue;

		groups.set(group.key, group);
	}

	return Array.from(groups.values()).sort(
		(a, b) => transportTypeGroupOrder.indexOf(a.key) - transportTypeGroupOrder.indexOf(b.key)
	);
};

export { getTransportTypeIconGroup, getTransportTypeIconGroups, type TransportTypeIconGroup };
