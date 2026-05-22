<script lang="ts">
	import { loadMetric } from "@lib/remote/metrics.remote";
	import { type StationMetricMapSettings } from "./station-metric-map/station-metric-map";
	import { createNetworkTimeSeriesRequest, getNetworkTimeSeriesOption } from "./network-time-series/network-time-series";
	import Activity from "@lucide/svelte/icons/activity";
	import Button from "@lib/components/ui/Button.svelte";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import StationMetricMapSettingsDialog from "./station-metric-map/StationMetricMapSettingsDialog.svelte";
	import NetworkQualityTimeSeries from "./network-time-series/NetworkQualityTimeSeries.svelte";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		settings: StationMetricMapSettings;
	};
	let { promise, settings }: Props = $props();

	let metricLoading: boolean = $state(true);
	let settingsDialogVisible: boolean = $state(false);
	let loadingVersion = 0;
	let metricOption = $derived(getNetworkTimeSeriesOption(settings.seriesType));

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
		promise = loadMetric({
			request: createNetworkTimeSeriesRequest(nextSettings)
		}).run();
	};
</script>

<section class="space-y-4">
	<div class="relative flex items-center justify-between gap-4">
		<div class="flex items-center gap-2">
			<Activity size={22} class="text-accent" />
			<h2 class="text-2xl font-semibold">Network Trend</h2>
		</div>

		<Button
			mode="secondary"
			onclick={() => (settingsDialogVisible = !settingsDialogVisible)}
			disabled={metricLoading}
			aria-label="Open network trend settings"
			class="inline-flex items-center justify-center gap-x-2 sm:w-auto sm:px-3 sm:py-1.5"
		>
			<SlidersHorizontal size={18} />
			<span class="hidden sm:inline">Settings</span>
		</Button>

		<StationMetricMapSettingsDialog
			bind:isVisible={settingsDialogVisible}
			{settings}
			title="Time Series Settings"
			onsave={saveSettings}
		/>
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		Track the selected station-event metric over time across the network and compare transport-type movement.
		{metricOption?.description ?? ""}
	</p>

	<NetworkQualityTimeSeries {promise} />
</section>
