<script lang="ts">
	import type { PageProps } from "./$types";
	import StationMetricMap from "@lib/components/statistics/global/station-metric-map/StationMetricMap.svelte";
	import AdministrationRanking from "@lib/components/statistics/global/administration-ranking/AdministrationRanking.svelte";
	import NetworkQualityTimeSeries from "@lib/components/statistics/global/network-time-series/NetworkQualityTimeSeries.svelte";
	import NetworkKpiStrip from "@lib/components/statistics/global/network-kpis/NetworkKpiStrip.svelte";
	import NetworkTransportBreakdown from "@lib/components/statistics/global/transport-breakdown/NetworkTransportBreakdown.svelte";
	import LineRanking from "@lib/components/statistics/global/line-ranking/LineRanking.svelte";
	import * as Accordion from "@lib/components/ui/accordion";
	import StatisticsScopeControls from "@lib/components/statistics/global/StatisticsScopeControls.svelte";
	import {
		setStatisticsScopeContext,
		StatisticsScopeContext
	} from "@lib/components/statistics/global/statistics-scope-context.svelte";

	let { data }: PageProps = $props();

	let faqValues: string[] = $state([]);

	// svelte-ignore state_referenced_locally
	const scopeContext = new StatisticsScopeContext(data.scope);
	setStatisticsScopeContext(scopeContext);

	let stationMetricMapLoading: boolean = $state(true);
	let networkKpiStripLoading: boolean = $state(true);
	let networkQualityLoading: boolean = $state(true);
	let administrationRankingLoading: boolean = $state(true);
	let lineRankingLoading: boolean = $state(true);
	const metricLoading = $derived(
		stationMetricMapLoading ||
			networkKpiStripLoading ||
			networkQualityLoading ||
			administrationRankingLoading ||
			lineRankingLoading
	);
</script>

<svelte:head>
	<title>Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-8 p-4 sm:py-8">
	<section class="relative flex flex-col gap-4">
		<div class="grid gap-1">
			<h1 class="text-3xl font-bold">Statistics</h1>
			<p class="text-foreground/60 max-w-3xl text-sm sm:text-base">
				Compare activity and quality across the selected network slice.
			</p>
		</div>
	</section>

	<StatisticsScopeControls isUpdating={metricLoading} />

	<NetworkKpiStrip bind:isLoading={networkKpiStripLoading} promises={data.networkQuality.promises} />

	<StationMetricMap
		bind:isLoading={stationMetricMapLoading}
		settings={data.stationMetricMap.settings}
		promise={data.stationMetricMap.promise}
	/>

	<NetworkQualityTimeSeries bind:isLoading={networkQualityLoading} promises={data.networkQuality.promises} />

	<NetworkTransportBreakdown promises={data.networkQuality.promises} />

	<AdministrationRanking
		bind:isLoading={administrationRankingLoading}
		settings={data.administrationRanking.settings}
		promise={data.administrationRanking.promise}
	/>

	<LineRanking bind:isLoading={lineRankingLoading} settings={data.lineRanking.settings} promise={data.lineRanking.promise} />

	<section class="space-y-4">
		<div class="flex flex-col gap-y-1">
			<h2 class="text-2xl font-medium">Statistics FAQ</h2>
			<p class="text-foreground/60 max-w-3xl text-sm sm:text-base">Short answers to the terms and choices that matter most.</p>
		</div>

		<Accordion.Root type="multiple" bind:value={faqValues}>
			<Accordion.Item value="misleading-100">
				<Accordion.Trigger>Why can 100% be misleading?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Percent values are only as strong as their denominator. A line with one punctual measured journey can show 100
						percent punctuality, while another line with thousands of journeys and 99 percent punctuality is usually the more
						reliable signal. Tooltips and ranking samples expose numerator and denominator so small samples stay visible.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="journey-description">
				<Accordion.Trigger>What is a journey description?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						A journey description is the public-facing service description recorded for a journey, for example a category or
						route label such as RE, S, or ICE. In line rankings it helps group and filter services together with the numeric
						journey or line number.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="arrivals-departures">
				<Accordion.Trigger>Why are arrivals and departures evaluated separately?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Arrivals and departures answer different questions. Arrival quality is closer to whether passengers reach a station
						on time, while departure quality shows how reliably services leave a station. Keeping them separate avoids mixing
						two operational moments that can have different delays, cancellations, and passenger impact.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="replacement-services">
				<Accordion.Trigger>How are replacement services handled?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Replacement services are included by default so the dashboard reflects the full recorded service offer. The Scope
						settings can exclude them when you want to focus on regular services only. A service is treated as replacement
						transport when the recorded journey or its transport information marks it that way.
					</p>
				</Accordion.Content>
			</Accordion.Item>
		</Accordion.Root>
	</section>
</main>
