<script module lang="ts">
	import type { BaseStation } from "@lib/api";

	type StationHeatmapPoint = {
		station: BaseStation;
		value: number;
	};

	export { type StationHeatmapPoint };
</script>

<script lang="ts">
	import type { ClassValue } from "svelte/elements";
	import Map, { type MapHeatmapGradient, type MapHeatmapPoint } from "@lib/components/ui/map/Map.svelte";

	type Props = {
		stationHeatmapPoints: StationHeatmapPoint[];
		scale: [number, number];
		heatmapGradient?: MapHeatmapGradient;
		class?: ClassValue;
	};

	let { stationHeatmapPoints, scale = $bindable([0, 100]), heatmapGradient, class: classes = "" }: Props = $props();

	let heatmapPoints: MapHeatmapPoint[] = $derived(
		stationHeatmapPoints.map((point: StationHeatmapPoint) => ({
			id: point.station.evaNumber,
			latitude: Number(point.station.position.latitude),
			longitude: Number(point.station.position.longitude),
			value: point.value
		}))
	);
</script>

<Map
	heatmap={true}
	{heatmapPoints}
	{heatmapGradient}
	bind:scale
	center={{ latitude: 50.1066819, longitude: 8.66282825 }}
	zoom={6}
	class={classes}
/>
