<script lang="ts">
	import type { Connection, LineColor } from "$models/connection";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import ChevronRight from "lucide-svelte/icons/chevron-right";
	import Platform from "$components/ui/info/Platform.svelte";
	import { formatDuration } from "$lib";
	import ChevronDown from "lucide-svelte/icons/chevron-down";
	import ChevronUp from "lucide-svelte/icons/chevron-up";
	import StopChild from "$components/route-planner/details/StopChild.svelte";

	let { leg, lineColor }: { leg: Connection; lineColor?: LineColor } = $props();
	let showViaStops = $state<boolean>(false);
</script>

<div class="flex flex-col text-base">
	<!-- Origin -->
	<StopChild time={leg?.departure} stop={leg?.origin} />

	<!-- Line Info -->
	<div class="flex flex-row">
		<!-- 1/6 Time -->
		<span class="basis-1/6 text-right text-sm">{formatDuration(leg?.arrival, leg?.departure)}</span>

		<!-- Connecting Line -->
		<div class="relative flex basis-1/6 justify-center md:max-w-[5%]">
			<span class="bg-text absolute z-0 h-full w-[4px]"></span>
		</div>
		<div class="flex basis-4/6 flex-col">
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
			<span class="text-text/65">{leg?.direction ? `Continues to ${leg.direction}` : ""}</span>
		</div>
	</div>

	<!-- ViaStops -->
	{#if leg?.viaStops?.length ?? 0 > 0}
		<div class="relative flex flex-row pt-6 md:pt-12">
			<span class="basis-1/6"></span>
			<div class="flex basis-1/6 justify-center md:max-w-[5%]">
				<span class="bg-text absolute z-0 h-full w-[4px] self-end"></span>
			</div>
			<button
				class="flex basis-4/6 cursor-pointer flex-row items-center gap-x-2 text-left"
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
			<div class="relative flex flex-row pt-2">
				<div class="flex basis-1/6 flex-col items-end">
					<TimeInformation
						time={stop?.arrival}
						noDoubleSpan={true}
						class="items-end text-base"
						delayClass="text-sm md:text-base"
					/>
					<TimeInformation
						time={stop?.departure}
						noDoubleSpan={true}
						class="items-end text-base"
						delayClass="text-sm md:text-base"
					/>
				</div>
				<div class="flex basis-1/6 justify-center md:max-w-[5%]">
					<CircleDot class="bg-background absolute z-10 shrink-0 self-center" />
					<span class="bg-text absolute z-0 h-full w-[4px] self-end"></span>
				</div>
				<div class="flex basis-4/6 flex-row items-center justify-between">
					<a class="flex flex-row font-bold" href={`/${stop?.evaNumber}/departures`} target="_blank">
						{stop?.name}
						<ChevronRight color="#ffda0a" class="shrink-0" />
					</a>
					<Platform time={stop?.departure} class="self-center text-right" changeClass="font-bold" direction="row" />
				</div>
			</div>
		{/each}
	{/if}

	<!-- Destination -->
	<div class="relative flex flex-row pt-4 md:pt-6">
		<TimeInformation
			time={leg?.arrival}
			direction="col"
			class="basis-1/6 items-end self-end text-base"
			delayClass="text-sm md:text-base"
		/>
		<div class="flex basis-1/6 justify-center md:max-w-[5%]">
			<CircleDot class="bg-background absolute z-10 shrink-0 self-end" />
			<span class="bg-text absolute z-0 h-full w-[4px] self-end"></span>
		</div>
		<div class="flex basis-4/6 flex-row items-center justify-between self-end">
			<a class="flex flex-row self-end font-bold" href={`/${leg?.destination?.evaNumber}/departures`} target="_blank">
				{leg?.destination?.name}
				<ChevronRight color="#ffda0a" class="shrink-0 self-center" />
			</a>
			<Platform time={leg?.arrival} class="self-end text-right" direction="col" />
		</div>
	</div>
</div>
