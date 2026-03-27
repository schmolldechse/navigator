<script lang="ts">
	import MapPin from "@lucide/svelte/icons/map-pin";
	import type { StationHeatmapPoint } from "../StationHeatmap.svelte";
	import * as Tooltip from "@lib/components/interactable/tooltip";
	import Separator from "@lib/components/interactable/Separator.svelte";

	type Props = {
		point: StationHeatmapPoint;
	};
	let { point }: Props = $props();
</script>

<Tooltip.Root delay={100}>
	<Tooltip.Trigger>
		<div
			class="bg-muted/70 border-muted-foreground/20 hover:bg-muted-foreground/20 flex h-8 w-8 cursor-pointer items-center justify-center rounded-full border shadow-sm backdrop-blur-md transition-all hover:scale-110"
		>
			<MapPin size={16} class="text-accent" />
		</div>
	</Tooltip.Trigger>

	<Tooltip.Content position="top">
		<div class="flex flex-col gap-1.5">
			<span class="text-text text-sm font-semibold">{point.station.name}</span>

			<Separator />

			<span class="text-accent text-right text-sm font-bold tabular-nums">{point.value.toLocaleString()}</span>
		</div>
	</Tooltip.Content>
</Tooltip.Root>
