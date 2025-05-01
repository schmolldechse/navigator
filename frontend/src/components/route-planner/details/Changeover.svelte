<script lang="ts">
	import { calculateDuration, formatDuration } from "$lib";
	import type { Time } from "$models/time";
	import Walking from "$components/ui/icons/Walking.svelte";
	import { DateTime } from "luxon";
	import CancelledTrip from "$components/timetable/messages/icons/CancelledTrip.svelte";
	import type { Stop } from "$models/station";
	import StopChild from "$components/route-planner/details/StopChild.svelte";

	let {
		startWalking,
		stopWalking,
		firstIsWalking = undefined,
		lastIsWalking = undefined
	}: {
		startWalking?: Time;
		stopWalking?: Time;
		firstIsWalking?: Stop;
		lastIsWalking?: Stop;
	} = $props();
	if (firstIsWalking && lastIsWalking) throw new Error("firstIsWalking and lastIsWalking cannot be both set");

	const walkingDuration: number = $derived.by(() => {
		if (!startWalking || !stopWalking) return 0;
		return calculateDuration(
			DateTime.fromISO(stopWalking?.actualTime ?? stopWalking?.plannedTime ?? ""),
			DateTime.fromISO(startWalking?.actualTime ?? startWalking?.plannedTime ?? ""),
			"minutes"
		).minutes;
	});
</script>

{#if firstIsWalking}
	<!-- Origin -->
	<StopChild time={startWalking} stop={firstIsWalking} isChangeover={true} position="start" />
{/if}

<div class="flex flex-row">
	<!-- 1/6 Time -->
	<span class="text-text/65 basis-1/6 self-center text-right">
		{walkingDuration > 0 ? formatDuration(stopWalking, startWalking) : ""}
	</span>

	<!-- Connecting Line -->
	<div class="relative flex justify-center w-[50px] md:w-[75px] transition-all duration-500">
		<span class="absolute z-0 h-full w-[4px] changeover"></span>
	</div>

	<!-- 5/6 Changeover Info -->
	<div class="flex basis-5/6 flex-col justify-center gap-y-2">
		<span class="flex items-center">
			<Walking height="35px" width="35px" class="stroke-accent" />
			Changeover
		</span>

		<!-- Warning -->
		{#if walkingDuration <= 0}
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
	<StopChild time={stopWalking} stop={lastIsWalking} isChangeover={true} position="end" />
{/if}
