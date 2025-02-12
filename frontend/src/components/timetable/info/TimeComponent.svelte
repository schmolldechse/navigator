<script lang="ts">
	import type { Time } from "$models/time";
	import calculateDuration from "$lib/time";
	import { DateTime } from "luxon";

	let { time }: { time?: Time } = $props();

	const isDelayed = () => {
		const diff = calculateDuration(
			DateTime.fromISO(time?.actualTime || time?.plannedTime!),
			DateTime.fromISO(time?.plannedTime!),
			"minutes"
		);
		return diff >= 1;
	}

	const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE);
</script>

<div class="flex flex-row justify-end items-center gap-x-2">
	<span>{displayTime(time?.plannedTime ?? "")}</span>
	{#if isDelayed()}
		<span class="font-bold bg-text text-background text-2xl px-2 py-0.25">{displayTime(time?.actualTime ?? "")}</span>
	{/if}
</div>