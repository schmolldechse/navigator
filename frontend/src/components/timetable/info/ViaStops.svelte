<script lang="ts">
	import type { Stop } from "$models/station";
	import { writeStop } from "$lib";
	import ShowMore from "$components/timetable/info/ShowMore.svelte";

	let expanded = $state<boolean>(false);
	let { viaStops }: { viaStops?: Stop[] } = $props();
</script>

<span>
	{#if viaStops}
		{#if expanded}
			{viaStops.map((stop) => writeStop(stop, stop.name)).join(" - ")}
		{:else}
			{viaStops.slice(0, 3).map((stop) => writeStop(stop, stop.name)).join(" - ")}
		{/if}
	{/if}
</span>

{#if viaStops && viaStops.length > 3}
	<ShowMore onclick={() => expanded = !expanded} />
{/if}