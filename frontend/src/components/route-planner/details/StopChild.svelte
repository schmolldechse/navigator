<script lang="ts">
	import type { Time } from "$models/time";
	import type { Stop } from "$models/station";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import ChevronRight from "lucide-svelte/icons/chevron-right";
	import Platform from "$components/ui/info/Platform.svelte";

	let { time, stop, showBothTimes = false, class: className = "" }: {
		time?: Time,
		stop?: Stop,
		showBothTimes?: boolean,
		class?: string
	} = $props();
</script>

<div class={["flex flex-row", className]}>
	<!-- 1/6 Time -->
	{#if !showBothTimes}
		<TimeInformation {time} direction="col" class="items-end basis-1/6 text-base"
						 delayClass="text-sm md:text-base" />
	{:else}
		<div class="flex items-end basis-1/6">
			<TimeInformation time={stop?.arrival} class="text-base" delayClass="text-sm md:text-base" />
			<TimeInformation time={stop?.departure} class="text-base" delayClass="text-sm md:text-base" />
		</div>
	{/if}

	<!-- 1/6 Connecting Line -->
	<div class="relative flex justify-center basis-1/6 lg:max-w-[5%]">
		<CircleDot class="bg-background absolute z-1 shrink-0" />
		<span class="bg-text absolute z-0 h-full w-[4px]"></span>
	</div>

	<!-- 4/6 Stop Info -->
	<div class="flex basis-4/6 flex-row justify-between">
		<div class="flex flex-row items-center">
			<a class="font-bold break-words hyphens-auto" href={`/${stop?.evaNumber}/departures`} target="_blank">
				{stop?.name}
			</a>
			<ChevronRight color="#ffda0a" class="shrink-0" />
		</div>

		<Platform {time} class="text-right" direction="col" />
	</div>
</div>
