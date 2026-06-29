<script lang="ts">
	import type { PageProps } from "./$types";
	import AdministrationRanking from "@lib/components/statistics/global/administration-ranking/AdministrationRanking.svelte";
	import LineRanking from "@lib/components/statistics/global/line-ranking/LineRanking.svelte";
	import NetworkKpiStrip from "@lib/components/statistics/global/network-kpis/NetworkKpiStrip.svelte";
	import NetworkQualityTimeSeries from "@lib/components/statistics/global/network-time-series/NetworkQualityTimeSeries.svelte";
	import NetworkTransportBreakdown from "@lib/components/statistics/global/transport-breakdown/NetworkTransportBreakdown.svelte";
	import StatisticsScopeControls from "@lib/components/statistics/global/StatisticsScopeControls.svelte";
	import {
		setStatisticsScopeContext,
		StatisticsScopeContext
	} from "@lib/components/statistics/global/statistics-scope-context.svelte";
	import {
		NetworkQualityContext,
		setNetworkQualityContext
	} from "@lib/components/statistics/global/network-quality-context.svelte";
	import StationStatisticsHeader from "@lib/components/statistics/station/StationStatisticsHeader.svelte";

	let { data }: PageProps = $props();

	// svelte-ignore state_referenced_locally
	const scopeContext = new StatisticsScopeContext(data.scope);
	setStatisticsScopeContext(scopeContext);
	// svelte-ignore state_referenced_locally
	const networkQualityContext = new NetworkQualityContext(data.networkQuality.promises, data.evaNumber);
	setNetworkQualityContext(networkQualityContext);

	let networkKpiStripLoading: boolean = $state(true);
	let stationQualityLoading: boolean = $state(true);
	let stationTransportBreakdownLoading: boolean = $state(true);
	let administrationRankingLoading: boolean = $state(true);
	let lineRankingLoading: boolean = $state(true);
	const metricLoading = $derived(
		networkKpiStripLoading ||
			stationQualityLoading ||
			stationTransportBreakdownLoading ||
			administrationRankingLoading ||
			lineRankingLoading
	);

	let networkInitialized = false;
	$effect(() => {
		const scope = scopeContext.current;
		if (!scope) return;

		if (!networkInitialized) {
			networkInitialized = true;
			return;
		}

		networkQualityContext.update(scope);
	});
</script>

<svelte:head>
	<title>{data.station.name} Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-8 p-4 sm:py-8">
	<StationStatisticsHeader station={data.station} gatheringPromise={data.gatheringPromise} />

	<StatisticsScopeControls isUpdating={metricLoading} />

	<NetworkKpiStrip bind:isLoading={networkKpiStripLoading} />

	<NetworkQualityTimeSeries
		bind:isLoading={stationQualityLoading}
		title="Station Quality Trends"
		description="Read this station as separate signals: daily event rhythm, punctuality thresholds, cancellations, and average delay."
	/>

	<NetworkTransportBreakdown
		bind:isLoading={stationTransportBreakdownLoading}
		title="Station Transport Mix & Quality"
		description="Compare how recorded transport families contribute to this station's activity and quality."
	/>

	<AdministrationRanking
		bind:isLoading={administrationRankingLoading}
		settings={data.administrationRanking.settings}
		promise={data.administrationRanking.promise}
		evaNumber={data.evaNumber}
	/>

	<LineRanking
		bind:isLoading={lineRankingLoading}
		settings={data.lineRanking.settings}
		promise={data.lineRanking.promise}
		evaNumber={data.evaNumber}
	/>
</main>
