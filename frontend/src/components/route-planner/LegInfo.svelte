<script lang="ts">
	import type { Connection, LineColor } from "$models/connection";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import { ChevronRight } from "lucide-svelte";
	import Platform from "$components/ui/info/Platform.svelte";
	import { formatDuration } from "$lib";
	import ChevronDown from "lucide-svelte/icons/chevron-down";
	import ChevronUp from "lucide-svelte/icons/chevron-up";

	let { leg, lineColor }: { leg: Connection, lineColor?: LineColor } = $props();
	let showViaStops = $state<boolean>(false);
</script>

<div class="flex flex-col text-base">
	<!-- Origin -->
	<div class="flex flex-row">
		<TimeInformation time={leg?.departure} direction="col" class="basis-1/5 text-base items-end"
						 delayClass="text-sm md:text-base" />
		<div class="relative basis-[15%] md:basis-[5%] flex justify-center">
			<CircleDot class="absolute shrink-0 z-10 bg-background self-start" />
			<span class="absolute bg-text h-full w-[4px] z-0 self-end"></span>
		</div>
		<a class="basis-3/5 font-bold w-fit flex flex-row" href={`/${leg?.origin?.evaNumber}/departures`}
		   target="_blank">
			{leg?.origin?.name}
			<ChevronRight color="#ffda0a" class="shrink-0" />
		</a>
		<Platform time={leg?.departure} class="basis-1/5 text-right" direction="col" />
	</div>

	<!-- Line Info -->
	<div class="relative flex flex-row pt-6 md:pt-12">
		<span class="basis-1/5 text-sm text-right">{formatDuration(leg?.arrival, leg?.departure)}</span>
		<div class="basis-[15%] md:basis-[5%] flex justify-center">
			<span class="absolute bg-text h-full inset-y-4 w-[4px] z-0 self-end"></span>
		</div>
		<div class="basis-3/5 flex flex-col">
			<div class="flex flex-row items-baseline gap-x-2">
			<span class="font-extrabold w-fit {lineColor?.shape}"
				  style:color={lineColor?.textColor ? lineColor?.textColor : "inherit"}
				  style:background-color={lineColor?.backgroundColor ? lineColor?.backgroundColor : "inherit"}
				  style:border-color={lineColor?.borderColor ? lineColor?.borderColor : "none"}
			>
						{leg?.lineInformation?.lineName}
					</span>
				<span>({leg?.lineInformation?.fahrtNr})</span>
			</div>
			<span class="text-text/65">{leg?.direction ? `Continues to ${leg.direction}` : ""}</span>
		</div>
		<span class="basis-1/5"></span>
	</div>

	<!-- ViaStops -->
	{#if leg?.viaStops?.length ?? 0 > 0}
		<div class="relative flex flex-row pt-6 md:pt-12">
			<span class="basis-1/5"></span>
			<div class="basis-[15%] md:basis-[5%] flex justify-center">
				<span class="absolute bg-text h-full inset-y-4 w-[4px] z-0 self-end"></span>
			</div>
			<button class="text-left basis-3/5 flex flex-row items-center gap-x-2 w-fit cursor-pointer"
					onclick={() => showViaStops = !showViaStops}>
				{leg?.viaStops?.length ?? 0} stop{leg?.viaStops?.length ?? 0 > 1 ? "s" : ""}

				{#if !showViaStops}
					<ChevronDown color="#ffda0a" />
				{:else}
					<ChevronUp color="#ffda0a" />
				{/if}
			</button>
			<span class="basis-1/5"></span>
		</div>
	{/if}

	{#if showViaStops}
		{#each leg?.viaStops ?? [] as stop, i}
			<div class="relative flex flex-row pt-2">
				<div class="basis-1/5 flex flex-col items-end">
					<TimeInformation time={stop?.arrival} noDoubleSpan={true} class="basis-1/5 text-base items-end"
									 delayClass="text-sm md:text-base" />
					<TimeInformation time={stop?.departure} noDoubleSpan={true} class="basis-1/5 text-base items-end"
									 delayClass="text-sm md:text-base" />
				</div>
				<div class="basis-[15%] md:basis-[5%] flex justify-center">
					<CircleDot class="absolute shrink-0 z-10 bg-background self-center" />
					<span class="absolute bg-text h-full w-[4px] z-0 self-end"></span>
				</div>
				<a class="basis-3/5 font-bold w-fit flex flex-row items-center self-center"
				   href={`/${stop?.evaNumber}/departures`}
				   target="_blank">
					{stop?.name}
					<ChevronRight color="#ffda0a" class="shrink-0" />
				</a>
				<Platform time={stop?.departure} class="basis-1/5 text-right self-center" changeClass="font-bold" direction="row"  />
			</div>
		{/each}
	{/if}

	<!-- Destination -->
	<div class="relative flex flex-row pt-4 md:pt-6">
		<TimeInformation time={leg?.arrival} direction="col" class="basis-1/5 text-base items-end"
						 delayClass="text-sm md:text-base" />
		<div class="basis-[15%] md:basis-[5%] flex justify-center">
			<CircleDot class="absolute shrink-0 z-10 bg-background self-end" />
			<span class="absolute bg-text h-full w-[4px] z-0 self-end"></span>
		</div>
		<a class="basis-3/5 font-bold w-fit flex flex-row items-center self-end"
		   href={`/${leg?.destination?.evaNumber}/departures`}
		   target="_blank">
			{leg?.destination?.name}
			<ChevronRight color="#ffda0a" class="shrink-0" />
		</a>
		<Platform time={leg?.arrival} class="basis-1/5 text-right self-end" direction="col" />
	</div>
</div>