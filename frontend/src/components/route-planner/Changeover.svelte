<script lang="ts">
	import { calculateDuration, formatDuration } from "$lib";
	import type { Time } from "$models/time";
	import Walking from "$components/ui/icons/Walking.svelte";
	import { DateTime } from "luxon";
	import CancelledTrip from "$components/timetable/messages/icons/CancelledTrip.svelte";
	import Platform from "$components/ui/info/Platform.svelte";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import type { Stop } from "$models/station";
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import ChevronRight from "lucide-svelte/icons/chevron-right";

	/**
	 * if firstIsWalking is true, arrival & departure are swapped
	 * this is because it uses the "departure" time from the stop where you start walking, and the "arrival" time at the stop where you stop walking
	 */
	let { arrival, departure, firstIsWalking = false, origin }: {
		arrival?: Time;
		departure?: Time,
		firstIsWalking?: boolean,
		origin?: Stop
	} = $props();
	if (firstIsWalking && !origin) throw new Error("Origin is required when firstIsWalking is true");

	const duration = () => calculateDuration(
		DateTime.fromISO(firstIsWalking ? (arrival?.actualTime ?? arrival?.plannedTime ?? "") : (departure?.actualTime ?? departure?.plannedTime ?? "")),
		DateTime.fromISO(firstIsWalking ? (departure?.actualTime ?? departure?.plannedTime ?? "") : (arrival?.actualTime ?? arrival?.plannedTime ?? "")),
		["minutes"]
	).as("minutes");
</script>

{#if firstIsWalking}
	<!-- Origin -->
	<div class="flex flex-row text-base">
		<!-- Departure is the time when you start walking -->
		<TimeInformation
			time={departure}
			direction="col"
			class="basis-1/6 items-end text-base"
			delayClass="text-sm md:text-base"
		/>
		<div class="relative flex basis-1/6 md:max-w-[5%] justify-center">
			<CircleDot class="bg-background absolute z-10 shrink-0 self-start" />
			<span
				class="bg-[repeating-linear-gradient(0deg,_#9ca3af_0px,_#9ca3af_2px,_transparent_0px,_transparent_5px)] absolute z-0 h-full w-[4px] self-end"></span>
		</div>
		<div class="flex flex-row basis-4/6 items-center justify-between">
			<a
				class="flex flex-row font-bold self-start"
				href={`/${origin?.evaNumber}/departures`}
				target="_blank"
			>
				{origin?.name}
				<ChevronRight color="#ffda0a" class="shrink-0" />
			</a>
			<Platform time={departure} class="basis-1/6 text-right self-start" direction="col" />
		</div>
	</div>
{/if}

<div class="relative flex flex-row text-base py-12">
	<span class="text-text/65 basis-1/6 self-center text-right">
		{duration() > 0 ? formatDuration(firstIsWalking ? arrival : departure, firstIsWalking ? departure : arrival) : ""}
	</span>
	<div class="flex basis-1/6 md:max-w-[5%] justify-center">
		<span
			class="absolute inset-y-4 z-0 h-full w-[4px] self-center bg-[repeating-linear-gradient(0deg,_#9ca3af_0px,_#9ca3af_2px,_transparent_0px,_transparent_5px)]"
		></span>
	</div>
	<div class="flex flex-col basis-4/6 justify-center gap-y-2">
		<span class="flex items-center">
			<Walking height="35px" width="35px" class="stroke-accent" />
			Changeover
		</span>
		{#if duration() < 0}
			<div class="text-background bg-white p-1">
				<div class="flex flex-row gap-x-2">
					<CancelledTrip />
					<span class="font-bold">Warning:</span>
				</div>
				<span>Changeover probably not possible</span>
			</div>
		{/if}
	</div>
</div>
