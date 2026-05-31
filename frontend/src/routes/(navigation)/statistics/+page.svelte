<script lang="ts">
	import { goto } from "$app/navigation";
	import { page } from "$app/state";
	import type { PageProps } from "./$types";
	import type { Station } from "@lib/api";
	import StationMetricMap from "@lib/components/statistics/global/station-metric-map/StationMetricMap.svelte";
	import AdministrationRanking from "@lib/components/statistics/global/administration-ranking/AdministrationRanking.svelte";
	import NetworkQualityTimeSeries from "@lib/components/statistics/global/network-time-series/NetworkQualityTimeSeries.svelte";
	import NetworkKpiStrip from "@lib/components/statistics/global/network-kpis/NetworkKpiStrip.svelte";
	import NetworkTransportBreakdown from "@lib/components/statistics/global/transport-breakdown/NetworkTransportBreakdown.svelte";
	import LineRanking from "@lib/components/statistics/global/line-ranking/LineRanking.svelte";
	import StationSearch from "@lib/components/station/StationSearch.svelte";
	import * as Accordion from "@lib/components/ui/accordion";
	import StatisticsScopeControls from "@lib/components/statistics/global/StatisticsScopeControls.svelte";
	import {
		setStatisticsScopeContext,
		StatisticsScopeContext
	} from "@lib/components/statistics/global/statistics-scope-context.svelte";
	import {
		NetworkQualityContext,
		setNetworkQualityContext
	} from "@lib/components/statistics/global/network-quality-context.svelte";

	let { data }: PageProps = $props();

	let faqValues: string[] = $state([]);

	// svelte-ignore state_referenced_locally
	const scopeContext = new StatisticsScopeContext(data.scope);
	setStatisticsScopeContext(scopeContext);
	// svelte-ignore state_referenced_locally
	const networkQualityContext = new NetworkQualityContext(data.networkQuality.promises);
	setNetworkQualityContext(networkQualityContext);

	let stationMetricMapLoading: boolean = $state(true);
	let networkKpiStripLoading: boolean = $state(true);
	let networkQualityLoading: boolean = $state(true);
	let networkTransportBreakdownLoading: boolean = $state(true);
	let administrationRankingLoading: boolean = $state(true);
	let lineRankingLoading: boolean = $state(true);
	const metricLoading = $derived(
		stationMetricMapLoading ||
			networkKpiStripLoading ||
			networkQualityLoading ||
			networkTransportBreakdownLoading ||
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

	const selectStation = async (station: Station) => {
		const evaNumber = Number(station.evaNumber);
		const search = page.url.searchParams.toString();
		await goto(`/statistics/${evaNumber}${search ? `?${search}` : ""}`);
	};
</script>

<svelte:head>
	<title>Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-12 p-4 sm:py-8">
	<section class="relative">
		<div class="bg-accent absolute top-1 bottom-1 w-0.5 sm:-left-4 sm:w-1"></div>

		<div class="flex flex-col gap-y-2 pl-4 sm:pl-8">
			<h1 class="text-3xl font-bold text-balance sm:text-4xl">Statistics</h1>
			<p class="text-foreground/60 max-w-3xl text-sm leading-relaxed sm:text-base">
				Compare activity and quality across the selected network slice.
			</p>
		</div>
	</section>

	<section class="border-border bg-secondary/10 -mx-4 border-y px-4 py-8 sm:-mx-6 sm:px-6">
		<div class="flex flex-col gap-y-5">
			<div class="flex max-w-3xl flex-col gap-y-2">
				<h2 class="text-2xl font-medium">Station metrics</h2>
				<p class="text-foreground/60 max-w-3xl text-sm leading-relaxed sm:text-base">
					Search a station to open station-specific punctuality, delay, cancellation, operator, and line statistics.
				</p>
			</div>

			<StationSearch class="w-full max-w-3xl" onselect={selectStation} />
		</div>
	</section>

	<section class="flex flex-col gap-y-8">
		<div class="flex flex-col gap-y-2">
			<h2 class="text-2xl font-medium">Global metrics</h2>
			<p class="text-foreground/60 max-w-3xl text-sm leading-relaxed sm:text-base">
				Filter the collected network data and compare quality signals across stations, transport families, operators, and lines.
			</p>
		</div>

		<StatisticsScopeControls isUpdating={metricLoading} />

		<NetworkKpiStrip bind:isLoading={networkKpiStripLoading} />

		<StationMetricMap
			bind:isLoading={stationMetricMapLoading}
			settings={data.stationMetricMap.settings}
			promise={data.stationMetricMap.promise}
		/>

		<NetworkQualityTimeSeries bind:isLoading={networkQualityLoading} />

		<NetworkTransportBreakdown bind:isLoading={networkTransportBreakdownLoading} />

		<AdministrationRanking
			bind:isLoading={administrationRankingLoading}
			settings={data.administrationRanking.settings}
			promise={data.administrationRanking.promise}
		/>

		<LineRanking bind:isLoading={lineRankingLoading} settings={data.lineRanking.settings} promise={data.lineRanking.promise} />
	</section>

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
