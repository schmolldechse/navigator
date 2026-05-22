<script lang="ts">
	import Button from "@lib/components/ui/Button.svelte";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import StationMetricHeatmapSettingsDialog from "./heatmap/StationMetricHeatmapSettingsDialog.svelte";
	import StationMetricHeatmap from "./heatmap/StationMetricHeatmap.svelte";
	import { loadHeatmap } from "./heatmap/heatmap.remote";
	import { createHeatmapRequest, type HeatmapSettings } from "./heatmap/heatmap";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadHeatmap>>>;
		settings: HeatmapSettings;
	};
	let { promise, settings }: Props = $props();

	let metricLoading: boolean = $state(true);
	let settingsDialogVisible: boolean = $state(false);

	$effect(() => {
		const currentHeatmapPromise = promise;
		if (!currentHeatmapPromise) return;

		metricLoading = true;
		currentHeatmapPromise.finally(() => (metricLoading = false));
	});

	const saveSettings = (nextSettings: HeatmapSettings) => {
		settings = nextSettings;
		promise = loadHeatmap({
			request: createHeatmapRequest(nextSettings)
		}).run();
	};
</script>

<section class="space-y-4">
	<div class="relative flex items-center justify-between gap-4">
		<h1 class="text-3xl font-bold">Statistics</h1>

		<Button
			mode="secondary"
			onclick={() => (settingsDialogVisible = !settingsDialogVisible)}
			disabled={metricLoading}
			aria-label="Open heatmap settings"
			class="inline-flex items-center justify-center gap-x-2 sm:w-auto sm:px-3 sm:py-1.5"
		>
			<SlidersHorizontal size={18} />
			<span class="hidden sm:inline">Settings</span>
		</Button>

		<StationMetricHeatmapSettingsDialog bind:isVisible={settingsDialogVisible} {settings} onsave={saveSettings} />
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		Compare station activity across the visible map, then tune the time range, metric, schedule direction, and transport mix
		when you need a sharper view.
	</p>

	<StationMetricHeatmap {promise} class="h-[62vh] max-h-[48rem] min-h-[30rem]" />
</section>
