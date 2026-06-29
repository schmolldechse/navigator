<script lang="ts">
	import Activity from "@lucide/svelte/icons/activity";
	import NetworkAverageDelayTrendChart from "./NetworkAverageDelayTrendChart.svelte";
	import NetworkCancellationTrendChart from "./NetworkCancellationTrendChart.svelte";
	import NetworkEventHourDistributionChart from "./NetworkEventHourDistributionChart.svelte";
	import NetworkPunctualityThresholdTrendChart from "./NetworkPunctualityThresholdTrendChart.svelte";
	import { getStatisticsScopeContext } from "../statistics-scope-context.svelte";
	import { getNetworkQualityContext } from "../network-quality-context.svelte";
	import type { NetworkTimeSeriesPromises } from "./network-time-series";

	type Props = {
		isLoading: boolean;
		title?: string;
		description?: string;
	};
	let {
		isLoading = $bindable(true),
		title = "Network Quality Trends",
		description = "Read the network as separate signals: daily event rhythm, punctuality thresholds, cancellations, and average delay."
	}: Props = $props();

	const scopeContext = getStatisticsScopeContext();
	const networkQualityContext = getNetworkQualityContext();
	let promises: NetworkTimeSeriesPromises = $derived(networkQualityContext.promises);

	$effect(() => {
		isLoading = networkQualityContext.isLoading();
	});
</script>

<section class="space-y-4">
	<div class="flex items-center gap-2">
		<Activity size={22} class="text-accent" />
		<h2 class="text-2xl font-semibold">{title}</h2>
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		{description}
	</p>

	<div class="grid gap-4 xl:grid-cols-2">
		<NetworkEventHourDistributionChart promise={promises.eventCount} scheduleType={scopeContext.current.scheduleType} />

		<NetworkPunctualityThresholdTrendChart
			punctuality5Promise={promises.punctuality5}
			punctuality15Promise={promises.punctuality15}
		/>

		<NetworkCancellationTrendChart
			cancellationRatePromise={promises.cancellationRate}
			cancellationCountPromise={promises.cancellationCount}
		/>

		<NetworkAverageDelayTrendChart promise={promises.averageDelay} />
	</div>
</section>
