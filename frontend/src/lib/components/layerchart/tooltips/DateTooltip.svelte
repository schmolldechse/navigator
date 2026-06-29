<script lang="ts" generics="T extends { date: Date }">
	import { type ChartState, Tooltip } from "layerchart";
	import { DateTime } from "luxon";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";

	type Props = {
		context: ChartState;
		value?: (data: T) => string;
	};
	let { context, value }: Props = $props();
</script>

<Tooltip.Root
	x="pointer"
	y={context.height + context.padding.top + 2}
	anchor="top"
	variant="none"
	class="bg-background border-border rounded-lg border-2 px-2"
>
	{#snippet children({ data }: { data: T })}
		<span class="text-text text-xs">
			{#if "date" in (data as any) && (data as any).date instanceof Date}
				{@const date = DateTime.fromJSDate((data as any).date)}
				{date.toLocaleString(DateTime.DATETIME_MED)}
			{:else if value}
				{value(data)}
			{:else}
				<CircleAlert size={22} />
				No data available.
			{/if}
		</span>
	{/snippet}
</Tooltip.Root>
