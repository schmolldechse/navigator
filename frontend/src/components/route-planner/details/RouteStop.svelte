<script lang="ts">
	import GeneralWarning from "$components/timetable/messages/icons/GeneralWarning.svelte";
	import NoOnwardJourney from "$components/timetable/messages/icons/NoOnwardJourney.svelte";
	import type { ExtendedRouteStop, RouteMessage, Time } from "$models/models";
	import ChevronRight from "lucide-svelte/icons/chevron-right";
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import { DateTime } from "luxon";
	import type { Component } from "svelte";

	interface Props {
		stop: ExtendedRouteStop;
		showDeparture?: boolean;
        position: "start" | "end";
	}

	let { stop, showDeparture = true, position }: Props = $props();

	const getMessageComponent = (message: RouteMessage, fallbackCancelledValue: boolean): Component => {
		if (message.content === "Stop cancelled" || message.content === "Halt entfällt" || fallbackCancelledValue)
			return NoOnwardJourney;
		return GeneralWarning;
	};

	const isSamePlatform = (plannedPlatform: string, actualPlatform: string): boolean => {
		return plannedPlatform === actualPlatform || (plannedPlatform === "" && actualPlatform === "");
	};
</script>

{#snippet renderTimeInformation(time: Time)}
	{@const delay = DateTime.fromISO(time?.actualTime).diff(DateTime.fromISO(time?.plannedTime), ["seconds"]).seconds}
	{@const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE)}

	<div class="flex basis-1/6 flex-col justify-end gap-x-2 items-end">
		<span class="text-base font-medium md:text-xl">{displayTime(time.plannedTime)}</span>
		{#if delay < -60 || delay > 60}
			<span class="text-background bg-white px-1 text-[10pt] font-bold md:px-2 md:text-[12pt]"
				>{displayTime(time.actualTime)}</span
			>
		{/if}
	</div>
{/snippet}

<div class="relative flex min-h-fit flex-row">
	<!-- 1/6 TimeInformation -->
	{@render renderTimeInformation(showDeparture ? stop.departure! : stop.arrival!)}

	<!-- Connecting Line -->
	<div class="flex w-[50px] justify-center transition-all duration-500 md:w-[75px] items-center">
		<CircleDot class={`bg-background z-1 absolute shrink-0 ${position === "start" ? "self-start" : ""} ${position === "end" ? "self-end" : ""}`} />
		<span class={["absolute z-0 h-full w-[4px] bg-white", { "inset-y-1": position === "start" }, { "-inset-y-1": position === "end" }]}></span>
	</div>

	<!-- 4/6 Stop -->
	<div class={["flex basis-4/6 flex-col", { "self-start": position === "start" }, { "self-end": position === "end" }]}>
		<div class="flex flex-row">
			<span class="text-base md:text-xl">{stop.name}</span>
			<ChevronRight color="#ffda0a" class="ml-1 shrink-0" />
		</div>

		<div class="flex flex-col">
			{#each stop.messages as message}
				{@const MessageComponent = getMessageComponent(message, stop.cancelled)}
				<div class="flex items-center gap-x-2">
					<MessageComponent />
					<span class="text-sm font-semibold">{message.content}</span>
				</div>
			{/each}
		</div>
	</div>

	<!-- 1/6 Platform -->
	<div class={["flex basis-1/6 flex-col items-end md:items-start", { "self-start": position === "start" }, { "self-end": position === "end" }]}>
		<span class="text-base font-medium md:text-xl">{showDeparture ? (stop.departure?.plannedPlatform ?? "") : (stop.arrival?.plannedPlatform ?? "")}</span>
		{#if !isSamePlatform(showDeparture ? (stop.departure?.plannedPlatform ?? "") : (stop.arrival?.plannedPlatform ?? ""), showDeparture ? (stop.departure?.actualPlatform ?? "") : (stop.arrival?.actualPlatform ?? ""))}
			<span class="text-background bg-white px-1 text-[10pt] font-bold md:px-2 md:text-[12pt]"
				>{showDeparture ? (stop.departure?.actualPlatform ?? "") : (stop.arrival?.actualPlatform ?? "")}</span
			>
		{/if}
	</div>
</div>
