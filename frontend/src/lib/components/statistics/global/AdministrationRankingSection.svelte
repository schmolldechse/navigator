<script lang="ts">
	import { loadMetric } from "@lib/remote/metrics.remote";
	import {
		createAdministrationRankingRequest,
		type AdministrationRankingSettings
	} from "./administration-ranking/administration-ranking";
	import ChartBar from "@lucide/svelte/icons/chart-bar";
	import Button from "@lib/components/ui/Button.svelte";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import AdministrationRankingSettingsDialog from "./administration-ranking/AdministrationRankingSettingsDialog.svelte";
	import AdministrationRanking from "./administration-ranking/AdministrationRanking.svelte";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		settings: AdministrationRankingSettings;
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

	const saveSettings = (nextSettings: AdministrationRankingSettings) => {
		settings = { ...nextSettings, limit: 10, offset: 0 };
		promise = loadMetric({
			request: createAdministrationRankingRequest(settings)
		}).run();
	};

	const changePage = (offset: number) => {
		settings = { ...settings, limit: 10, offset };
		promise = loadMetric({
			request: createAdministrationRankingRequest(settings)
		}).run();
	};
</script>

<section class="space-y-4">
	<div class="relative flex items-center justify-between gap-4">
		<div class="flex items-center gap-2">
			<ChartBar size={22} class="text-accent" />
			<h2 class="text-2xl font-semibold">Administration Ranking</h2>
		</div>

		<Button
			mode="secondary"
			onclick={() => (settingsDialogVisible = !settingsDialogVisible)}
			disabled={metricLoading}
			aria-label="Open administration ranking settings"
			class="inline-flex items-center justify-center gap-x-2 sm:w-auto sm:px-3 sm:py-1.5"
		>
			<SlidersHorizontal size={18} />
			<span class="hidden sm:inline">Settings</span>
		</Button>

		<AdministrationRankingSettingsDialog bind:isVisible={settingsDialogVisible} {settings} onsave={saveSettings} />
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		Compare operators across the global journey dataset for a dedicated time range. Values are sorted descending by the selected
		metric.
	</p>

	<AdministrationRanking {promise} onpagechange={changePage} />
</section>
