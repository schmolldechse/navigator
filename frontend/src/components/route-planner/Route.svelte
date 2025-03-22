<script lang="ts">
	import type { Route } from "$models/route";
	import Minus from "lucide-svelte/icons/minus";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import calculateDuration from "$lib/time";
	import { DateTime } from "luxon";
	import type { Connection } from "$models/connection";

	let { route }: { route: Route } = $props();

	const formatDuration = (): string => {
		const end = route?.legs[route?.legs?.length - 1];
		const start = route?.legs[0];

		const duration = calculateDuration(
			DateTime.fromISO(end?.arrival?.actualTime ?? end?.arrival?.plannedTime ?? ""),
			DateTime.fromISO(start?.departure?.actualTime ?? start?.departure?.plannedTime ?? ""),
			["minutes"]
		).as("minutes");

		if (duration > 60) {
			const hours = Math.floor(duration / 60);
			const minutes = duration % 60;
			return `${hours} h ${minutes} min`;
		} else return `${Math.floor(duration)} min`;
	};

	const durationWithoutWalking = $derived(() => {
		// remove legs with "walking" property
		const legs = (route?.legs || []).filter(leg => !leg?.walking);
		if (legs.length === 0) return 0;

		return legs.flatMap(leg => calculateDuration(
				DateTime.fromISO(leg?.arrival?.actualTime ?? leg?.arrival?.plannedTime ?? ""),
				DateTime.fromISO(leg?.departure?.actualTime ?? leg?.departure?.plannedTime ?? ""),
				["minutes"]
			).as("minutes"))
			.reduce((acc, el) => acc + el);
	});

	const durationByConnection = (connection: Connection) => {
		return calculateDuration(
			DateTime.fromISO(connection?.arrival?.actualTime ?? connection?.arrival?.plannedTime ?? ""),
			DateTime.fromISO(connection?.departure?.actualTime ?? connection?.departure?.plannedTime ?? ""),
			["minutes"]
		).as("minutes");
	}

	const getWidthRatio = (duration: number, maxDuration: number) => {
		if (maxDuration <= 0) return '0%';
		return `${(duration / maxDuration) * 100}%;`
	}
</script>

<div class="px-4 py-2 text-2xl font-medium space-y-2 border-primary-dark/75 border-2 rounded-lg">
	<!-- Time Info -->
	<div class="flex flex-row gap-x-2">
		<div class="flex flex-row ">
			<TimeInformation time={route?.legs[0]?.departure} direction="col" />
			<Minus class="mx-2 mt-[0.25rem]" />
			<TimeInformation time={route?.legs[route?.legs?.length - 1]?.arrival} direction="col" />
		</div>

		<span class="text-primary/90">|</span>
		<span
			class="text-lg mt-[0.35rem]">{formatDuration()}</span>
	</div>

	<div class="flex flex-row w-full gap-x-2">
		{#each route?.legs.filter(leg => !leg?.walking) as leg}
			<span class="rounded-lg bg-primary-darker text-base md:text-lg px-2 py-1 text-center line-clamp-1 md:line-clamp-none truncate" style={`width: ${getWidthRatio(durationByConnection(leg), durationWithoutWalking())}`}>{leg?.lineInformation?.lineName}</span>
		{/each}
	</div>
</div>