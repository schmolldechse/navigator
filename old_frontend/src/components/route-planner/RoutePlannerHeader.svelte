<script lang="ts">
	import type { Station } from "$models/models";
	import CornerDownRight from "@lucide/svelte/icons/corner-down-right";
	import CircleDot from "@lucide/svelte/icons/circle-dot";

	interface Props {
		stations: Promise<{ start: Station; destination: Station }>;
	}

	let { stations }: Props = $props();
</script>

<div class="container mx-auto">
	<span class="text-xl font-bold md:text-2xl">Route</span>

	{#await stations}
		<div class="flex flex-row items-center gap-x-2">
			<CircleDot class="h-[20px] w-[20px] md:h-[25px] md:w-[25px]" />
			<span class="bg-primary-dark/40 h-[2rem] w-[12rem] animate-pulse rounded-2xl text-lg font-medium"></span>
		</div>

		<div class="flex flex-row items-center gap-x-2">
			<CornerDownRight class="h-[20px] w-[20px] md:h-[25px] md:w-[25px]" />
			<span class="bg-primary-dark/40 h-[2rem] w-[12rem] animate-pulse rounded-2xl text-lg font-medium"></span>
		</div>
	{:then { start, destination }}
		<div class="flex flex-row items-center gap-x-2">
			<CircleDot class="h-[20px] w-[20px] md:h-[25px] md:w-[25px]" />
			<span class="text-normal font-medium tracking-tight md:text-lg">{start?.name}</span>
		</div>

		<div class="flex flex-row items-center gap-x-2">
			<CornerDownRight class="h-[20px] w-[20px] md:h-[25px] md:w-[25px]" />
			<span class="text-normal font-medium tracking-tight md:text-lg">{destination?.name}</span>
		</div>
	{/await}
</div>
