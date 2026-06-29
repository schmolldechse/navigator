<script lang="ts">
	import type { NormalSection, RouteMessage } from "$models/models";
	import { DateTime } from "luxon";
	import RouteStop from "./RouteStop.svelte";
	import GeneralWarning from "$components/timetable/messages/icons/GeneralWarning.svelte";
	import ChevronDown from "@lucide/svelte/icons/chevron-down";
	import ChevronUp from "@lucide/svelte/icons/chevron-up";
	import RouteViaStop from "./RouteViaStop.svelte";

	interface Props {
		section: NormalSection;
	}

	let { section }: Props = $props();
	let showViaStops = $state<boolean>(false);

	let sectionDuration = $derived.by(() => {
		const departureTime = DateTime.fromISO(section.origin.departure!.actualTime);
		const arrivalTime = DateTime.fromISO(section.destination.arrival!.actualTime);

		const duration = arrivalTime.diff(departureTime, ["hours", "minutes"]);
		if (duration.hours >= 1) return duration.toFormat("h'h' m'm'");
		else return duration.toFormat("m'm'");
	});
</script>

<div class="flex flex-col md:px-2">
	<!-- Origin -->
	<RouteStop stop={section.origin} showDeparture={true} position="start" />

	<!-- Line Info -->
	<div class="relative flex flex-row py-10">
		<!-- 1/6 Time -->
		<span class="min-w-1 basis-1/6 text-right text-sm">{sectionDuration}</span>

		<!-- Connecting Line -->
		<div class="flex w-[50px] justify-center transition-all duration-500 md:w-[75px]">
			<span class="absolute inset-y-0 z-0 h-full w-[4px] bg-white"></span>
		</div>

		<!-- 5/6 -->
		<div class="flex basis-5/6 flex-col">
			<!-- LineColor -->
			<div class="flex flex-row items-baseline gap-x-2">
				<span>
					{section.lineInformation?.journeyName}
				</span>
				{#if section.lineInformation.journeyNumber}
					<span>({section.lineInformation?.journeyNumber})</span>
				{/if}
			</div>

			<!-- Continuation -->
			<span class="text-text/65">{`Continues to ${section.journeysDirection?.name}`}</span>
		</div>
	</div>

	<!-- (Warning-)Messages, that are not type of "hint" (<- only information regarding the Connection) -->
	{#if section.messages.length > 0}
		<div class="relative flex flex-row pb-6">
			<!-- 1/6 Time -->
			<span class="min-w-1 basis-1/6"></span>

			<!-- Connecting Line -->
			<div class="flex w-[50px] justify-center transition-all duration-500 md:w-[75px]">
				<span class="bg-text absolute z-0 h-full w-[4px]"></span>
			</div>

			<!-- 5/6 -->
			<div class="flex basis-5/6 flex-col">
				<span class="text-base font-bold">Warnings:</span>
				<div class="flex flex-col gap-y-1">
					{#each section.messages as message}
						<div class="flex items-start gap-x-2">
							<GeneralWarning />
							<span class="text-sm font-semibold">{message.content}</span>
						</div>
					{/each}
				</div>
			</div>
		</div>
	{/if}

	<!-- ViaStops -->
	{#if section.viaStops.length ?? 0 > 0}
		<div class="relative flex min-h-fit flex-row pb-4">
			<!-- 1/6 Time -->
			<span class="min-w-1 basis-1/6"></span>

			<!-- Connecting Line -->
			<div class="flex w-[50px] justify-center transition-all duration-500 md:w-[75px]">
				<span class="bg-text absolute z-0 h-full w-[4px]"></span>
			</div>

			<!-- 5/6 Button -->
			<button
				class="flex w-full basis-5/6 cursor-pointer flex-row items-center gap-x-2 text-left"
				onclick={() => (showViaStops = !showViaStops)}
			>
				<span class="font-light tracking-wide"
					>{section.viaStops.length ?? 0} stop{(section.viaStops.length ?? 0) === 1 ? "" : "s"}</span
				>

				{#if !showViaStops}
					<ChevronDown color="#ffda0a" />
				{:else}
					<ChevronUp color="#ffda0a" />
				{/if}
			</button>
		</div>
	{/if}

	{#if showViaStops}
		{#each section.viaStops as stop, i}
			<RouteViaStop {stop} />
		{/each}
	{/if}

	<!-- Destination -->
	<RouteStop stop={section.destination} showDeparture={false} position="end" />
</div>
