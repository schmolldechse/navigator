<script lang="ts">
	import type { Route } from "$models/route";
	import Minus from "lucide-svelte/icons/minus";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import calculateDuration from "$lib/time";
	import { DateTime } from "luxon";

	let { route }: { route: Route } = $props();

	const formatDuration = (): string => {
		const duration = calculateDuration(
			DateTime.fromISO(route?.legs[route?.legs?.length - 1]?.arrival?.plannedTime ?? route?.legs[route?.legs?.length - 1]?.arrival?.actualTime ?? ""),
			DateTime.fromISO(route?.legs[0]?.departure?.plannedTime ?? route?.legs[0]?.departure?.actualTime ?? ""),
			["minutes"]
		).as("minutes");

		if (duration > 60) {
			const hours = Math.floor(duration / 60);
			const minutes = duration % 60;
			return `${hours} h ${minutes} min`;
		} else return `${Math.floor(duration)} min`;
	};
</script>

<div class="text-2xl font-medium">
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
</div>