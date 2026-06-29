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
	import type { NetworkTimeSeriesPromises } from "../network-time-series/network-time-series";
	import { aggregateMetricSeries } from "../network-quality-utils";
	import { formatMetricValue } from "../../metric-format";
	import { getNetworkQualityContext } from "../network-quality-context.svelte";

	type KpiDefinition = {
		key: keyof NetworkTimeSeriesPromises;
		title: string;
		icon: LucideIcon;
		seriesType: MetricSeriesType;
	};

	type Props = {
		isLoading: boolean;
	};
	let { isLoading = $bindable(true) }: Props = $props();

	const networkQualityContext = getNetworkQualityContext();
	let promises: NetworkTimeSeriesPromises = $derived(networkQualityContext.promises);

	$effect(() => {
		isLoading = networkQualityContext.isLoading();
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
			title: "Punctual <= 5:59 min",
			icon: ShieldCheck,
			seriesType: MetricSeriesType.STATION_EVENT_PUNCTUALITY5_RATE
		},
		{
			key: "punctuality15",
			title: "Punctual <= 14:59 min",
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
