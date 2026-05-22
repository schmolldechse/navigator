<script lang="ts">
	import type { PageProps } from "./$types";
	import HeatmapMetricSection from "@lib/components/statistics/global/HeatmapMetricSection.svelte";
	import AdministrationRankingSection from "@lib/components/statistics/global/AdministrationRankingSection.svelte";

	let { data }: PageProps = $props();
</script>

<svelte:head>
	<title>Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-8 p-4 sm:py-8">
	<HeatmapMetricSection promise={data.heatmap.promise} settings={data.heatmap.settings} />

	<AdministrationRankingSection promise={data.administrationRanking.promise} settings={data.administrationRanking.settings} />
</main>
