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
	label="Average delay"
	title="Measured station-event delay over time"
	errorTitle="The delay trend could not be loaded."
/>
