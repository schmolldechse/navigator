<script lang="ts">
	import Map, {
		type MapHeatmapGradientInterpolator,
		type MapHeatmapPoint,
		type MapMarker,
		type MapMarkerRenderContext,
		type MapViewportChange
	} from "@lib/components/ui/map/Map.svelte";
	import Button from "@lib/components/ui/Button.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import Input from "@lib/components/ui/Input.svelte";
	import {
		interpolateCividis,
		interpolateCool,
		interpolateInferno,
		interpolateMagma,
		interpolatePlasma,
		interpolateRainbow,
		interpolateTurbo,
		interpolateViridis,
		interpolateWarm
	} from "d3-scale-chromatic";
	import MapPin from "@lucide/svelte/icons/map-pin";
	import Plus from "@lucide/svelte/icons/plus";
	import RotateCcw from "@lucide/svelte/icons/rotate-ccw";

	const markerCategories = ["hub", "city", "airport"] as const;
	type SampleMarkerCategory = (typeof markerCategories)[number];

	type SampleMarkerData = {
		category: SampleMarkerCategory;
		color: string;
		score: number;
	};

	const defaultCenter = { latitude: 50.1066819, longitude: 8.66282825 };
	const categoryColors: Record<SampleMarkerCategory, string> = {
		hub: "#4e79a7",
		city: "#59a14f",
		airport: "#f28e2b"
	};
	const gradientSampleDensities = [0, 0.2, 0.4, 0.6, 0.8, 1] as const;
	const heatmapGradientOptions = [
		{ id: "turbo", label: "Turbo", interpolate: interpolateTurbo },
		{ id: "viridis", label: "Viridis", interpolate: interpolateViridis },
		{ id: "plasma", label: "Plasma", interpolate: interpolatePlasma },
		{ id: "inferno", label: "Inferno", interpolate: interpolateInferno },
		{ id: "magma", label: "Magma", interpolate: interpolateMagma },
		{ id: "cividis", label: "Cividis", interpolate: interpolateCividis },
		{ id: "warm", label: "Warm", interpolate: interpolateWarm },
		{ id: "cool", label: "Cool", interpolate: interpolateCool },
		{ id: "rainbow", label: "Rainbow", interpolate: interpolateRainbow }
	] as const;
	type HeatmapGradientId = (typeof heatmapGradientOptions)[number]["id"];

	const createInitialMarkers = (): MapMarker<SampleMarkerData>[] => [
		{
			id: "ffm-hbf",
			label: "Frankfurt Hbf",
			latitude: 50.107145,
			longitude: 8.663789,
			data: { category: "hub", color: "#ffda0a", score: 92 }
		},
		{
			id: "hauptwache",
			label: "Hauptwache",
			latitude: 50.11357,
			longitude: 8.67905,
			data: { category: "city", color: categoryColors.city, score: 74 }
		},
		{
			id: "konstablerwache",
			label: "Konstablerwache",
			latitude: 50.11455,
			longitude: 8.68774,
			data: { category: "city", color: categoryColors.city, score: 68 }
		},
		{
			id: "ffm-sued",
			label: "Frankfurt Sud",
			latitude: 50.09927,
			longitude: 8.68609,
			data: { category: "hub", color: categoryColors.hub, score: 46 }
		},
		{
			id: "airport",
			label: "Frankfurt Flughafen",
			latitude: 50.05218,
			longitude: 8.57082,
			data: { category: "airport", color: categoryColors.airport, score: 57 }
		},
		{
			id: "mainz-hbf",
			label: "Mainz Hbf",
			latitude: 50.00102,
			longitude: 8.25856,
			data: { category: "hub", color: "#e15759", score: 31 }
		},
		{
			id: "darmstadt-hbf",
			label: "Darmstadt Hbf",
			latitude: 49.87284,
			longitude: 8.63272,
			data: { category: "hub", color: "#b07aa1", score: 26 }
		}
	];

	const getHeatmapGradientInterpolator = (gradientId: HeatmapGradientId) =>
		(heatmapGradientOptions.find((option) => option.id === gradientId) ?? heatmapGradientOptions[0]).interpolate;

	let markers = $state<MapMarker<SampleMarkerData>[]>(createInitialMarkers());
	let selectedMarker = $state<MapMarker<SampleMarkerData> | null>(null);
	let viewport = $state<MapViewportChange | null>(null);
	let scale = $state<[number, number]>([0, 100]);
	let showMarkers = $state(true);
	let heatmapEnabled = $state(true);
	let selectedHeatmapGradientId = $state<HeatmapGradientId>("turbo");
	let markerRenderPadding = $state(128);
	let markerSize = $state(36);
	let markerSpreadKm = $state(25);
	let nextGeneratedMarker = $state(1);

	let heatmapPoints: MapHeatmapPoint[] = $derived(
		markers.map((marker: MapMarker<SampleMarkerData>) => ({
			id: `${marker.id}-heat`,
			latitude: marker.latitude,
			longitude: marker.longitude,
			value: marker.data?.score ?? 0
		}))
	);
	let markerIconSize = $derived(Math.max(14, Math.min(28, markerSize - 14)));
	let heatmapGradient: MapHeatmapGradientInterpolator = $derived(getHeatmapGradientInterpolator(selectedHeatmapGradientId));
	let heatmapGradientColors = $derived(gradientSampleDensities.map((density) => heatmapGradient(density)));
	let heatmapGradientPreview = $derived(`linear-gradient(to right, ${heatmapGradientColors.join(", ")})`);

	const formatNumber = (value: number, maximumFractionDigits = 2) => value.toLocaleString(undefined, { maximumFractionDigits });

	const clampSetting = (value: string | number, min: number, max: number, fallback: number) => {
		const parsedValue = typeof value === "number" ? value : Number(value);
		if (!Number.isFinite(parsedValue)) return fallback;

		return Math.min(max, Math.max(min, parsedValue));
	};

	const handleHeatmapGradientChange = (event: Event) => {
		selectedHeatmapGradientId = (event.currentTarget as HTMLSelectElement).value as HeatmapGradientId;
	};

	const addMarker = () => {
		const markerNumber = nextGeneratedMarker;
		const category = markerCategories[markerNumber % markerCategories.length];
		const center = viewport?.center ?? defaultCenter;
		const distanceKm = Math.sqrt(Math.random()) * markerSpreadKm;
		const angle = Math.random() * Math.PI * 2;
		const latitude = Math.min(90, Math.max(-90, center.latitude + (Math.cos(angle) * distanceKm) / 111));
		const longitudeScale = Math.max(0.1, Math.cos((center.latitude * Math.PI) / 180));
		const longitude = Math.min(180, Math.max(-180, center.longitude + (Math.sin(angle) * distanceKm) / (111 * longitudeScale)));
		const score = Math.round(10 + Math.random() * 90);
		const marker: MapMarker<SampleMarkerData> = {
			id: `generated-${markerNumber}`,
			label: `Generated marker ${markerNumber}`,
			latitude,
			longitude,
			data: { category, color: categoryColors[category], score }
		};

		nextGeneratedMarker += 1;
		markers = [...markers, marker];
		selectedMarker = marker;
	};

	const resetMarkers = () => {
		markers = createInitialMarkers();
		selectedMarker = null;
		nextGeneratedMarker = 1;
	};
</script>

<svelte:head>
	<title>Map Test - Navigator</title>
</svelte:head>

{#snippet customMarker(context: MapMarkerRenderContext<SampleMarkerData>)}
	<div
		class={[
			"border-border bg-background/85 flex items-center justify-center rounded-full border shadow-sm backdrop-blur-md transition-transform hover:scale-110",
			selectedMarker?.id === context.marker.id && "ring-accent ring-2"
		]}
		style={`height: ${markerSize}px; width: ${markerSize}px;`}
		title={context.marker.label}
	>
		<MapPin size={markerIconSize} color={context.marker.data?.color ?? "#ffda0a"} />
	</div>
{/snippet}

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">Map Test</h1>
		<p class="text-foreground/60 text-sm">MapLibre GL sample with configurable station points.</p>
	</div>

	<section class="grid grid-cols-1 gap-4 xl:grid-cols-[minmax(0,1fr)_22rem]">
		<Map
			center={defaultCenter}
			zoom={10}
			{markers}
			marker={showMarkers ? customMarker : undefined}
			{markerRenderPadding}
			heatmap={heatmapEnabled}
			{heatmapPoints}
			{heatmapGradient}
			bind:scale
			onmarkerselect={(marker: MapMarker<SampleMarkerData>) => (selectedMarker = marker)}
			onmoveend={(change: MapViewportChange) => (viewport = change)}
			class="border-border h-[70vh] max-h-[720px] min-h-[420px] w-full border"
		/>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Settings</h2>
				<p class="text-foreground/60 text-sm">{markers.length.toLocaleString()} markers</p>
			</div>

			<div class="flex flex-wrap gap-2">
				<Button onclick={addMarker} class="inline-flex items-center gap-2">
					<Plus size={16} />
					Add marker
				</Button>
				<Button mode="secondary" onclick={resetMarkers} class="inline-flex items-center gap-2">
					<RotateCcw size={16} />
					Reset
				</Button>
			</div>

			<div class="grid gap-3">
				<label class="flex items-center gap-2 text-sm font-medium">
					<Checkbox bind:checked={showMarkers} />
					Show markers
				</label>

				<label class="flex items-center gap-2 text-sm font-medium">
					<Checkbox bind:checked={heatmapEnabled} />
					Heatmap
				</label>
			</div>

			<div class="grid gap-3">
				<label class="grid gap-1 text-sm font-medium">
					Heatmap gradient
					<select
						value={selectedHeatmapGradientId}
						onchange={handleHeatmapGradientChange}
						class="border-border bg-background text-foreground rounded-lg border-2 px-3 py-1.5 text-sm font-semibold transition-all outline-none"
					>
						{#each heatmapGradientOptions as option}
							<option value={option.id}>{option.label}</option>
						{/each}
					</select>
				</label>

				<label class="grid gap-1 text-sm font-medium">
					Render padding
					<Input
						type="number"
						value={markerRenderPadding}
						min={0}
						max={2048}
						step={16}
						onchange={(value: string | number) => (markerRenderPadding = clampSetting(value, 0, 2048, 128))}
					/>
				</label>

				<label class="grid gap-1 text-sm font-medium">
					Marker size
					<Input
						type="number"
						value={markerSize}
						min={20}
						max={96}
						step={2}
						onchange={(value: string | number) => (markerSize = clampSetting(value, 20, 96, 36))}
					/>
				</label>

				<label class="grid gap-1 text-sm font-medium">
					Add radius
					<Input
						type="number"
						value={markerSpreadKm}
						min={1}
						max={500}
						step={1}
						onchange={(value: string | number) => (markerSpreadKm = clampSetting(value, 1, 500, 25))}
					/>
				</label>
			</div>
		</Card>
	</section>

	<section class="grid grid-cols-1 gap-4 lg:grid-cols-3">
		<Card class="gap-y-3">
			<div>
				<h2 class="text-lg font-semibold">Selected Marker</h2>
				<p class="text-foreground/60 text-sm">{selectedMarker?.label ?? "None"}</p>
			</div>

			{#if selectedMarker}
				<div class="text-foreground/70 grid grid-cols-2 gap-3 text-sm">
					<span>Latitude</span>
					<span class="text-foreground text-right tabular-nums">{formatNumber(selectedMarker.latitude, 5)}</span>
					<span>Longitude</span>
					<span class="text-foreground text-right tabular-nums">{formatNumber(selectedMarker.longitude, 5)}</span>
					<span>Category</span>
					<span class="text-foreground text-right capitalize">{selectedMarker.data?.category}</span>
					<span>Score</span>
					<span class="text-foreground text-right tabular-nums">{selectedMarker.data?.score}</span>
				</div>
			{/if}
		</Card>

		<Card class="gap-y-3">
			<div>
				<h2 class="text-lg font-semibold">Viewport</h2>
				<p class="text-foreground/60 text-sm">Zoom {viewport ? formatNumber(viewport.zoom, 1) : "pending"}</p>
			</div>

			{#if viewport}
				<div class="text-foreground/70 grid grid-cols-2 gap-3 text-sm">
					<span>Center lat</span>
					<span class="text-foreground text-right tabular-nums">{formatNumber(viewport.center.latitude, 5)}</span>
					<span>Center lng</span>
					<span class="text-foreground text-right tabular-nums">{formatNumber(viewport.center.longitude, 5)}</span>
					<span>Radius</span>
					<span class="text-foreground text-right tabular-nums">{formatNumber(viewport.radius / 1000, 1)} km</span>
				</div>
			{/if}
		</Card>

		<Card class="gap-y-3">
			<div>
				<h2 class="text-lg font-semibold">Heatmap Scale</h2>
				<p class="text-foreground/60 text-sm">{formatNumber(scale[0], 0)} to {formatNumber(scale[1], 0)}</p>
			</div>

			<div class="bg-secondary h-3 overflow-hidden rounded-full">
				<div class="h-full w-full" style={`background: ${heatmapGradientPreview}`}></div>
			</div>
		</Card>
	</section>
</main>
