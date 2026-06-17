<script lang="ts">
	import { goto } from "$app/navigation";
	import type { GeoJsonFeature, GeoJsonFeatureCollection } from "@lib/api";
	import Map, { type MapHeatmapPoint, type MapMarker, type MapValuePoint } from "@lib/components/ui/map/Map.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import MapPinned from "@lucide/svelte/icons/map-pinned";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import { formatCount, toNumber } from "../shared/statistics-dashboard";

	type HotspotData = {
		stationEvaNumber: number;
		stationName: string;
		plannedEvents: number | null;
		customerReliability5Rate: number | null;
		cancellationRate: number | null;
		delayDebtMinutes: number | null;
	};

	type Props = {
		promise: RemoteQuery<GeoJsonFeatureCollection>;
	};

	let { promise }: Props = $props();
	let scale: [number, number] = $state([0, 1]);

	const getProperty = (feature: GeoJsonFeature, key: string): unknown => feature.properties?.[key];

	const createHotspotData = (feature: GeoJsonFeature): HotspotData | null => {
		const stationEvaNumber = toNumber(getProperty(feature, "stationEvaNumber") as number | string | null | undefined);
		if (stationEvaNumber === null) return null;

		return {
			stationEvaNumber,
			stationName: String(getProperty(feature, "stationName") ?? "Unnamed station"),
			plannedEvents: toNumber(getProperty(feature, "plannedEvents") as number | string | null | undefined),
			customerReliability5Rate: toNumber(
				getProperty(feature, "customerReliability5Rate") as number | string | null | undefined
			),
			cancellationRate: toNumber(getProperty(feature, "cancellationRate") as number | string | null | undefined),
			delayDebtMinutes: toNumber(getProperty(feature, "delayDebtMinutes") as number | string | null | undefined)
		};
	};

	const qualityColor = (score: number | null): string => {
		if (score === null) return "#94a3b8";
		if (score >= 0.9) return "#16a34a";
		if (score >= 0.75) return "#ca8a04";
		return "#dc2626";
	};

	const createMarkers = (features: GeoJsonFeature[]): MapMarker<HotspotData>[] =>
		features
			.map((feature) => {
				const coordinates = feature.geometry.coordinates;
				const longitude = toNumber(coordinates[0]);
				const latitude = toNumber(coordinates[1]);
				const data = createHotspotData(feature);
				if (longitude === null || latitude === null || !data) return null;

				const marker: MapMarker<HotspotData> = {
					id: data.stationEvaNumber,
					label: data.stationName,
					longitude,
					latitude,
					data
				};

				return marker;
			})
			.filter((marker): marker is MapMarker<HotspotData> => marker !== null);

	const createHeatmapPoints = (markers: MapMarker<HotspotData>[]): MapHeatmapPoint[] =>
		markers.map((marker) => ({
			id: marker.id,
			longitude: marker.longitude,
			latitude: marker.latitude,
			value: Math.max(1, marker.data?.delayDebtMinutes ?? 1)
		}));

	const createValuePoints = (markers: MapMarker<HotspotData>[]): MapValuePoint[] =>
		markers.map((marker) => ({
			id: marker.id,
			longitude: marker.longitude,
			latitude: marker.latitude,
			value: marker.data?.customerReliability5Rate ?? 0,
			color: qualityColor(marker.data?.customerReliability5Rate ?? null)
		}));

	const openStation = async (marker: MapMarker<HotspotData>) => {
		await goto(`/statistics/${marker.id}`);
	};
</script>

<DashboardPanel
	title="Station hotspots"
	description="Delay pressure and reliability by station."
	icon={MapPinned}
	class="overflow-hidden"
>
	{#if promise.loading}
		<div class="flex min-h-[32rem] flex-col gap-y-3">
			<Skeleton class="h-[30rem] w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-[32rem] flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const markers = createMarkers(promise.current.features)}
		{@const heatmapPoints = createHeatmapPoints(markers)}
		{@const valuePoints = createValuePoints(markers)}
		{#if markers.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-[32rem] items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No station hotspots available.</p>
			</div>
		{:else}
			<Map
				bind:scale
				{markers}
				{heatmapPoints}
				{valuePoints}
				valuePointLayer
				heatmap
				center={{ latitude: 51.1657, longitude: 10.4515 }}
				zoom={5.4}
				markerMinZoom={8.5}
				markerRenderPadding={48}
				onmarkerselect={openStation}
				class="min-h-[32rem] overflow-hidden rounded-lg"
				ariaLabel="Station quality hotspot map"
			>
				{#snippet marker({ marker })}
					<div
						class="border-background bg-accent text-accent-foreground flex h-7 min-w-7 items-center justify-center rounded-full border-2 px-2 text-xs font-bold shadow-lg"
						title={marker.data?.stationName}
					>
						{formatCount(marker.data?.plannedEvents, true)}
					</div>
				{/snippet}
			</Map>
		{/if}
	{/if}
</DashboardPanel>
