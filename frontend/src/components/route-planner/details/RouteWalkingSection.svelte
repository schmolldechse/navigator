<script lang="ts">
	import type { NormalRouteStop, Time } from "$models/models";
	import { DateTime } from "luxon";
	import RouteStop from "./RouteStop.svelte";

	interface Props {
		stop: NormalRouteStop;
		startWalking: Time;
		stopWalking: Time;
		distanceInMeters: number;
		isFirst: boolean;
		isLast: boolean;
	}

	let { stop, startWalking, stopWalking, distanceInMeters, isFirst = false, isLast = false }: Props = $props();
	if (isFirst && isLast) {
		throw new Error("WalkingSection cannot be both first and last");
	}

	const walkingDuration = $derived.by(() => {
		const departureTime = DateTime.fromISO(startWalking.actualTime);
		const arrivalTime = DateTime.fromISO(stopWalking.actualTime);

		const duration = arrivalTime.diff(departureTime, ["hours", "minutes"]);
		if (duration.hours >= 1) return duration.toFormat("h'h' m'm'");
		else return duration.toFormat("m'm'");
	});
</script>

{#if isFirst}
	<div class="flex flex-col">
		<RouteStop {stop} showDeparture={true} position="start" />

		<div class="relative flex min-h-fit flex-row py-6">
			<div class="text-text/75 flex basis-1/6 justify-end italic">
				{walkingDuration}
			</div>

			<div class="flex w-[50px] justify-center md:w-[75px]">
				<span class="z-1 absolute inset-y-0 h-full w-[4px] bg-white"></span>
			</div>

			<span class="flex h-[20px] basis-4/6">
				<p>{distanceInMeters}m walking</p>
			</span>

			<span class="flex h-[20px] basis-1/6"></span>
		</div>
	</div>
{/if}

{#if !isFirst && !isLast}
	<div class="relative flex min-h-fit flex-row py-6">
		<div class="text-text/75 flex basis-1/6 justify-end italic">
			{walkingDuration}
		</div>

		<div class="flex w-[50px] justify-center md:w-[75px]">
			<span class="z-1 absolute inset-y-0 h-full w-[4px] bg-white"></span>
		</div>

		<span class="flex h-[20px] basis-4/6">
			<p>{distanceInMeters}m walking</p>
		</span>

		<span class="flex h-[20px] basis-1/6"></span>
	</div>
{/if}

{#if isLast}
	<div class="flex flex-col">
		<div class="relative flex min-h-fit flex-row py-6">
			<div class="text-text/75 flex basis-1/6 justify-end italic">
				{walkingDuration}
			</div>

			<div class="flex w-[50px] justify-center md:w-[75px]">
				<span class="z-1 absolute inset-y-0 h-full w-[4px] bg-white"></span>
			</div>

			<span class="flex h-[20px] basis-4/6">
				<p>{distanceInMeters}m walking</p>
			</span>

			<span class="flex h-[20px] basis-1/6"></span>
		</div>

		<RouteStop {stop} showDeparture={false} position="end" />
	</div>
{/if}
