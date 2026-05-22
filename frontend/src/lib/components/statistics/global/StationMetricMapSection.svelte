<script lang="ts">
	import Button from "@lib/components/ui/Button.svelte";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import StationMetricMapSettingsDialog from "./station-metric-map/StationMetricMapSettingsDialog.svelte";
	import StationMetricMap from "./station-metric-map/StationMetricMap.svelte";
	import { loadStationMetricMap } from "./station-metric-map/station-metric-map.remote";
	import { createStationMetricMapRequest, type StationMetricMapSettings } from "./station-metric-map/station-metric-map";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadStationMetricMap>>>;
		settings: StationMetricMapSettings;
	};
	let { promise, settings }: Props = $props();

	let metricLoading: boolean = $state(true);
	let settingsDialogVisible: boolean = $state(false);
	let loadingVersion = 0;

	$effect(() => {
		const currentPromise = promise;
		const version = ++loadingVersion;
		if (!currentPromise) return;

		metricLoading = true;
		currentPromise.finally(() => {
			if (version === loadingVersion) metricLoading = false;
		});
	});

	const saveSettings = (nextSettings: StationMetricMapSettings) => {
		settings = nextSettings;
		promise = loadStationMetricMap({
			request: createStationMetricMapRequest(nextSettings)
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
			aria-label="Open station metric map settings"
			class="inline-flex items-center justify-center gap-x-2 sm:w-auto sm:px-3 sm:py-1.5"
		>
			<SlidersHorizontal size={18} />
			<span class="hidden sm:inline">Settings</span>
		</Button>

		<StationMetricMapSettingsDialog
			bind:isVisible={settingsDialogVisible}
			{settings}
			title="Map Settings"
			onsave={saveSettings}
		/>
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		Compare station activity as density and station quality as value-colored points, then tune the time range, schedule
		direction, and transport mix when you need a sharper view.
	</p>

	<StationMetricMap {promise} class="h-[62vh] max-h-[48rem] min-h-[30rem]" />
</section>
