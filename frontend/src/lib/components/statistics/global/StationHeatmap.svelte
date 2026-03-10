<script lang="ts">
	import { onMount } from "svelte";
	import "leaflet/dist/leaflet.css";
	import { Map as LeafletMap, TileLayer, LatLngBounds } from "leaflet";
	import { HeatmapLayer, type HeatPoint } from "@lib/leafletImplementation/heatmapLayer";

	interface Props {
		heatPoints: HeatPoint[];
		class?: string;
	}

	let { heatPoints, class: classes = "" }: Props = $props();

	let mapContainer: HTMLDivElement | undefined = $state(undefined);

	let map: LeafletMap;
	let heatLayer: HeatmapLayer;

	onMount(async () => {
		if (!mapContainer) return;

		map = new LeafletMap(mapContainer, {
			attributionControl: false,
			zoomControl: false
		}).setView([50.1066819, 8.66282825], 6);
		new TileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", { maxZoom: 19 }).addTo(map);

		heatLayer = new HeatmapLayer([], {
			radius: 20,
			blur: 15,
			maxIntensity: 0 // auto-detect
		});
		heatLayer.addTo(map);

		updateHeatmap();
	});

	const updateHeatmap = () => {
		if (!map || !heatLayer || heatPoints.length === 0) return;

		heatLayer.setData(heatPoints);

		// fit map bounds to the data points with padding
		const bounds = new LatLngBounds(heatPoints.map((p) => [p[0], p[1]]));
		map.fitBounds(bounds, { padding: [40, 40] });
	};

	$effect(() => {
		// reactive dependency on heatPoints
		if (heatPoints && map && heatLayer) {
			updateHeatmap();
		}
	});
</script>

<svelte:head>
	<meta name="viewport" content="width=device-width; initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
</svelte:head>

<div bind:this={mapContainer} class={classes}></div>
