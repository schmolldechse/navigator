<script lang="ts">
	import { loadMetric } from "@lib/remote/metrics.remote";
	import { createLineRankingRequest, getLineRankingMetricOption, type LineRankingSettings } from "./line-ranking/line-ranking";
	import ChartBar from "@lucide/svelte/icons/chart-bar";
	import Button from "@lib/components/ui/Button.svelte";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import LineRankingSettingsDialog from "./line-ranking/LineRankingSettingsDialog.svelte";
	import LineRanking from "./line-ranking/LineRanking.svelte";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		settings: LineRankingSettings;
	};
	let { promise, settings }: Props = $props();

	let metricLoading: boolean = $state(true);
	let settingsDialogVisible: boolean = $state(false);
	let loadingVersion = 0;
	let metricOption = $derived(getLineRankingMetricOption(settings.seriesType));

	$effect(() => {
		const currentPromise = promise;
		const version = ++loadingVersion;
		if (!currentPromise) return;

		metricLoading = true;
		currentPromise.finally(() => {
			if (version === loadingVersion) metricLoading = false;
		});
	});

	const saveSettings = (nextSettings: LineRankingSettings) => {
		settings = { ...nextSettings, limit: 10, offset: 0 };
		promise = loadMetric({
			request: createLineRankingRequest(settings)
		}).run();
	};

	const changePage = (offset: number) => {
		settings = { ...settings, limit: 10, offset };
		promise = loadMetric({
			request: createLineRankingRequest(settings)
		}).run();
	};
</script>

<section class="space-y-4">
	<div class="relative flex items-center justify-between gap-4">
		<div class="flex items-center gap-2">
			<ChartBar size={22} class="text-accent" />
			<h2 class="text-2xl font-semibold">Line Ranking</h2>
		</div>

		<Button
			mode="secondary"
			onclick={() => (settingsDialogVisible = !settingsDialogVisible)}
			disabled={metricLoading}
			aria-label="Open line ranking settings"
			class="inline-flex items-center justify-center gap-x-2 sm:w-auto sm:px-3 sm:py-1.5"
		>
			<SlidersHorizontal size={18} />
			<span class="hidden sm:inline">Settings</span>
		</Button>

		<LineRankingSettingsDialog bind:isVisible={settingsDialogVisible} {settings} onsave={saveSettings} />
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		Compare individual lines and routes across the global journey dataset.
		{metricOption?.rankingDescription ?? "Values are ranked by the selected metric."}
	</p>

	<LineRanking {promise} onpagechange={changePage} />
</section>
