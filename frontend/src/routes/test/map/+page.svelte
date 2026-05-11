<script lang="ts">
	import Map, {
		type MapHeatmapGradient,
		type MapHeatmapPoint,
		type MapMarker,
		type MapViewportChange
	} from "@lib/components/ui/map/Map.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import MapPin from "@lucide/svelte/icons/map-pin";

	type SampleMarkerData = {
		category: "hub" | "city" | "airport";
		score: number;
	};

	const markers: MapMarker<SampleMarkerData>[] = [
		{
			id: "ffm-hbf",
			label: "Frankfurt Hbf",
			latitude: 50.107145,
			longitude: 8.663789,
			color: "#ffda0a",
			data: { category: "hub", score: 92 }
		},
		{
			id: "hauptwache",
			label: "Hauptwache",
			latitude: 50.11357,
			longitude: 8.67905,
			color: "#59a14f",
			data: { category: "city", score: 74 }
		},
		{
			id: "konstablerwache",
			label: "Konstablerwache",
			latitude: 50.11455,
			longitude: 8.68774,
			color: "#59a14f",
			data: { category: "city", score: 68 }
		},
		{
			id: "ffm-sued",
			label: "Frankfurt Sud",
			latitude: 50.09927,
			longitude: 8.68609,
			color: "#4e79a7",
			data: { category: "hub", score: 46 }
		},
		{
			id: "airport",
			label: "Frankfurt Flughafen",
			latitude: 50.05218,
			longitude: 8.57082,
			color: "#f28e2b",
			data: { category: "airport", score: 57 }
		},
		{
			id: "mainz-hbf",
			label: "Mainz Hbf",
			latitude: 50.00102,
			longitude: 8.25856,
			color: "#e15759",
			data: { category: "hub", score: 31 }
		},
		{
			id: "darmstadt-hbf",
			label: "Darmstadt Hbf",
			latitude: 49.87284,
			longitude: 8.63272,
			color: "#b07aa1",
			data: { category: "hub", score: 26 }
		}
	];

	const heatmapPoints: MapHeatmapPoint[] = [
		{ id: "ffm-hbf-heat", latitude: 50.107145, longitude: 8.663789, value: 92 },
		{ id: "hauptwache-heat", latitude: 50.11357, longitude: 8.67905, value: 74 },
		{ id: "konstablerwache-heat", latitude: 50.11455, longitude: 8.68774, value: 68 },
		{ id: "ffm-sued-heat", latitude: 50.09927, longitude: 8.68609, value: 46 },
		{ id: "airport-heat", latitude: 50.05218, longitude: 8.57082, value: 57 },
		{ id: "mainz-hbf-heat", latitude: 50.00102, longitude: 8.25856, value: 31 },
		{ id: "darmstadt-hbf-heat", latitude: 49.87284, longitude: 8.63272, value: 26 }
	];

	const heatmapGradientColors = ["#355c7d", "#6c5b7b", "#c06c84", "#f67280", "#f8b195"];
	const heatmapGradient: MapHeatmapGradient = heatmapGradientColors;
	const heatmapGradientPreview = `linear-gradient(to right, ${heatmapGradientColors.join(", ")})`;

	let selectedMarker = $state<MapMarker<SampleMarkerData> | null>(null);
	let viewport = $state<MapViewportChange | null>(null);
	let scale = $state<[number, number]>([0, 100]);

	const formatNumber = (value: number, maximumFractionDigits = 2) => value.toLocaleString(undefined, { maximumFractionDigits });
</script>

<svelte:head>
	<title>Map Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">Map Test</h1>
		<p class="text-foreground/60 text-sm">MapLibre GL sample with static station points.</p>
	</div>

	<section class="grid grid-cols-1 gap-4 lg:grid-cols-3">
		<div class="lg:col-span-3">
			<Map
				center={{ latitude: 50.1066819, longitude: 8.66282825 }}
				zoom={10}
				{markers}
				heatmap={true}
				{heatmapPoints}
				{heatmapGradient}
				bind:scale
				onmarkerselect={(marker: MapMarker<SampleMarkerData>) => (selectedMarker = marker)}
				onmoveend={(change: MapViewportChange) => (viewport = change)}
				class="border-border h-[70vh] max-h-[720px] min-h-[420px] w-full border"
			>
				{#snippet marker({ marker })}
					<div
						class={[
							"border-border bg-background/85 flex h-9 w-9 items-center justify-center rounded-full border shadow-sm backdrop-blur-md transition-transform hover:scale-110",
							selectedMarker?.id === marker.id && "ring-accent ring-2"
						]}
						title={marker.label}
					>
						<MapPin size={18} color={marker.color ?? "#ffda0a"} />
					</div>
				{/snippet}
			</Map>
		</div>

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
