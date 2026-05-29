<script lang="ts">
	import type { MetricSeries } from "@lib/api";
	import { loadMetric } from "@lib/remote/metrics.remote";
	import Activity from "@lucide/svelte/icons/activity";
	import { createNetworkTimeSeriesRequests, type NetworkTimeSeriesPromises } from "./network-time-series";
	import NetworkAverageDelayTrendChart from "./NetworkAverageDelayTrendChart.svelte";
	import NetworkCancellationTrendChart from "./NetworkCancellationTrendChart.svelte";
	import NetworkEventHourDistributionChart from "./NetworkEventHourDistributionChart.svelte";
	import NetworkPunctualityThresholdTrendChart from "./NetworkPunctualityThresholdTrendChart.svelte";
	import { getStatisticsScopeContext } from "../statistics-scope-context.svelte";

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
</script>

<section class="space-y-4">
	<div class="flex items-center gap-2">
		<Activity size={22} class="text-accent" />
		<h2 class="text-2xl font-semibold">Network Quality Trends</h2>
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		Read the network as separate signals: daily event rhythm, punctuality thresholds, cancellations, and average delay.
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
