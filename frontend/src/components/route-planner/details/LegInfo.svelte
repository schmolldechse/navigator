<script lang="ts">
	import type { Connection, LineColor } from "$models/connection";
	import { formatDuration } from "$lib";
	import ChevronDown from "lucide-svelte/icons/chevron-down";
	import ChevronUp from "lucide-svelte/icons/chevron-up";
	import StopChild from "$components/route-planner/details/StopChild.svelte";

	let { leg, lineColor, hasLastStop = false }: { leg: Connection; lineColor?: LineColor, hasLastStop?: boolean } = $props();
	let showViaStops = $state<boolean>(false);
</script>

<div class="flex flex-col">
	<!-- Origin -->
	<StopChild class="pb-8" time={leg?.departure} stop={leg?.origin} />

	<!-- Line Info -->
	<div class="relative flex flex-row pb-6">
		<!-- 1/6 Time -->
		<span class="basis-1/6 text-right text-sm">{formatDuration(leg?.arrival, leg?.departure)}</span>

		<!-- Connecting Line -->
		<div class="flex w-[50px] justify-center transition-all duration-500 md:w-[75px]">
			<span class="bg-text absolute z-0 h-full w-[4px]"></span>
		</div>

		<!-- 5/6 -->
		<div class="flex basis-5/6 flex-col">
			<!-- LineColor -->
			<div class="flex flex-row items-baseline gap-x-2">
				<span
					class="font-extrabold {lineColor?.shape}"
					style:color={lineColor?.textColor ? lineColor?.textColor : "inherit"}
					style:background-color={lineColor?.backgroundColor ? lineColor?.backgroundColor : "inherit"}
					style:border-color={lineColor?.borderColor ? lineColor?.borderColor : "none"}
				>
					{leg?.lineInformation?.lineName}
				</span>
				<span>({leg?.lineInformation?.fahrtNr})</span>
			</div>

			<!-- Continuation -->
			<span class="text-text/65">{leg?.direction ? `Continues to ${leg.direction}` : ""}</span>
		</div>
	</div>

	<!-- ViaStops -->
	{#if leg?.viaStops?.length ?? 0 > 0}
		<div class="relative flex min-h-fit flex-row pb-4">
			<!-- 1/6 Time -->
			<span class="basis-1/6"></span>

			<!-- Connecting Line -->
			<div class="flex w-[50px] justify-center transition-all duration-500 md:w-[75px]">
				<span class="bg-text absolute z-0 h-full w-[4px]"></span>
			</div>

			<!-- 5/6 Button -->
			<button
				class="flex w-full basis-5/6 cursor-pointer flex-row items-center gap-x-2 text-left"
				onclick={() => (showViaStops = !showViaStops)}
			>
				{leg?.viaStops?.length ?? 0} stop{(leg?.viaStops?.length ?? 0) === 1 ? "" : "s"}

				{#if !showViaStops}
					<ChevronDown color="#ffda0a" />
				{:else}
					<ChevronUp color="#ffda0a" />
				{/if}
			</button>
		</div>
	{/if}

	{#if showViaStops}
		{#each leg?.viaStops ?? [] as stop, i}
			<StopChild time={stop?.departure} {stop} showBothTimes={true} position="center" />
		{/each}
	{/if}

	<!-- Destination -->
	<StopChild class="pt-4" time={leg?.arrival} stop={leg?.destination} isLastStop={hasLastStop} position="end" />
</div>
