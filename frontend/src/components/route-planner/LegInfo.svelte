<script lang="ts">
	import type { Connection } from "$models/connection";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";

	let { leg }: { leg: Connection } = $props();
</script>

<div class="flex items-start w-[75%] gap-x-4 text-sm">
	<!-- origin -->
	<div class="flex flex-col items-end flex-1">
		<TimeInformation time={leg?.departure} direction="col" class="text-base items-end" delayClass="text-sm md:text-lg" />
	</div>

	<div class="flex-shrink-0 circle bg-black rounded-full mt-1"></div>

	<span class="flex-5">{leg?.origin?.name}</span>

	<span class="flex-1">{leg?.departure?.plannedPlatform}</span>
</div>

<style>
	.circle {
		shape-outside: circle(50%);
		clip-path: circle(6rem at 12rem 8rem);
	}
</style>