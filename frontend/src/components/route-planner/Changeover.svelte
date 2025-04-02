<script lang="ts">
	import { calculateDuration, formatDuration } from "$lib";
	import type { Time } from "$models/time";
	import Walking from "$components/ui/icons/Walking.svelte";
	import { DateTime } from "luxon";
	import CancelledTrip from "$components/timetable/messages/icons/CancelledTrip.svelte";

	let { arrival, departure }: { arrival?: Time; departure?: Time } = $props();

	const duration = () => calculateDuration(
		DateTime.fromISO(departure?.actualTime ?? departure?.plannedTime ?? ""),
		DateTime.fromISO(arrival?.actualTime ?? arrival?.plannedTime ?? ""),
		["minutes"]
	).as("minutes");
</script>

<div class="relative flex flex-row text-base py-12">
	<span class="text-text/65 basis-1/6 self-center text-right">
		{duration() > 0 ? formatDuration(departure, arrival) : ""}
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
