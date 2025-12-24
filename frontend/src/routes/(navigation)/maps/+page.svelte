<script lang="ts">
	import StationMap from "@lib/components/maps/StationMap.svelte";
	import type { PageProps } from "./$types";
	import type { BaseStation } from "@lib/api/types.gen";
	import { findNearbyStations } from "@lib/remote/geostation.remote";
	import SelectedStationDialog from "@lib/components/maps/SelectedStationDialog.svelte";

	let { data }: PageProps = $props();

	let stations: BaseStation[] = $state(data.stations);

	let stationDialogVisible: boolean = $state(false);
	let selectedStation: BaseStation | null = $state(null);
</script>

<svelte:head>
	<title>Station Map - Navigator</title>
</svelte:head>

<main class="flex-1 p-4 sm:py-8">
	<StationMap
		{stations}
		onresize={async ({ latitude, longitude, radius }) => {
			const newStations = await findNearbyStations({ latitude, longitude, maxDistance: radius });

			const allStations = [...stations, ...newStations];
			stations = Array.from(
				new globalThis.Map(allStations.map((station: BaseStation) => [station.evaNumber, station])).values()
			);
		}}
		onselect={(station: BaseStation) => {
			selectedStation = station;
			stationDialogVisible = true;
		}}
		class="z-10"
	/>

	{#if selectedStation}
		<SelectedStationDialog
			isVisible={stationDialogVisible}
			station={selectedStation}
			onclose={() => {
				stationDialogVisible = false;
				selectedStation = null;
			}}
			class="z-20"
		/>
	{/if}
</main>

<style>
	:global(html, body) {
		@apply flex h-screen flex-col overflow-hidden;
	}
</style>
