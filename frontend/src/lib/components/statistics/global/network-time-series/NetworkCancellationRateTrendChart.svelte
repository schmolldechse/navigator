<script lang="ts">
	import type { MetricSeries } from "@lib/api";
	import NetworkSingleMetricTrendChart from "./NetworkSingleMetricTrendChart.svelte";

	type Props = {
		promise: Promise<MetricSeries>;
	};
	let { promise }: Props = $props();
</script>

<NetworkSingleMetricTrendChart
	{promise}
	label="Cancellation rate"
	title="Cancelled station events over time"
	errorTitle="The cancellation trend could not be loaded."
/>
