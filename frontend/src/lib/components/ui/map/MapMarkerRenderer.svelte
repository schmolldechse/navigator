<script lang="ts" generics="TData = unknown">
	import type { Snippet } from "svelte";
	import type { MapMarker, MapMarkerRenderContext } from "./Map.svelte";

	type Props<TMarkerData = unknown> = {
		marker: MapMarker<TMarkerData>;
		markerContent: Snippet<[MapMarkerRenderContext<TMarkerData>]>;
	};
	let { marker, markerContent }: Props<TData> = $props();
</script>

{@render markerContent({ marker })}
