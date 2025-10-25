<script lang="ts">
	import { onMount } from "svelte";
	import "leaflet/dist/leaflet.css";

	let map;
	let mapContainer: HTMLDivElement;

	onMount(async () => {
		const L = await import("leaflet");
		map = L.map(mapContainer).setView([48.7758, 9.1829], 13);
		L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
			maxZoom: 19,
			attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
		}).addTo(map);
	});
</script>

<svelte:head>
	<meta name="viewport" content="width=device-width; initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
</svelte:head>

<div bind:this={mapContainer} class="h-full w-full rounded-xl"></div>
