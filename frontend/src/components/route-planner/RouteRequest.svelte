<script lang="ts">
	import type { Station } from "$models/station";
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import CornerDownRight from "lucide-svelte/icons/corner-down-right";

	let { stations }: { stations: Promise<[Station, Station]> } = $props();
</script>

<div class="flex flex-col items-start gap-y-1">
	<span class="font-bold text-2xl">Your requested route</span>
	{#await stations}
		<div class="items-center flex flex-row gap-x-2">
			<CircleDot />
			<span class="rounded-2xl bg-primary-dark/40 text-lg font-medium animate-pulse h-[2rem] w-[12rem]"></span>
		</div>

		<div class="flex flex-row items-center gap-x-2">
			<CornerDownRight />
			<span class="rounded-2xl bg-primary-dark/40 text-lg font-medium animate-pulse h-[2rem] w-[12rem]"></span>
		</div>
	{:then [from, to]}
		<div class="items-center flex flex-row gap-x-2">
			<CircleDot />
			<span class="text-lg font-medium">{from?.name}</span>
		</div>

		<div class="flex flex-row items-center gap-x-2">
			<CornerDownRight />
			<span class="text-lg font-medium">{to?.name}</span>
		</div>
	{/await}
</div>