<script lang="ts">
	import type { PageProps } from "./$types";
	import Button from "@lib/components/ui/Button.svelte";
	import StationMetricHeatmapSettingsDialog from "@lib/components/statistics/heatmap/StationMetricHeatmapSettingsDialog.svelte";
	import StationMetricHeatmap from "@lib/components/statistics/heatmap/StationMetricHeatmap.svelte";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import { loadHeatmap } from "@lib/components/statistics/heatmap/heatmap.remote";
	import { type HeatmapSettings } from "@lib/components/statistics/heatmap/heatmap";

	let { data }: PageProps = $props();

	// svelte-ignore state_referenced_locally
	let heatmapPromise: Promise<Awaited<ReturnType<typeof loadHeatmap>>> = $state(data.heatmap.promise);
	// svelte-ignore state_referenced_locally
	let heatmapSettings: HeatmapSettings = $state(data.heatmap.settings);
	let heatmapLoading: boolean = $state(true);

	$effect(() => {
		const currentHeatmapPromise = heatmapPromise;
		if (!currentHeatmapPromise) return;

		heatmapLoading = true;
		currentHeatmapPromise.finally(() => (heatmapLoading = false));
	});

	const createHeatmapRequest = (settings: HeatmapSettings) => ({
		seriesType: settings.seriesType,
		scheduleType: settings.scheduleType,
		start: settings.dates.start.startOf("day").toISO()!,
		end: settings.dates.end.endOf("day").toISO()!,
		transportTypes: settings.transportTypes
	});

	let settingsDialogVisible: boolean = $state(false);

	const saveHeatmapSettings = (nextSettings: HeatmapSettings) => {
		heatmapSettings = nextSettings;
		heatmapPromise = loadHeatmap({
			request: createHeatmapRequest(nextSettings)
		}).run();
	};
</script>

<svelte:head>
	<title>Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-8 p-4 sm:py-8">
	<section class="space-y-4">
		<div class="relative flex items-center justify-between gap-4">
			<h1 class="text-3xl font-bold">Statistics</h1>

			<Button
				mode="secondary"
				onclick={() => (settingsDialogVisible = !settingsDialogVisible)}
				disabled={heatmapLoading}
				aria-label="Open heatmap settings"
				class="inline-flex items-center justify-center gap-x-2 sm:w-auto sm:px-3 sm:py-1.5"
			>
				<SlidersHorizontal size={18} />
				<span class="hidden sm:inline">Settings</span>
			</Button>

			<StationMetricHeatmapSettingsDialog
				bind:isVisible={settingsDialogVisible}
				settings={heatmapSettings}
				onsave={saveHeatmapSettings}
			/>
		</div>

		<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
			<span class="text-foreground font-semibold">Network lens.</span>
			Compare station activity across the visible map, then tune the time range, metric, schedule direction, and transport mix when
			you need a sharper view.
		</p>

		<StationMetricHeatmap promise={heatmapPromise} class="h-[62vh] max-h-[48rem] min-h-[30rem]" />
	</section>
</main>
