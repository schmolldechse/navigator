<script lang="ts">
	import { onMount } from "svelte";
	import { type Marker as LeafletMarker, type Map as LeafletMap } from "leaflet";
	import "leaflet/dist/leaflet.css";
	import type { StationSummaryDTO } from "$lib/models/StationSummaryDTO";
	import StationPopup from "./StationPopup.svelte";

	let {
		stations = $bindable(),
		onresize,
		onselect
	}: {
		stations: StationSummaryDTO[];
		onresize: (coords: { latitude: number; longitude: number; radius: number }) => void;
		onselect?: (station: StationSummaryDTO) => void;
	} = $props();

	let mapLoaded: boolean = $state(false);
	let selectedStation: StationSummaryDTO | null = $state(null);

	let L: typeof import("leaflet");
	let map: LeafletMap;
	let mapContainer: HTMLDivElement;
	const markers = new Map<number, LeafletMarker>();

	let debounceTimer: NodeJS.Timeout;
	const debounce = (func: () => void, delay: number) => {
		if (debounceTimer) clearTimeout(debounceTimer);
		debounceTimer = setTimeout(func, delay);
	};

	const handleMapChange = () => {
		const center = map.getCenter();
		const bounds = map.getBounds();
		const distance = bounds.getNorthEast().distanceTo(bounds.getSouthWest());
		onresize({ latitude: center.lat, longitude: center.lng, radius: distance });
	};

	onMount(async () => {
		L = await import("leaflet");

		map = L.map(mapContainer, { attributionControl: false, zoomControl: false }).setView([50.1066819, 8.66282825], 14);
		const tileLayer = L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", { maxZoom: 19 }).addTo(map);

		tileLayer.on("load", () => {
			if (mapLoaded) return;
			mapLoaded = true;

			handleMapChange();
		});

		map.on("zoom", () => debounce(() => handleMapChange(), 300));
		map.on("move", () => debounce(() => handleMapChange(), 300));
	});

	$effect(() => {
		if (!mapLoaded || !map) return;

		stations.forEach((station: StationSummaryDTO) => {
			if (markers.has(station.evaNumber)) return;

			const marker = L.marker([station.position.latitude, station.position.longitude], { riseOnHover: true }).addTo(map);
			marker.on("click", () => {
				onselect?.(station);
				selectedStation = station;
			});

			markers.set(station.evaNumber, marker);
		});
	});
</script>

<svelte:head>
	<meta name="viewport" content="width=device-width; initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
</svelte:head>

<div bind:this={mapContainer} class={["h-full w-full rounded-xl"]}></div>

{#if selectedStation}
	<StationPopup bind:station={selectedStation} />
{/if}
