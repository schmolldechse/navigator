<script lang="ts">
	import type { BaseStation } from "@lib/api/types.gen";
	import type { ClassValue } from "svelte/elements";
	import Map, { type MapMarker, type MapViewportChange } from "../ui/map/Map.svelte";
	import MapPin from "@lucide/svelte/icons/map-pin";

	type Props = {
		stations?: BaseStation[];
		onresize?: ({
			latitude,
			longitude,
			radius
		}: {
			latitude: number;
			longitude: number;
			radius: number;
		}) => void | Promise<void>;
		onselect?: (station: BaseStation) => void;
		class?: ClassValue;
	};

	let { stations = [], onresize, onselect, class: className }: Props = $props();

	let markers: MapMarker<BaseStation>[] = $derived(
		stations.map((station: BaseStation) => ({
			id: station.evaNumber,
			label: station.name,
			latitude: Number(station.position.latitude),
			longitude: Number(station.position.longitude),
			color: "#ffda0a",
			data: station
		}))
	);

	const handleMoveEnd = (viewport: MapViewportChange) => {
		onresize?.({
			latitude: viewport.center.latitude,
			longitude: viewport.center.longitude,
			radius: viewport.radius
		});
	};

	const handleMarkerSelect = (marker: MapMarker<BaseStation>) => {
		if (marker.data) onselect?.(marker.data);
	};
</script>

<Map
	center={{ latitude: 50.1066819, longitude: 8.66282825 }}
	zoom={14}
	{markers}
	onmoveend={handleMoveEnd}
	onmarkerselect={handleMarkerSelect}
	ariaLabel="Station map"
	class={["h-full w-full rounded-xl", className]}
>
	{#snippet marker({ marker })}
		<div
			class="border-border bg-background/85 flex h-9 w-9 items-center justify-center rounded-full border shadow-sm backdrop-blur-md"
		>
			<MapPin size={18} color={marker.color ?? "#ffda0a"} />
		</div>
	{/snippet}
</Map>
