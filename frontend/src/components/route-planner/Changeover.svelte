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

	let {
		arrival,
		departure,
		firstIsWalking = undefined,
		lastIsWalking = undefined
	}: {
		arrival?: Time;
		departure?: Time;
		firstIsWalking?: Stop;
		lastIsWalking?: Stop;
	} = $props();
	if (firstIsWalking && lastIsWalking) throw new Error("firstIsWalking and lastIsWalking cannot be both set");

	const duration = () =>
		calculateDuration(
			DateTime.fromISO(
				firstIsWalking
					? (arrival?.actualTime ?? arrival?.plannedTime ?? "")
					: (departure?.actualTime ?? departure?.plannedTime ?? "")
			),
			DateTime.fromISO(
				firstIsWalking
					? (departure?.actualTime ?? departure?.plannedTime ?? "")
					: (arrival?.actualTime ?? arrival?.plannedTime ?? "")
			),
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
		<div class="relative flex basis-1/6 justify-center md:max-w-[5%]">
			<CircleDot class="bg-background absolute z-10 shrink-0 self-start" />
			<span
				class="absolute z-0 h-full w-[4px] self-end bg-[repeating-linear-gradient(0deg,_#9ca3af_0px,_#9ca3af_2px,_transparent_0px,_transparent_5px)]"
			></span>
		</div>
		<div class="flex basis-4/6 flex-row items-center justify-between">
			<a class="flex flex-row self-start font-bold" href={`/${firstIsWalking?.evaNumber}/departures`} target="_blank">
				{firstIsWalking?.name}
				<ChevronRight color="#ffda0a" class="shrink-0" />
			</a>
			<Platform time={departure} class="basis-1/6 self-start text-right" direction="col" />
		</div>
	</div>
{/if}

<div class="relative flex flex-row py-12 text-base">
	<span class="text-text/65 basis-1/6 self-center text-right">
		{duration() > 0 ? formatDuration(firstIsWalking ? arrival : departure, firstIsWalking ? departure : arrival) : ""}
	</span>
	<div class="flex basis-1/6 justify-center md:max-w-[5%]">
		<span
			class="absolute inset-y-4 z-0 h-full w-[4px] self-center bg-[repeating-linear-gradient(0deg,_#9ca3af_0px,_#9ca3af_2px,_transparent_0px,_transparent_5px)]"
		></span>
	</div>
	<div class="flex basis-4/6 flex-col justify-center gap-y-2">
		<span class="flex items-center">
			<Walking height="35px" width="35px" class="stroke-accent" />
			Changeover
		</span>
		{#if duration() <= 0}
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

{#if lastIsWalking}
	<!-- Destination -->
	<div class="flex flex-row text-base">
		<TimeInformation
			time={departure}
			direction="col"
			class="basis-1/6 items-end self-end text-base"
			delayClass="text-sm md:text-base"
		/>
		<div class="relative flex basis-1/6 justify-center md:max-w-[5%]">
			<CircleDot class="bg-background absolute z-10 shrink-0 self-start" />
			<span
				class="absolute z-0 h-full w-[4px] self-end bg-[repeating-linear-gradient(0deg,_#9ca3af_0px,_#9ca3af_2px,_transparent_0px,_transparent_5px)]"
			></span>
		</div>
		<div class="flex basis-4/6 flex-row items-center justify-between self-end">
			<a class="flex flex-row font-bold self-end" href={`/${lastIsWalking?.evaNumber}/departures`} target="_blank">
				{lastIsWalking?.name}
				<ChevronRight color="#ffda0a" class="shrink-0 self-center" />
			</a>
			<Platform time={arrival} class="self-end text-right" direction="col" />
		</div>
	</div>
{/if}