<script lang="ts">
	import { onMount } from "svelte";
	import "leaflet/dist/leaflet.css";
	import type { BaseStation } from "@lib/api/types.gen";
	import { DivIcon, Map as LeafletMap, Marker, TileLayer } from "leaflet";
	import { MarkerClusterGroup } from "@kristjan.esperanto/leaflet.markercluster";

	interface Props {
		stations: BaseStation[];
		onresize: ({ latitude, longitude, radius }: { latitude: number; longitude: number; radius: number }) => void;
		onselect?: (station: BaseStation) => void;
		class?: string;
	}

	let { stations = $bindable(), onresize, onselect, class: classes = "" }: Props = $props();

	let mapContainer: HTMLDivElement | undefined = $state(undefined);

	let map: LeafletMap;

	let mapMarkerClusterGroup: MarkerClusterGroup;
	let markersMap: Map<number, Marker> = new Map();

	let debounceTimer: NodeJS.Timeout;
	const debounce = (func: () => void, delay: number) => {
		if (debounceTimer) clearTimeout(debounceTimer);
		debounceTimer = setTimeout(func, delay);
	};

	const handleMapChange = () => {
		if (!map) return;

		const center = map.getCenter();
		const bounds = map.getBounds();
		const distance = bounds.getNorthEast().distanceTo(bounds.getSouthWest());
		onresize({ latitude: center.lat, longitude: center.lng, radius: distance });
	};

	onMount(async () => {
		if (!mapContainer) return;

		map = new LeafletMap(mapContainer, {
			attributionControl: false,
			zoomControl: false
		}).setView([50.1066819, 8.66282825], 14);
		new TileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", { maxZoom: 19 }).addTo(map);

		mapMarkerClusterGroup = new MarkerClusterGroup({
			showCoverageOnHover: false,
			zoomToBoundsOnClick: true,
			maxClusterRadius: 50,
			iconCreateFunction: (cluster) => {
				const childCount = cluster.getChildCount();

				let c = " custom-cluster-";
				let size = 40;

				if (childCount < 10) {
					c += "small";
					size = 35;
				} else if (childCount < 100) {
					c += "medium";
					size = 45;
				} else {
					c += "large";
					size = 55;
				}

				return new DivIcon({
					html: `<div><span>${childCount}</span></div>`,
					className: "custom-cluster" + c,
					iconSize: [size, size],
					// center the icon anchor so it sits exactly on the coordinate
					iconAnchor: [size / 2, size / 2]
				});
			}
		});
		map.addLayer(mapMarkerClusterGroup);

		map.on("zoom", () => debounce(() => handleMapChange(), 300));
		map.on("move", () => debounce(() => handleMapChange(), 300));
	});

	$effect(() => {
		if (!map || !mapMarkerClusterGroup) return;

		// remove station markers that are no longer present
		const currentStationIds = new Set(stations.map((station: BaseStation) => Number(station.evaNumber)));
		for (const [evaNumber, marker] of markersMap) {
			if (currentStationIds.has(evaNumber)) continue;

			mapMarkerClusterGroup.removeLayer(marker);
			markersMap.delete(evaNumber);
		}

		// add new station markers
		stations.forEach((station: BaseStation) => {
			if (markersMap.has(Number(station.evaNumber))) return;

			const marker = new Marker([Number(station.position.latitude), Number(station.position.longitude)], { riseOnHover: true });
			marker.on("click", () => onselect?.(station));

			mapMarkerClusterGroup.addLayer(marker);
			markersMap.set(Number(station.evaNumber), marker);
		});
	});
</script>

<svelte:head>
	<meta name="viewport" content="width=device-width; initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
	<style>
		/* The outer ring (semi-transparent yellow) */
		.custom-cluster {
			background-color: rgba(255, 218, 10, 0.4);
			border-radius: 50%;
			text-align: center;
			box-sizing: border-box;
		}

		/* The inner circle (solid yellow with black text) */
		.custom-cluster div {
			background-color: #ffda0a;
			color: #0a0a0a;
			font-weight: 700;
			font-family: sans-serif;
			border-radius: 50%;
			/* Center the number vertically and horizontally */
			display: flex;
			justify-content: center;
			align-items: center;
			/* Sizing relative to outer ring */
			width: 84%;
			height: 84%;
			margin: 8%;
		}

		/* Specific sizing and font tweaks based on count */
		.custom-cluster-small div {
			font-size: 14px;
		}
		.custom-cluster-medium div {
			font-size: 16px;
		}
		.custom-cluster-large div {
			font-size: 18px;
		}
	</style>
</svelte:head>

<div bind:this={mapContainer} class={["h-full w-full rounded-xl", classes]}></div>
