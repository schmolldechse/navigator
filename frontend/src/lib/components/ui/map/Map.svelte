<script module lang="ts">
	import type { StyleSpecification } from "maplibre-gl";

	type MapCoordinates = {
		latitude: number;
		longitude: number;
	};

	type MapMarker<TData = unknown> = MapCoordinates & {
		id: number | string;
		label?: string;
		data?: TData;
	};

	type MapMarkerRenderContext<TData = unknown> = {
		marker: MapMarker<TData>;
	};

	type MapMarkerAnchor =
		| "center"
		| "top"
		| "bottom"
		| "left"
		| "right"
		| "top-left"
		| "top-right"
		| "bottom-left"
		| "bottom-right";

	type MapMarkerOffset = [number, number];

	type MapHeatmapPoint = MapCoordinates & {
		id: number | string;
		value: number;
	};

	type MapHeatmapGradientStop = {
		density: number;
		color: string;
	};

	type MapHeatmapGradientInterpolator = (density: number) => string;

	type MapHeatmapGradient = MapHeatmapGradientInterpolator | readonly string[] | readonly MapHeatmapGradientStop[];

	type MapViewportChange = {
		center: MapCoordinates;
		zoom: number;
		bounds: {
			north: number;
			east: number;
			south: number;
			west: number;
		};
		radius: number;
	};

	type MapStyle = StyleSpecification | string;

	export type {
		MapCoordinates,
		MapMarker,
		MapMarkerAnchor,
		MapMarkerOffset,
		MapMarkerRenderContext,
		MapHeatmapPoint,
		MapHeatmapGradient,
		MapHeatmapGradientInterpolator,
		MapHeatmapGradientStop,
		MapViewportChange,
		MapStyle
	};
</script>

<script lang="ts" generics="TData = unknown">
	import { mount, onMount, unmount, type Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import maplibregl, { type GeoJSONSource, type Map as MapLibreMap } from "maplibre-gl";
	import "maplibre-gl/dist/maplibre-gl.css";
	import MapMarkerRenderer from "./MapMarkerRenderer.svelte";

	type Props<TMarkerData = unknown> = {
		center?: MapCoordinates;
		zoom?: number;
		style?: MapStyle;
		markers?: MapMarker<TMarkerData>[];
		marker?: Snippet<[MapMarkerRenderContext<TMarkerData>]>;
		markerAnchor?: MapMarkerAnchor;
		markerOffset?: MapMarkerOffset;
		markerRenderPadding?: number;
		markerMinZoom?: number;
		heatmap?: boolean;
		heatmapPoints?: MapHeatmapPoint[];
		heatmapGradient?: MapHeatmapGradient;
		scale?: [number, number];
		navigationControl?: boolean;
		attributionControl?: boolean;
		ariaLabel?: string;
		class?: ClassValue;
		onmoveend?: (viewport: MapViewportChange) => void;
		onmarkerselect?: (marker: MapMarker<TMarkerData>) => void;
	};

	const DEFAULT_STYLE: MapStyle = {
		version: 8,
		sources: {},
		layers: []
	};

	const BASE_SOURCE_ID = "navigator-map-osm-tiles";
	const BASE_LAYER_ID = "navigator-map-osm-tiles";
	const HEATMAP_SOURCE_ID = "navigator-map-heatmap";
	const HEATMAP_LAYER_ID = "navigator-map-heatmap-layer";
	const HEATMAP_SCALE_PADDING = 128;
	const HEATMAP_INTERPOLATOR_STOP_COUNT = 12;
	const DEFAULT_HEATMAP_GRADIENT: MapHeatmapGradientStop[] = [
		{ density: 0, color: "rgba(0, 0, 0, 0)" },
		{ density: 0.08, color: "rgba(0, 42, 255, 0.08)" },
		{ density: 0.18, color: "rgba(0, 42, 255, 0.36)" },
		{ density: 0.34, color: "rgba(0, 26, 255, 0.72)" },
		{ density: 0.5, color: "rgba(38, 0, 255, 0.88)" },
		{ density: 0.68, color: "rgba(190, 0, 120, 0.94)" },
		{ density: 0.82, color: "rgba(255, 0, 35, 0.98)" },
		{ density: 1, color: "#ff1700" }
	];

	let {
		center = { latitude: 50.1066819, longitude: 8.66282825 },
		zoom = 12,
		style = DEFAULT_STYLE,
		markers = [],
		marker: markerContent,
		markerAnchor = "center",
		markerOffset,
		markerRenderPadding = 128,
		markerMinZoom = 0,
		heatmap = false,
		heatmapPoints = [],
		heatmapGradient = DEFAULT_HEATMAP_GRADIENT,
		scale = $bindable([0, 100] as [number, number]),
		navigationControl = true,
		attributionControl = true,
		ariaLabel = "Interactive map",
		class: className,
		onmoveend,
		onmarkerselect
	}: Props<TData> = $props();

	let mapContainer: HTMLDivElement | undefined = $state(undefined);
	let map: MapLibreMap | undefined = $state(undefined);
	let styleLoaded = $state(false);
	let customMarkerById = new globalThis.Map<
		string,
		{
			marker: maplibregl.Marker;
			component: ReturnType<typeof mount>;
			source: MapMarker<TData>;
			content: Snippet<[MapMarkerRenderContext<TData>]>;
		}
	>();
	let customMarkerSyncFrame: number | undefined;
	let pendingCustomMarkerSync:
		| {
				items: MapMarker<TData>[];
				content: Snippet<[MapMarkerRenderContext<TData>]>;
				renderPadding: number;
				minZoom: number;
		  }
		| undefined;
	let heatmapScaleFrame: number | undefined;
	let resizeObserver: ResizeObserver | undefined;

	const isFiniteCoordinate = ({ latitude, longitude }: MapCoordinates) =>
		Number.isFinite(latitude) &&
		Number.isFinite(longitude) &&
		latitude >= -90 &&
		latitude <= 90 &&
		longitude >= -180 &&
		longitude <= 180;

	const isValidHeatmapPoint = (point: MapHeatmapPoint) => isFiniteCoordinate(point) && Number.isFinite(point.value);

	const createHeatmapFeatureCollection = (items: MapHeatmapPoint[]) =>
		({
			type: "FeatureCollection",
			features: items.filter(isValidHeatmapPoint).map((point: MapHeatmapPoint) => ({
				type: "Feature",
				id: String(point.id),
				properties: {
					id: String(point.id),
					value: point.value
				},
				geometry: {
					type: "Point",
					coordinates: [point.longitude, point.latitude]
				}
			}))
		}) as unknown as Parameters<GeoJSONSource["setData"]>[0];

	const clampDensity = (density: number) => Math.min(1, Math.max(0, density));

	const normalizeHeatmapGradientStops = (stops: readonly MapHeatmapGradientStop[]) => {
		const normalizedStops = stops
			.filter(
				(stop): stop is MapHeatmapGradientStop =>
					typeof stop === "object" &&
					stop !== null &&
					Number.isFinite(stop.density) &&
					typeof stop.color === "string" &&
					stop.color.length > 0
			)
			.map((stop: MapHeatmapGradientStop) => ({
				density: clampDensity(stop.density),
				color: stop.color
			}))
			.sort((left, right) => left.density - right.density);

		if (normalizedStops.length === 0) return DEFAULT_HEATMAP_GRADIENT;
		if (normalizedStops[0].density > 0) {
			return [{ density: 0, color: "rgba(0, 0, 0, 0)" }, ...normalizedStops];
		}

		return normalizedStops;
	};

	const normalizeHeatmapGradient = (gradient: MapHeatmapGradient) => {
		if (typeof gradient === "function") {
			const stops: MapHeatmapGradientStop[] = [
				{ density: 0, color: "rgba(0, 0, 0, 0)" },
				...Array.from({ length: HEATMAP_INTERPOLATOR_STOP_COUNT }, (_, index) => {
					const value = index / (HEATMAP_INTERPOLATOR_STOP_COUNT - 1);

					return {
						density: 0.1 + value * 0.9,
						color: gradient(value)
					};
				})
			];

			return normalizeHeatmapGradientStops(stops);
		}

		if (gradient.length === 0) return DEFAULT_HEATMAP_GRADIENT;

		if (typeof gradient[0] === "string") {
			const colors = gradient.filter((color): color is string => typeof color === "string" && color.length > 0);

			if (colors.length === 0) return DEFAULT_HEATMAP_GRADIENT;
			if (colors.length === 1) {
				return [
					{ density: 0, color: "rgba(0, 0, 0, 0)" },
					{ density: 1, color: colors[0] }
				];
			}

			return [
				{ density: 0, color: "rgba(0, 0, 0, 0)" },
				...colors.map((color, index) => ({
					density: 0.1 + (index / (colors.length - 1)) * 0.9,
					color
				}))
			];
		}

		return normalizeHeatmapGradientStops(gradient as readonly MapHeatmapGradientStop[]);
	};

	const createHeatmapColorExpression = (gradient: MapHeatmapGradient) =>
		[
			"interpolate",
			["linear"],
			["heatmap-density"],
			...normalizeHeatmapGradient(gradient).flatMap((stop: MapHeatmapGradientStop) => [stop.density, stop.color])
		] as unknown as Parameters<MapLibreMap["setPaintProperty"]>[2];

	const getGeoJsonSource = (sourceId: string) => map?.getSource(sourceId) as GeoJSONSource | undefined;

	const getMarkerRenderPadding = () => (Number.isFinite(markerRenderPadding) ? Math.max(0, markerRenderPadding) : 128);
	const getMarkerMinZoom = () => (Number.isFinite(markerMinZoom) ? Math.max(0, markerMinZoom) : 0);

	const isCustomMarkerInRenderViewport = (marker: MapMarker<TData>, renderPadding: number) => {
		if (!map || !isFiniteCoordinate(marker)) return false;

		const canvas = map.getCanvas();
		const width = canvas.clientWidth;
		const height = canvas.clientHeight;

		if (width <= 0 || height <= 0) return false;

		const point = map.project([marker.longitude, marker.latitude]);

		return (
			point.x >= -renderPadding &&
			point.x <= width + renderPadding &&
			point.y >= -renderPadding &&
			point.y <= height + renderPadding
		);
	};

	const cancelCustomMarkerSync = () => {
		if (customMarkerSyncFrame !== undefined) {
			cancelAnimationFrame(customMarkerSyncFrame);
			customMarkerSyncFrame = undefined;
		}

		pendingCustomMarkerSync = undefined;
	};

	const cancelHeatmapScaleSync = () => {
		if (heatmapScaleFrame !== undefined) {
			cancelAnimationFrame(heatmapScaleFrame);
			heatmapScaleFrame = undefined;
		}
	};

	const clearCustomMarkers = () => {
		for (const mountedMarker of customMarkerById.values()) {
			unmount(mountedMarker.component);
			mountedMarker.marker.remove();
		}

		customMarkerById.clear();
	};

	const getDistanceInMeters = (start: MapCoordinates, end: MapCoordinates) => {
		const earthRadius = 6_371_000;
		const latA = (start.latitude * Math.PI) / 180;
		const latB = (end.latitude * Math.PI) / 180;
		const deltaLat = ((end.latitude - start.latitude) * Math.PI) / 180;
		const deltaLng = ((end.longitude - start.longitude) * Math.PI) / 180;
		const a =
			Math.sin(deltaLat / 2) * Math.sin(deltaLat / 2) +
			Math.cos(latA) * Math.cos(latB) * Math.sin(deltaLng / 2) * Math.sin(deltaLng / 2);

		return 2 * earthRadius * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
	};

	const getViewportChange = (): MapViewportChange | null => {
		if (!map) return null;

		const nextCenter = map.getCenter();
		const bounds = map.getBounds();
		const northEast = bounds.getNorthEast();

		return {
			center: {
				latitude: nextCenter.lat,
				longitude: nextCenter.lng
			},
			zoom: map.getZoom(),
			bounds: {
				north: bounds.getNorth(),
				east: bounds.getEast(),
				south: bounds.getSouth(),
				west: bounds.getWest()
			},
			radius: getDistanceInMeters(
				{ latitude: nextCenter.lat, longitude: nextCenter.lng },
				{ latitude: northEast.lat, longitude: northEast.lng }
			)
		};
	};

	const isHeatmapPointInScaleViewport = (point: MapHeatmapPoint) => {
		if (!map || !isValidHeatmapPoint(point)) return false;

		const canvas = map.getCanvas();
		const width = canvas.clientWidth;
		const height = canvas.clientHeight;

		if (width <= 0 || height <= 0) return false;

		const projectedPoint = map.project([point.longitude, point.latitude]);

		return (
			projectedPoint.x >= -HEATMAP_SCALE_PADDING &&
			projectedPoint.x <= width + HEATMAP_SCALE_PADDING &&
			projectedPoint.y >= -HEATMAP_SCALE_PADDING &&
			projectedPoint.y <= height + HEATMAP_SCALE_PADDING
		);
	};

	const updateVisibleHeatmapScale = () => {
		if (!map || heatmapPoints.length === 0) {
			scale = [0, 100];
			return;
		}

		const visiblePoints = heatmapPoints.filter(isHeatmapPointInScaleViewport);

		if (visiblePoints.length === 0) {
			scale = [0, 100];
			return;
		}

		const values = visiblePoints.map((point: MapHeatmapPoint) => point.value);
		scale = [Math.min(...values), Math.max(...values)];
	};

	const scheduleVisibleHeatmapScaleUpdate = () => {
		if (!map || !styleLoaded) return;
		if (heatmapScaleFrame !== undefined) return;

		heatmapScaleFrame = requestAnimationFrame(() => {
			heatmapScaleFrame = undefined;
			updateVisibleHeatmapScale();
		});
	};

	const emitViewportChange = () => {
		cancelHeatmapScaleSync();
		updateVisibleHeatmapScale();

		const viewport = getViewportChange();
		if (viewport) onmoveend?.(viewport);
	};

	const updateHeatmapPaint = () => {
		if (!map || !map.getLayer(HEATMAP_LAYER_ID)) return;

		const [min, max] = scale;
		if (!Number.isFinite(min) || !Number.isFinite(max)) {
			map.setPaintProperty(HEATMAP_LAYER_ID, "heatmap-weight", 0.8);
			return;
		}

		const lower = Math.min(min, max);
		const upper = Math.max(min, max);

		if (lower === upper) {
			map.setPaintProperty(HEATMAP_LAYER_ID, "heatmap-weight", ["case", ["==", ["get", "value"], lower], 0.72, 0.24]);
			return;
		}

		map.setPaintProperty(HEATMAP_LAYER_ID, "heatmap-weight", [
			"interpolate",
			["linear"],
			["get", "value"],
			lower,
			0.04,
			upper,
			0.82
		]);
	};

	const addDefaultBaseLayer = () => {
		if (!map || style !== DEFAULT_STYLE) return;

		if (!map.getSource(BASE_SOURCE_ID)) {
			map.addSource(BASE_SOURCE_ID, {
				type: "raster",
				tiles: ["https://tile.openstreetmap.org/{z}/{x}/{y}.png"],
				tileSize: 256,
				attribution: "&copy; OpenStreetMap contributors"
			});
		}

		if (map.getLayer(BASE_LAYER_ID)) return;

		map.addLayer(
			{
				id: BASE_LAYER_ID,
				type: "raster",
				source: BASE_SOURCE_ID
			},
			map.getLayer(HEATMAP_LAYER_ID) ? HEATMAP_LAYER_ID : undefined
		);
	};

	const addHeatmapLayer = () => {
		if (!map || map.getSource(HEATMAP_SOURCE_ID)) return;

		map.addSource(HEATMAP_SOURCE_ID, {
			type: "geojson",
			data: createHeatmapFeatureCollection(heatmapPoints)
		});

		map.addLayer({
			id: HEATMAP_LAYER_ID,
			type: "heatmap",
			source: HEATMAP_SOURCE_ID,
			paint: {
				"heatmap-weight": ["interpolate", ["linear"], ["get", "value"], 0, 0.04, 100, 0.82],
				"heatmap-intensity": ["interpolate", ["linear"], ["zoom"], 4, 0.34, 7, 0.58, 10, 1, 13, 1.48, 15, 1.85],
				"heatmap-color": createHeatmapColorExpression(heatmapGradient),
				"heatmap-radius": ["interpolate", ["linear"], ["zoom"], 4, 9, 7, 16, 10, 27, 13, 46, 15, 64],
				"heatmap-opacity": ["interpolate", ["linear"], ["zoom"], 4, 0.64, 8, 0.72, 12, 0.78, 15, 0.84]
			}
		});
	};

	const createCustomMarker = (item: MapMarker<TData>, content: Snippet<[MapMarkerRenderContext<TData>]>) => {
		if (!map) return;

		const element = document.createElement("div");
		element.className = "navigator-map-custom-marker";
		element.setAttribute("role", "button");
		element.setAttribute("tabindex", "0");
		element.setAttribute("aria-label", item.label ? `Select ${item.label}` : "Select map marker");

		element.addEventListener("click", (event: MouseEvent) => {
			event.stopPropagation();
			onmarkerselect?.(item);
		});
		element.addEventListener("keydown", (event: KeyboardEvent) => {
			if (event.key !== "Enter" && event.key !== " ") return;

			event.preventDefault();
			event.stopPropagation();
			onmarkerselect?.(item);
		});

		const component = mount(MapMarkerRenderer<TData>, {
			target: element,
			props: {
				marker: item,
				markerContent: content
			}
		});
		const mapMarker = new maplibregl.Marker({
			element,
			anchor: markerAnchor,
			offset: markerOffset
		})
			.setLngLat([item.longitude, item.latitude])
			.addTo(map);

		customMarkerById.set(String(item.id), {
			marker: mapMarker,
			component,
			source: item,
			content
		});
	};

	const syncCustomMarkers = (
		items: MapMarker<TData>[],
		content: Snippet<[MapMarkerRenderContext<TData>]>,
		renderPadding = getMarkerRenderPadding(),
		minZoom = getMarkerMinZoom()
	) => {
		if (!map) return;

		const visibleItems =
			map.getZoom() >= minZoom
				? items.filter((item: MapMarker<TData>) => isCustomMarkerInRenderViewport(item, renderPadding))
				: [];
		const nextIds = new Set(visibleItems.map((item: MapMarker<TData>) => String(item.id)));

		for (const [id, mountedMarker] of customMarkerById.entries()) {
			if (nextIds.has(id)) continue;

			unmount(mountedMarker.component);
			mountedMarker.marker.remove();
			customMarkerById.delete(id);
		}

		for (const item of visibleItems) {
			const id = String(item.id);
			const mountedMarker = customMarkerById.get(id);

			if (mountedMarker && mountedMarker.source === item && mountedMarker.content === content) {
				mountedMarker.marker.setLngLat([item.longitude, item.latitude]);
				continue;
			}

			if (mountedMarker) {
				unmount(mountedMarker.component);
				mountedMarker.marker.remove();
				customMarkerById.delete(id);
			}

			createCustomMarker(item, content);
		}
	};

	const scheduleCustomMarkerSync = (
		items: MapMarker<TData>[],
		content: Snippet<[MapMarkerRenderContext<TData>]>,
		renderPadding = getMarkerRenderPadding(),
		minZoom = getMarkerMinZoom()
	) => {
		if (!map || !styleLoaded) return;

		pendingCustomMarkerSync = { items, content, renderPadding, minZoom };
		if (customMarkerSyncFrame !== undefined) return;

		customMarkerSyncFrame = requestAnimationFrame(() => {
			customMarkerSyncFrame = undefined;

			const nextSync = pendingCustomMarkerSync;
			pendingCustomMarkerSync = undefined;
			if (!nextSync) return;

			syncCustomMarkers(nextSync.items, nextSync.content, nextSync.renderPadding, nextSync.minZoom);
		});
	};

	const handleCustomMarkerViewportChange = () => {
		if (!markerContent) return;

		scheduleCustomMarkerSync(markers, markerContent, getMarkerRenderPadding(), getMarkerMinZoom());
	};

	const handleHeatmapViewportChange = () => {
		scheduleVisibleHeatmapScaleUpdate();
	};

	onMount(() => {
		if (!mapContainer) return;

		const nextMap = new maplibregl.Map({
			container: mapContainer,
			style,
			center: [center.longitude, center.latitude],
			zoom,
			dragRotate: false,
			pitchWithRotate: false,
			touchPitch: false,
			attributionControl: attributionControl ? undefined : false
		});

		map = nextMap;
		nextMap.dragRotate.disable();
		nextMap.touchPitch.disable();
		const resizeFrame = requestAnimationFrame(() => nextMap.resize());

		if (navigationControl) {
			nextMap.addControl(new maplibregl.NavigationControl({ visualizePitch: true }), "top-right");
		}

		let baseLayerTimeout: ReturnType<typeof setTimeout> | undefined;
		const scheduleBaseLayerRetry = () => {
			if (baseLayerTimeout !== undefined) return;

			baseLayerTimeout = setTimeout(() => {
				baseLayerTimeout = undefined;
				tryAddDefaultBaseLayer();
			}, 100);
		};
		const tryAddDefaultBaseLayer = () => {
			try {
				addDefaultBaseLayer();
				if (mapContainer) delete mapContainer.dataset.mapBaseError;
			} catch (error) {
				if (mapContainer) mapContainer.dataset.mapBaseError = error instanceof Error ? error.message : String(error);
				scheduleBaseLayerRetry();
			}
		};

		const initializeMapLayers = () => {
			if (styleLoaded) return;

			addHeatmapLayer();
			if (markerContent) syncCustomMarkers(markers, markerContent);
			tryAddDefaultBaseLayer();

			nextMap.on("moveend", emitViewportChange);
			nextMap.on("move", handleCustomMarkerViewportChange);
			nextMap.on("move", handleHeatmapViewportChange);
			nextMap.on("zoom", handleCustomMarkerViewportChange);
			nextMap.on("zoom", handleHeatmapViewportChange);
			nextMap.on("resize", handleCustomMarkerViewportChange);
			nextMap.on("resize", handleHeatmapViewportChange);

			styleLoaded = true;
			if (mapContainer) {
				mapContainer.dataset.mapReady = "true";
				delete mapContainer.dataset.mapError;
			}
			emitViewportChange();
		};

		let initializeTimeout: ReturnType<typeof setTimeout> | undefined;
		const scheduleInitializeRetry = () => {
			if (initializeTimeout !== undefined) return;

			initializeTimeout = setTimeout(() => {
				initializeTimeout = undefined;
				tryInitializeMapLayers();
			}, 16);
		};

		const tryInitializeMapLayers = () => {
			if (styleLoaded) return;

			try {
				initializeMapLayers();
			} catch (error) {
				if (mapContainer) mapContainer.dataset.mapError = error instanceof Error ? error.message : String(error);
				scheduleInitializeRetry();
			}
		};

		nextMap.on("styledata", tryInitializeMapLayers);
		nextMap.once("load", tryInitializeMapLayers);
		tryInitializeMapLayers();

		resizeObserver = new ResizeObserver(() => {
			nextMap.resize();
			updateVisibleHeatmapScale();
			handleCustomMarkerViewportChange();
		});
		resizeObserver.observe(mapContainer);

		return () => {
			cancelAnimationFrame(resizeFrame);
			if (initializeTimeout !== undefined) clearTimeout(initializeTimeout);
			if (baseLayerTimeout !== undefined) clearTimeout(baseLayerTimeout);
			cancelCustomMarkerSync();
			cancelHeatmapScaleSync();
			nextMap.off("styledata", tryInitializeMapLayers);
			nextMap.off("load", tryInitializeMapLayers);
			nextMap.off("moveend", emitViewportChange);
			nextMap.off("move", handleCustomMarkerViewportChange);
			nextMap.off("move", handleHeatmapViewportChange);
			nextMap.off("zoom", handleCustomMarkerViewportChange);
			nextMap.off("zoom", handleHeatmapViewportChange);
			nextMap.off("resize", handleCustomMarkerViewportChange);
			nextMap.off("resize", handleHeatmapViewportChange);
			resizeObserver?.disconnect();
			resizeObserver = undefined;
			clearCustomMarkers();
			styleLoaded = false;
			nextMap.remove();
			map = undefined;
		};
	});

	$effect(() => {
		if (!map || !styleLoaded) return;

		const nextMarkers = markers;
		const nextMarkerContent = markerContent;
		const nextMarkerRenderPadding = getMarkerRenderPadding();
		const nextMarkerMinZoom = getMarkerMinZoom();

		if (nextMarkerContent) {
			scheduleCustomMarkerSync(nextMarkers, nextMarkerContent, nextMarkerRenderPadding, nextMarkerMinZoom);
			return;
		}

		cancelCustomMarkerSync();
		clearCustomMarkers();
	});

	$effect(() => {
		if (!map || !styleLoaded) return;

		getGeoJsonSource(HEATMAP_SOURCE_ID)?.setData(createHeatmapFeatureCollection(heatmapPoints));
		scheduleVisibleHeatmapScaleUpdate();
	});

	$effect(() => {
		if (!map || !styleLoaded || !map.getLayer(HEATMAP_LAYER_ID)) return;

		map.setLayoutProperty(HEATMAP_LAYER_ID, "visibility", heatmap ? "visible" : "none");
	});

	$effect(() => {
		if (!map || !styleLoaded) return;

		updateHeatmapPaint();
	});

	$effect(() => {
		if (!map || !styleLoaded || !map.getLayer(HEATMAP_LAYER_ID)) return;

		map.setPaintProperty(HEATMAP_LAYER_ID, "heatmap-color", createHeatmapColorExpression(heatmapGradient));
	});

	$effect(() => {
		if (!map) return;

		const nextCenter = center;
		const nextZoom = zoom;
		const currentCenter = map.getCenter();
		const centerChanged =
			Math.abs(currentCenter.lat - nextCenter.latitude) > 0.000001 ||
			Math.abs(currentCenter.lng - nextCenter.longitude) > 0.000001;
		const zoomChanged = Math.abs(map.getZoom() - nextZoom) > 0.000001;

		if (centerChanged || zoomChanged) {
			map.jumpTo({ center: [nextCenter.longitude, nextCenter.latitude], zoom: nextZoom });
		}
	});
</script>

<div bind:this={mapContainer} role="region" aria-label={ariaLabel} class={["min-h-64 w-full rounded-lg", className]}></div>

<style>
	:global(.navigator-map-custom-marker) {
		display: flex;
		align-items: center;
		justify-content: center;
		cursor: pointer;
		outline: none;
	}

	:global(.navigator-map-custom-marker:focus-visible) {
		border-radius: 9999px;
		box-shadow:
			0 0 0 2px var(--color-background),
			0 0 0 4px var(--color-accent);
	}
</style>
