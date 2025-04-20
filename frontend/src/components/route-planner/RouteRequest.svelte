<script lang="ts">
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import CornerDownRight from "lucide-svelte/icons/corner-down-right";
	import type { Station } from "$models/station";

	let {
		from,
		to
	}: {
		from: Station | undefined;
		to: Station | undefined;
	} = $props();
</script>

<div class="flex flex-col items-start gap-y-1">
	<span class="text-2xl font-bold">Route</span>
	{#if !from && !to}
		<div class="flex flex-row items-center gap-x-2">
			<CircleDot />
			<span class="bg-primary-dark/40 h-[2rem] w-[12rem] animate-pulse rounded-2xl text-lg font-medium"></span>
		</div>

		<div class="flex flex-row items-center gap-x-2">
			<CornerDownRight />
			<span class="bg-primary-dark/40 h-[2rem] w-[12rem] animate-pulse rounded-2xl text-lg font-medium"></span>
		</div>
	{:else}
		<div class="flex flex-row items-center gap-x-2">
			<CircleDot />
			<span class="text-lg font-medium">{from?.name}</span>
		</div>

		<div class="flex flex-row items-center gap-x-2">
			<CornerDownRight />
			<span class="text-lg font-medium">{to?.name}</span>
		</div>
	{/if}
</div>
