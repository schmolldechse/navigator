<script module lang="ts">
	type DistributionBarItem = {
		key: string;
		label: string;
		value: number;
		color: string;
	};

	type DistributionBarItemView = DistributionBarItem & {
		percentage: number;
	};

	export type { DistributionBarItem };
</script>

<script lang="ts">
	import type { ClassValue } from "svelte/elements";

	type Props = {
		items: DistributionBarItem[];
		ariaLabel: string;
		class?: ClassValue;
		barClass?: ClassValue;
		legendClass?: ClassValue;
		showLegend?: boolean;
		showValues?: boolean;
		valueFormatter?: (value: number) => string;
		percentageFormatter?: (percentage: number) => string;
	};
	let {
		items,
		ariaLabel,
		class: className,
		barClass,
		legendClass,
		showLegend = true,
		showValues = false,
		valueFormatter = (value: number) => value.toLocaleString(),
		percentageFormatter = (percentage: number) => `${percentage.toLocaleString(undefined, { maximumFractionDigits: 1 })}%`
	}: Props = $props();

	const total = $derived(items.reduce((sum: number, item: DistributionBarItem) => sum + item.value, 0));
	const itemViews = $derived(
		items.map(
			(item: DistributionBarItem): DistributionBarItemView => ({
				...item,
				percentage: total === 0 ? 0 : (item.value / total) * 100
			})
		)
	);
</script>

<div class={["flex flex-col gap-y-3", className]}>
	<div
		class={["border-border bg-secondary flex h-3 w-full overflow-hidden rounded-full border", barClass]}
		aria-label={ariaLabel}
		role="img"
	>
		{#each itemViews as item (item.key)}
			<div
				class={["h-full transition-[width] duration-300", item.value > 0 && "min-w-px"]}
				style:width={`${item.percentage}%`}
				style:background-color={item.color}
				title={`${item.label}: ${valueFormatter(item.value)} (${percentageFormatter(item.percentage)})`}
			></div>
		{/each}
	</div>

	{#if showLegend}
		<div class={["grid grid-cols-2 gap-3", legendClass]}>
			{#each itemViews as item (item.key)}
				<div class="flex min-w-0 items-center justify-between gap-x-3">
					<div class="flex min-w-0 items-center gap-x-2">
						<div class="h-2.5 w-2.5 shrink-0 rounded-full" style:background-color={item.color}></div>
						<span class="text-foreground/70 truncate text-xs font-semibold uppercase">{item.label}</span>
					</div>

					<span class="text-foreground text-xs font-semibold tabular-nums">
						{showValues ? valueFormatter(item.value) : percentageFormatter(item.percentage)}
					</span>
				</div>
			{/each}
		</div>
	{/if}
</div>
