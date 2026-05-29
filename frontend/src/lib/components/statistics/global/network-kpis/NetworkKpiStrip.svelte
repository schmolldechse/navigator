<script lang="ts">
	import { MetricSeriesType, MetricUnit, type MetricSeries } from "@lib/api";
	import Kpi from "@lib/components/ui/Kpi.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import Activity from "@lucide/svelte/icons/activity";
	import Ban from "@lucide/svelte/icons/ban";
	import Clock from "@lucide/svelte/icons/clock";
	import Gauge from "@lucide/svelte/icons/gauge";
	import ShieldCheck from "@lucide/svelte/icons/shield-check";
	import type { LucideIcon } from "@lucide/svelte";
	import { createNetworkTimeSeriesRequests, type NetworkTimeSeriesPromises } from "../network-time-series/network-time-series";
	import { aggregateMetricSeries } from "../network-quality-utils";
	import { getStatisticsScopeContext } from "../statistics-scope-context.svelte";
	import { loadMetric } from "@lib/remote/metrics.remote";
	import { formatMetricValue } from "../../metric-format";

	type KpiDefinition = {
		key: keyof NetworkTimeSeriesPromises;
		title: string;
		icon: LucideIcon;
		seriesType: MetricSeriesType;
	};

	type Props = {
		isLoading: boolean;
		promises: NetworkTimeSeriesPromises;
	};
	let { isLoading = $bindable(true), promises: initialPromises }: Props = $props();

	const scopeContext = getStatisticsScopeContext();

	const createPromises = (requests: ReturnType<typeof createNetworkTimeSeriesRequests>): NetworkTimeSeriesPromises => ({
		eventCount: loadMetric({ request: requests.eventCount }),
		cancellationCount: loadMetric({ request: requests.cancellationCount }),
		punctuality5: loadMetric({ request: requests.punctuality5 }),
		punctuality15: loadMetric({ request: requests.punctuality15 }),
		cancellationRate: loadMetric({ request: requests.cancellationRate }),
		averageDelay: loadMetric({ request: requests.averageDelay })
	});

	let requests = $derived(createNetworkTimeSeriesRequests(scopeContext.current));
	// svelte-ignore state_referenced_locally
	let promises: NetworkTimeSeriesPromises = $state(initialPromises);

	let initialized = false;
	$effect(() => {
		const currentRequests = requests;
		if (!currentRequests) return;

		if (!initialized) {
			initialized = true;
			return;
		}

		promises = createPromises(currentRequests);
	});

	$effect(() => {
		const currentPromises = promises;
		if (!currentPromises) return;

		isLoading = Object.values(currentPromises).some(
			(promise: Promise<MetricSeries>) => "loading" in promise && Boolean(promise.loading)
		);
	});

	const definitions: KpiDefinition[] = [
		{
			key: "eventCount",
			title: "Station Events",
			icon: Activity,
			seriesType: MetricSeriesType.STATION_EVENT_COUNT
		},
		{
			key: "cancellationCount",
			title: "Cancelled Events",
			icon: Ban,
			seriesType: MetricSeriesType.STATION_EVENT_CANCELLATION_COUNT
		},
		{
			key: "cancellationRate",
			title: "Cancellation Rate",
			icon: Gauge,
			seriesType: MetricSeriesType.STATION_EVENT_CANCELLATION_RATE
		},
		{
			key: "averageDelay",
			title: "Average Delay",
			icon: Clock,
			seriesType: MetricSeriesType.STATION_EVENT_DELAY_AVERAGE
		},
		{
			key: "punctuality5",
			title: "Punctuality 5",
			icon: ShieldCheck,
			seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY5_RATE
		},
		{
			key: "punctuality15",
			title: "Punctuality 15",
			icon: ShieldCheck,
			seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY15_RATE
		}
	];

	const validateMetric = (metric: MetricSeries, definition: KpiDefinition) => {
		if (metric.seriesType !== definition.seriesType) throw new Error(`Expected ${definition.seriesType}`);
		return metric;
	};
</script>

<section class="grid gap-3 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-6">
	{#each definitions as definition (definition.key)}
		{@const promise = promises[definition.key].then((metric) => validateMetric(metric, definition))}

		{#await promise}
			<Kpi title={definition.title} loading icon={definition.icon} />
		{:then metric}
			{@const aggregate = aggregateMetricSeries(metric)}
			{@const formatted = formatMetricValue(Number(aggregate.value), aggregate.unit)}

			<Kpi title={definition.title} metric={formatted} icon={definition.icon}>
				{#snippet footer()}
					{#if aggregate.sample && aggregate.unit !== MetricUnit.COUNT}
						<p class="text-foreground/55 text-xs">
							Weighted by {Number(aggregate.sample.denominator).toLocaleString()} measured events
						</p>
					{/if}
				{/snippet}
			</Kpi>
		{:catch}
			<Kpi title={definition.title} icon={definition.icon}>
				{#snippet footer()}
					<MetricLoadingFailedWarning />
				{/snippet}
			</Kpi>
		{/await}
	{/each}
</section>
