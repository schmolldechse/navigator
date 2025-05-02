<script lang="ts">
	import type { Time } from "$models/time";
	import type { Stop } from "$models/station";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import ChevronRight from "lucide-svelte/icons/chevron-right";
	import Platform from "$components/ui/info/Platform.svelte";

	let { time, stop, showBothTimes = false, isChangeover = false, position = "start", class: className = "" }: {
		time?: Time,
		stop?: Stop,
		showBothTimes?: boolean,
		isChangeover?: boolean,
		position?: "start" | "center" | "end",
		class?: string
	} = $props();
</script>

<div class={["relative flex flex-row min-h-fit", className]}>
	<!-- 1/6 Time -->
	{#if !showBothTimes}
		<TimeInformation {time} direction="col"
						 class={`items-end basis-1/6 text-base ${position === "end" ? "self-end" : ""}`}
						 delayClass="text-sm md:text-base" />
	{:else}
		<div class="flex flex-col items-end basis-1/6 self-center">
			<TimeInformation time={stop?.arrival} class="text-base" delayClass="text-sm md:text-base" />
			<TimeInformation time={stop?.departure} class="text-base" delayClass="text-sm md:text-base" />
		</div>
	{/if}

	<!-- Connecting Line -->
	<div class="flex justify-center w-[50px] md:w-[75px] transition-all duration-500"
		 class:items-end={position === "end"}
		 class:items-center={position === "center"}
	>
		<CircleDot class="bg-background absolute z-1 shrink-0" />
		<span class="bg-text absolute z-0 h-full w-[4px]" class:bg-text={!isChangeover}
			  class:changeover={isChangeover}></span>
	</div>

	<!-- 4/6 Stop Info -->
	<div class="flex basis-4/6 flex-row items-center">
		<a class="font-bold break-words hyphens-auto" href={`/${stop?.evaNumber}/departures`} target="_blank">
			{stop?.name}
		</a>
		<ChevronRight color="#ffda0a" class="shrink-0" />
	</div>

	<!-- 1/6 Platform -->
	<Platform {time} class={`basis-1/6 md:items-start ${position === "end" ? "self-end" : ""}`}
			  direction="col" />
</div>
