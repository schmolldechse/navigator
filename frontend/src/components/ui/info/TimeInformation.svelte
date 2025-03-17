<script lang="ts">
	import type { Time } from "$models/time";
	import calculateDuration from "$lib/time";
	import { DateTime } from "luxon";

	let { time, direction = "row" }: { time?: Time, direction?: "row" | "col" } = $props();

	const isDelayed = () => calculateDuration(DateTime.fromISO(time?.actualTime || time?.plannedTime!), DateTime.fromISO(time?.plannedTime!), "minutes") >= 1;
	const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE);
</script>

<div class="flex items-center justify-end gap-x-2"
	 class:flex-row={direction === "row"}
	 class:flex-col={direction === "col"}
>
	<span>{displayTime(time?.plannedTime ?? "")}</span>
	{#if isDelayed()}
		<span class="bg-text text-background px-2 text-lg font-bold md:py-0.25 md:text-2xl">{displayTime(time?.actualTime ?? "")}</span>
	{/if}
</div>
