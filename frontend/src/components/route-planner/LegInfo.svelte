<script lang="ts">
	import type { Connection } from "$models/connection";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import CircleDot from "lucide-svelte/icons/circle-dot";

	let { leg }: { leg: Connection } = $props();
</script>

<div class="flex text-base">
	<div class="flex-1 flex flex-row gap-x-2 justify-end">
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
	<div class="flex-4 flex flex-col">
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
		<span class="bg-green-500 w-full">hey</span>
	</div>
	<span class="bg-yellow-500 w-full flex-1">hey</span>
</div>

<!--
<div class="flex items-start w-[75%] gap-x-4 text-sm">
	<div class="flex flex-col items-end flex-1">
		<TimeInformation time={leg?.departure} direction="col" class="text-base items-end" delayClass="text-sm md:text-lg" />
	</div>

	<div class="flex-shrink-0 circle bg-black rounded-full mt-1"></div>

	<span class="flex-5">{leg?.origin?.name}</span>

	<span class="flex-1">{leg?.departure?.plannedPlatform}</span>
</div>
-->