<script module lang="ts">
	type StationHeatmapPoint = {
		station: BaseStation;
		value: number;
	};

	export { type StationHeatmapPoint };
</script>

<script lang="ts">
	import { onMount, untrack } from "svelte";
	import "leaflet/dist/leaflet.css";
	import { Map as LeafletMap, TileLayer, type LatLngBounds } from "leaflet";
	import { HeatmapLayer, type HeatPoint } from "@lib/leafletImplementation/heatmapLayer";
	import { type BaseStation } from "@lib/api";
	import type { ClassValue } from "svelte/elements";
	import HeatmapMarker from "./marker/HeatmapMarker.svelte";
	import { HeatmapMarker as HeatmapMarkerImpl } from "./marker/HeatmapMarker";

	type Props = {
		stationHeatmapPoints: StationHeatmapPoint[];
		scale: [number, number];
		class?: ClassValue;
	};

	let { stationHeatmapPoints, scale = $bindable([0, 100]), class: classes = "" }: Props = $props();

	let mapContainer: HTMLDivElement | undefined = $state(undefined);

	let map: LeafletMap;
	let mapBounds: LatLngBounds | undefined = $state(undefined);

	let heatLayer: HeatmapLayer;

	let markers = new Map<number, HeatmapMarkerImpl<typeof HeatmapMarker>>();

	let visiblePoints = $derived.by(() => {
		if (!mapBounds) return [];

		return stationHeatmapPoints.filter((point: StationHeatmapPoint) =>
			mapBounds!.contains([Number(point.station.position.latitude), Number(point.station.position.longitude)])
		);
	});

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
			maxIntensity: 0
		});
		heatLayer.addTo(map);

		map.on("zoomend", () => {
			mapBounds = map.getBounds();
			updateDynamicHeatmapRadius();
		});
		map.on("moveend", () => (mapBounds = map.getBounds()));

		mapBounds = map.getBounds();
		updateDynamicHeatmapRadius();
	});

	const updateDynamicHeatmapRadius = () => {
		if (!map || !heatLayer) return;

		const zoom = map.getZoom();

		const minZoom = 5; // ~5 (country layer)
		const maxZoom = 18; // ~18 (street layer)

		const zoomProgress = Math.max(0, Math.min(1, (zoom - minZoom) / (maxZoom - minZoom)));

		const minRadius = 7.5; // very small base radius. prevents painting the whole map continuously red when points cluster heavily
		const maxRadius = 60; // large geographic radius. ensures single stations visibly cover their local surrounding area

		// exponential scale matches geographic distance zoom mechanics better than pure linear math
		const dynamicRadius = minRadius + (maxRadius - minRadius) * Math.pow(zoomProgress, 1.5);

		// low zoom -> nearly 100% blur ratio (smooths out dense clusters into broad regional "heat" zones, preventing intense hard red dots)
		// high zoom -> ~35% blur ratio (maintains a solid core heatmap color for an isolated station while gently fading out)
		const blurRatio = 0.95 - zoomProgress * 0.35;

		const dynamicBlur = dynamicRadius * blurRatio;

		heatLayer.setOptions({ radius: dynamicRadius, blur: dynamicBlur });
	};

	const updateScale = () => {
		if (!map || !stationHeatmapPoints || stationHeatmapPoints.length === 0) return;

		if (visiblePoints.length > 0) {
			const min = Math.min(...visiblePoints.map((point: StationHeatmapPoint) => point.value));
			const max = Math.max(...visiblePoints.map((point: StationHeatmapPoint) => point.value));

			scale = [min, max];
		} else scale = [0, 100];
	};

	const updateHeatmap = () => {
		if (!map || !heatLayer) return;

		heatLayer.setOptions({ maxIntensity: scale[1] });
		heatLayer.setData(
			visiblePoints.map((point: StationHeatmapPoint) => [
				Number(point.station.position.latitude),
				Number(point.station.position.longitude),
				point.value
			]) as HeatPoint[]
		);
	};

	const updateMarkers = () => {
		if (!map) return;

		const MARKER_ZOOM_THRESHOLD = 12;
		if (map.getZoom() < MARKER_ZOOM_THRESHOLD) {
			for (const marker of markers.values()) {
				marker.removeFrom(map);
			}
			markers.clear();
			return;
		}

		const nextVisibleEvaNumbers = new Set(visiblePoints.map((point: StationHeatmapPoint) => Number(point.station.evaNumber)));

		for (const [evaNumber, marker] of markers.entries()) {
			if (nextVisibleEvaNumbers.has(evaNumber)) continue;

			marker.removeFrom(map);
			markers.delete(evaNumber);
		}

		for (const point of visiblePoints) {
			const evaNumber = Number(point.station.evaNumber);
			if (markers.has(evaNumber)) continue;

			const marker = new HeatmapMarkerImpl(
				[Number(point.station.position.latitude), Number(point.station.position.longitude)],
				HeatmapMarker,
				{ point }
			);
			marker.addTo(map);
			markers.set(evaNumber, marker);
		}
	};

	$effect(() => {
		if (!map || !heatLayer) return;

		// React strictly to visible points (which are derived from stationHeatmapPoints & mapBounds)
		const currentVisiblePoints = visiblePoints;
		if (!currentVisiblePoints) return;

		untrack(() => {
			updateScale();
			updateHeatmap();
			updateMarkers();
		});
	});
</script>

<svelte:head>
	<meta name="viewport" content="width=device-width; initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
</svelte:head>

<div bind:this={mapContainer} class={classes}></div>
