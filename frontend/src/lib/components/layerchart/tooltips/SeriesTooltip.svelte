<script module lang="ts">
	type SeriesTooltipItem<TData> = {
		key: Extract<keyof TData, string>;
		label?: string;
		color?: string;
		formatValue?: (value: unknown, data: TData) => string | number;
	};

	export type { SeriesTooltipItem };
</script>

<script lang="ts" generics="TData">
	import { Tooltip, type ChartState } from "layerchart";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		context: ChartState<TData>;
		items: SeriesTooltipItem<TData>[];
		header: (data: TData) => string | number;
		class?: ClassValue;
	};

	let { context, items, header, class: className }: Props = $props();

	const visibleSeries = $derived(context.tooltip.series.filter((series) => series.visible));

	const getItem = (key: string): SeriesTooltipItem<TData> | undefined => items.find((item) => item.key === key);

	const formatValue = (key: string, value: unknown, data: TData): string | number => {
		const item = getItem(key);
		if (item?.formatValue) return item.formatValue(value, data);
		if (value === null || value === undefined || value === "") return "-";
		return typeof value === "string" || typeof value === "number" ? value : String(value);
	};
</script>

<Tooltip.Root
	{context}
	variant="none"
	class={[
		"border-border bg-background/95 text-foreground min-w-56 rounded-lg border-2 px-3 py-2 shadow-xl backdrop-blur-sm",
		className
	]}
>
	{#snippet children({ data }: { data: TData })}
		<Tooltip.Header value={header(data)} class="border-border/70 mb-2 border-b pb-2 text-xs font-bold" />
		<Tooltip.List>
			{#each visibleSeries as series (series.key)}
				{@const item = getItem(series.key)}
				<Tooltip.Item
					label={item?.label ?? series.label ?? series.key}
					value={formatValue(series.key, series.value, data)}
					color={item?.color ?? series.color}
					valueAlign="right"
					data-highlighted={context.series.isHighlighted(series.key, true)}
					classes={{
						label: "text-foreground/65 text-xs font-semibold",
						value: "text-foreground text-xs font-bold"
					}}
				/>
			{/each}
		</Tooltip.List>
	{/snippet}
</Tooltip.Root>
