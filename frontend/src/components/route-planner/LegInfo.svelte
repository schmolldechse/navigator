<script lang="ts">
	import type { Connection } from "$models/connection";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import Platform from "$components/ui/info/Platform.svelte";
	import ChevronRight from "lucide-svelte/icons/chevron-right";

	let { leg }: { leg: Connection } = $props();
</script>

<!-- max width on larger screens -->
<div class="flex text-base gap-x-2 md:max-w-[85%]">
	<!-- Connecting Line-Area; must be larger on smaller screens -->
	<div class="flex-2 md:flex-1 flex flex-row gap-x-2 justify-end max-w-[20%]">
		<div class="flex flex-col justify-between">
			<TimeInformation time={leg?.departure} direction="col" class="text-base items-end"
							 delayClass="text-sm md:text-base" />
			<TimeInformation time={leg?.arrival} direction="col" class="text-base items-end"
							 delayClass="text-sm md:text-base" />
		</div>
		<div class="flex flex-col justify-between">
			<CircleDot class="shrink-0" />
			<span class="relative mt-[-0.25rem] inset-y-0.5 left-[0.625rem] w-[4px] h-full bg-white"></span>
			<CircleDot class="shrink-0" />
		</div>
	</div>

	<!-- Leg Info -->
	<div class="flex-5 flex flex-col justify-between">
		<!-- Origin -->
		<div class="flex flex-row justify-between">
			<a class="font-bold w-full flex flex-row" href={`/${leg?.origin?.evaNumber}/departures`} target="_blank">
				{leg?.origin?.name}
				<ChevronRight color="#ffda0a" />
			</a>
			<Platform time={leg?.departure} class="text-right md:max-w-[15%]" direction="col" />
		</div>

		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>

		<!-- Destination -->
		<div class="flex flex-row justify-between items-end">
			<a class="font-bold w-full flex flex-row" href={`/${leg?.destination?.evaNumber}/departures`} target="_blank">
				{leg?.destination?.name}
				<ChevronRight color="#ffda0a" />
			</a>
			<Platform time={leg?.arrival} class="text-right md:max-w-[15%]" direction="col" />
		</div>
	</div>
</div>