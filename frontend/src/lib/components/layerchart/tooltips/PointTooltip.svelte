<script module lang="ts">
	type PointTooltipItem<TData> = {
		key: string;
		label: string;
		value: (data: TData) => string | number;
		color?: string | ((data: TData) => string);
	};

	export type { PointTooltipItem };
</script>

<script lang="ts" generics="TData">
	import { Tooltip, type ChartState } from "layerchart";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		context: ChartState<TData>;
		items: PointTooltipItem<TData>[];
		header: (data: TData) => string | number;
		class?: ClassValue;
	};

	let { context, items, header, class: className }: Props = $props();

	const getColor = (item: PointTooltipItem<TData>, data: TData): string | undefined =>
		typeof item.color === "function" ? item.color(data) : item.color;
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
			{#each items as item (item.key)}
				<Tooltip.Item
					label={item.label}
					value={item.value(data)}
					color={getColor(item, data)}
					valueAlign="right"
					classes={{
						label: "text-foreground/65 text-xs font-semibold",
						value: "text-foreground text-xs font-bold"
					}}
				/>
			{/each}
		</Tooltip.List>
	{/snippet}
</Tooltip.Root>
