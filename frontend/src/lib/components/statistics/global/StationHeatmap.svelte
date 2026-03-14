<script module lang="ts">
	type StationHeatmapPoint = {
		station: BaseStation;
		value: number;
	};

	export { type StationHeatmapPoint };
</script>

<script lang="ts">
	import { onMount } from "svelte";
	import "leaflet/dist/leaflet.css";
	import { Map as LeafletMap, TileLayer } from "leaflet";
	import { HeatmapLayer, type HeatPoint } from "@lib/leafletImplementation/heatmapLayer";
	import { type BaseStation } from "@lib/api";

	type Props = {
		stationHeatmapPoints: StationHeatmapPoint[];
		class?: string;
	};

	let { stationHeatmapPoints, class: classes = "" }: Props = $props();

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
		if (!map || !heatLayer || stationHeatmapPoints.length === 0) return;

		const heatmapPoints = stationHeatmapPoints.map((point: StationHeatmapPoint) => [
			Number(point.station.position.latitude),
			Number(point.station.position.longitude),
			point.value
		]) as HeatPoint[];
		heatLayer.setData(heatmapPoints);
	};

	$effect(() => {
		if (!map || !heatLayer) return;
		if (!stationHeatmapPoints) return;

		updateHeatmap();
	});
</script>

<svelte:head>
	<meta name="viewport" content="width=device-width; initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
</svelte:head>

<div bind:this={mapContainer} class={classes}></div>
