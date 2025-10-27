<script lang="ts">
	import Map from "$lib/components/maps/Map.svelte";
	import type { StationSummaryDTO } from "$lib/models/StationSummaryDTO";
	import type { PageProps } from "./$types";
	import { getStations } from "./stations.remote";

	let { data }: PageProps = $props();
	let stations: StationSummaryDTO[] = $state(data.stations);
</script>

<svelte:head>
	<title>Station Map - Navigator</title>
</svelte:head>

<div class="relative flex-1 p-4 md:py-8">
	<Map
		bind:stations
		onresize={async ({ latitude, longitude, radius }) => {
			const newStations = await getStations({ latitude, longitude, radius });

			const allStations = [...stations, ...newStations];
			stations = Array.from(
				new globalThis.Map(allStations.map((station: StationSummaryDTO) => [station.evaNumber, station])).values()
			);
		}}
		onselect={(station: StationSummaryDTO) => {}}
	/>
</div>

<style>
	:global(html, body) {
		@apply flex h-screen flex-col overflow-hidden;
	}
</style>
