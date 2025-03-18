<script lang="ts">
	import type { Time } from "$models/time";
	import calculateDuration from "$lib/time";
	import { DateTime } from "luxon";

	let { time, direction = "row" }: { time?: Time, direction?: "row" | "col" } = $props();

	const isDelayed = () => calculateDuration(DateTime.fromISO(time?.actualTime || time?.plannedTime!), DateTime.fromISO(time?.plannedTime!), "minutes") >= 1;
	const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE);
</script>

<div class="flex"
	 class:flex-row={direction === "row"}
	 class:items-center={direction === "row"}
	 class:justify-end={direction === "row"}
	 class:gap-x-2={direction === "row"}
	 class:flex-col={direction === "col"}
>
	<span>{displayTime(time?.plannedTime ?? "")}</span>
	{#if isDelayed()}
		<span class:md:text-2xl={direction === "row"} class="text-center bg-text text-background px-2 text-lg font-bold">{displayTime(time?.actualTime ?? "")}</span>
	{/if}
</div>
