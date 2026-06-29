<script lang="ts">
	import GeneralWarning from "$components/timetable/messages/icons/GeneralWarning.svelte";
	import NoOnwardJourney from "$components/timetable/messages/icons/NoOnwardJourney.svelte";
	import type { ExtendedRouteStop, RouteMessage, Time } from "$models/models";
	import ChevronRight from "@lucide/svelte/icons/chevron-right";
	import CircleDot from "@lucide/svelte/icons/circle-dot";
	import { DateTime } from "luxon";
	import type { Component } from "svelte";

	interface Props {
		stop: ExtendedRouteStop;
	}

	let { stop }: Props = $props();

	const getMessageComponent = (message: RouteMessage, fallbackCancelledValue: boolean): Component => {
		if (message.content === "Stop cancelled" || message.content === "Halt entfällt" || fallbackCancelledValue)
			return NoOnwardJourney;
		return GeneralWarning;
	};

	const isSamePlatform = (): boolean => {
		return (
			stop.departure!.plannedPlatform === stop.departure!.actualPlatform ||
			(stop.departure!.plannedPlatform === "" && stop.departure!.actualPlatform === "")
		);
	};
</script>

{#snippet renderTimeInformation(time: Time)}
	{@const delay = DateTime.fromISO(time?.actualTime).diff(DateTime.fromISO(time?.plannedTime), ["seconds"]).seconds}
	{@const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE)}

	{@const isDelayed = delay < -60 || delay > 60}

	<div class="flex min-w-1 basis-1/6 flex-row items-center gap-x-2">
		<span class={["text-base font-medium", { hidden: isDelayed }]}>{displayTime(time.plannedTime)}</span>
		{#if isDelayed}
			<span class="text-background bg-white px-1 text-[10pt] font-bold md:px-2">{displayTime(time.actualTime)}</span>
		{/if}
	</div>
{/snippet}

<div class="relative flex min-h-fit flex-row py-1">
	<!-- 1/6 TimeInformation -->
	<div class="flex min-w-1 basis-1/6 flex-col items-end">
		{@render renderTimeInformation(stop.arrival!)}
		{@render renderTimeInformation(stop.departure!)}
	</div>

	<!-- Connecting Line -->
	<div class="flex w-[50px] justify-center transition-all duration-500 md:w-[75px]">
		<span class="absolute z-1 h-[15px] w-[15px] self-center rounded-full border-2 border-white/85 bg-black"></span>
		<span class="absolute inset-y-0 z-0 h-full w-[4px] bg-white"></span>
	</div>

	<!-- 4/6 Stop -->
	<div class="flex basis-4/6 flex-col self-center leading-none">
		<div class="flex flex-row items-center">
			<span class="text-base">{stop.name}</span>
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
	<div class="flex basis-1/6 flex-col items-end self-center md:items-start">
		<span class="text-base font-medium">{stop.departure?.plannedPlatform ?? ""}</span>
		{#if !isSamePlatform()}
			<span class="text-background bg-white px-1 text-[10pt] font-bold md:px-2"
				>{stop.departure?.actualPlatform ?? ""}</span
			>
		{/if}
	</div>
</div>
