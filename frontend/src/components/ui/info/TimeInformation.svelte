<script lang="ts">
	import type { Time } from "$models/time";
	import { calculateDuration } from "$lib";
	import { DateTime } from "luxon";

	let {
		time,
		noDoubleSpan = false,
		direction = "row",
		class: className = "",
		delayClass = ""
	}: {
		time?: Time;
		noDoubleSpan?: boolean;
		direction?: "row" | "col";
		class?: string;
		delayClass?: string;
	} = $props();

	const isDelayed = () =>
		calculateDuration(DateTime.fromISO(time?.actualTime || time?.plannedTime!), DateTime.fromISO(time?.plannedTime!), [
			"minutes"
		])?.minutes ?? 0 >= 1;
	const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE);
</script>

{#if noDoubleSpan}
	<span class={["w-fit", { "bg-text text-background px-2 font-bold": isDelayed() }, { [delayClass]: isDelayed() }, className]}
		>{displayTime(isDelayed() ? (time?.actualTime ?? "") : (time?.plannedTime ?? ""))}</span
	>
{:else}
	<div
		class={[
			"flex",
			{ "flex-row items-center justify-end gap-x-2": direction === "row" },
			{ "flex-col": direction === "col" },
			className
		]}
	>
		<span>{displayTime(time?.plannedTime ?? "")}</span>
		{#if isDelayed()}<span class={["bg-text text-background w-fit px-2 text-center font-bold", delayClass]}
				>{displayTime(time?.actualTime ?? "")}</span
			>{/if}
	</div>
{/if}
