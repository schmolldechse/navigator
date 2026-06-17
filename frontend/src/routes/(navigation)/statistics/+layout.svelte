<script lang="ts">
	import type { LayoutProps } from "./$types";
	import {
		StatisticsDashboardContext,
		setStatisticsDashboardContext
	} from "@lib/components/statistics/shared/statistics-context.svelte";

	let { children }: LayoutProps = $props();

	const statistics = new StatisticsDashboardContext();
	setStatisticsDashboardContext(statistics);
</script>

{@render children?.()}
