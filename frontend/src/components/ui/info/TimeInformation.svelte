<script lang="ts">
	import type { Time } from "$models/time";
	import calculateDuration from "$lib/time";
	import { DateTime } from "luxon";
	import clsx from "clsx";

	let { time, direction = "row", class: className = "", delayClass = "" }: {
		time?: Time;
		direction?: "row" | "col",
		class?: string,
		delayClass?: string
	} = $props();

	const isDelayed = () =>
		calculateDuration(DateTime.fromISO(time?.actualTime || time?.plannedTime!), DateTime.fromISO(time?.plannedTime!), [
			"minutes"
		])?.minutes ?? 0 >= 1;
	const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE);
</script>

<div
	class={clsx("flex", {
		"flex-row items-center justify-end gap-x-2": direction === "row",
		"flex-col": direction === "col"
	}, className)}
>
	<span>{displayTime(time?.plannedTime ?? "")}</span>
	{#if isDelayed()}
		<span class={clsx("w-fit bg-text text-background px-2 text-center font-bold", delayClass)}>{displayTime(time?.actualTime ?? "")}</span>
	{/if}
</div>
