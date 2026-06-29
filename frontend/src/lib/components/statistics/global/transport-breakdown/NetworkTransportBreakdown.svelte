<script lang="ts">
	import { MetricUnit, type MetricSeries } from "@lib/api";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import TrainFront from "@lucide/svelte/icons/train-front";
	import { Axis, BarChart, Tooltip } from "layerchart";
	import type { NetworkTimeSeriesPromises } from "../network-time-series/network-time-series";
	import { createTransportBreakdownRows, type TransportBreakdownRow } from "../network-quality-utils";
	import { formatMetricValue } from "../../metric-format";
	import { getNetworkQualityContext } from "../network-quality-context.svelte";

	type BreakdownOption = {
		key: keyof NetworkTimeSeriesPromises;
		label: string;
		description: string;
	};

	type Props = {
		isLoading: boolean;
		title?: string;
		description?: string;
	};
	let {
		isLoading = $bindable(true),
		title = "Transport Mix & Quality",
		description = "Compare how much each transport family contributes and how quality differs inside the selected scope."
	}: Props = $props();

	const networkQualityContext = getNetworkQualityContext();
	let promises: NetworkTimeSeriesPromises = $derived(networkQualityContext.promises);

	const options: BreakdownOption[] = [
		{ key: "eventCount", label: "Activity", description: "Recorded station events by transport family." },
		{ key: "cancellationRate", label: "Cancellation Rate", description: "Cancelled station-event share by transport family." },
		{ key: "averageDelay", label: "Average Delay", description: "Weighted delay by transport family." },
		{
			key: "punctuality5",
			label: "Punctual <= 5:59 min",
			description: "Events with less than six minutes delay by transport family."
		},
		{
			key: "punctuality15",
			label: "Punctual <= 14:59 min",
			description: "Events with less than fifteen minutes delay by transport family."
		}
	];

	let selectedOption: BreakdownOption = $state(options[0]);

	type Row = TransportBreakdownRow & {
		label: string;
		displayValue: number;
		formatted: string;
	};

	const series = [
		{
			key: "displayValue",
			label: "Value",
			value: (row: Row) => row.displayValue,
			color: "var(--color-accent)"
		}
	];

	const createRows = (metric: MetricSeries, eventCount: MetricSeries): Row[] =>
		createTransportBreakdownRows(metric, eventCount)
			.map((row) => {
				const formatted = formatMetricValue(Number(row.value), row.unit);
				return {
					...row,
					label: row.family.label,
					displayValue: row.value ?? 0,
					formatted: [formatted.value, formatted.unit].filter(Boolean).join(" ")
				};
			})
			.sort((left, right) => right.eventCount - left.eventCount);

	$effect(() => {
		isLoading = networkQualityContext.isLoading();
	});
</script>

<section class="space-y-4">
	<div class="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
		<div class="flex min-w-0 flex-col gap-y-1">
			<div class="flex items-center gap-2">
				<TrainFront size={22} class="text-accent" />
				<h2 class="text-2xl font-semibold">{title}</h2>
			</div>
			<p class="text-foreground/60 max-w-3xl text-sm leading-relaxed sm:text-base">
				{description}
			</p>
		</div>

		<ToggleGroup
			mode="single"
			allowEmpty={false}
			selected={selectedOption}
			keyFn={(option: BreakdownOption) => option.key}
			onselect={(option: BreakdownOption | undefined) => {
				if (option) selectedOption = option;
			}}
			class="gap-1.5 lg:justify-end"
		>
			{#each options as option (option.key)}
				<ToggleGroupItem
					item={option}
					title={option.description}
					aria-label={`${option.label}: ${option.description}`}
					class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground px-2.5 py-1.5 text-xs font-semibold transition-colors"
				>
					{option.label}
				</ToggleGroupItem>
			{/each}
		</ToggleGroup>
	</div>

	<Card class="gap-y-4">
		<div>
			<p class="text-foreground/60 text-xs font-medium">{selectedOption.label}</p>
			<h3 class="text-foreground text-base font-semibold">Transport families</h3>
		</div>

		{#await Promise.all([promises[selectedOption.key], promises.eventCount])}
			<div class="flex min-h-80 flex-col gap-y-3">
				<Skeleton class="h-64 w-full" />
				<Skeleton class="h-5 w-2/3" />
			</div>
		{:then [metric, eventCount]}
			{@const rows = createRows(metric, eventCount)}
			{#if rows.length === 0}
				<div
					class="bg-secondary/30 border-border flex min-h-80 flex-col items-center justify-center rounded-lg border text-center"
				>
					<p class="text-foreground font-semibold">No transport data available</p>
					<p class="text-foreground/60 max-w-sm text-sm">Try a wider time range or another scope.</p>
				</div>
			{:else}
				<BarChart
					data={rows}
					{series}
					x="label"
					yDomain={metric.unit === MetricUnit.PERCENT ? [0, 100] : [0, null]}
					height={320}
					padding={{ left: 52, right: 16, bottom: 32 }}
					tooltipContext={{ mode: "band" }}
				>
					{#snippet axis()}
						<Axis placement="left" rule tickLabelProps={{ textAnchor: "end" }} classes={{ root: "select-none" }} />
						<Axis placement="bottom" rule classes={{ root: "select-none" }} />
					{/snippet}

					{#snippet tooltip({ context })}
						<Tooltip.Root
							{context}
							anchor="top"
							contained="container"
							class="bg-background/95! border-border! w-72 rounded-lg border px-3 py-2 shadow-xl backdrop-blur-md select-none"
						>
							<Tooltip.Header>{context.tooltip.data?.label}</Tooltip.Header>
							<Tooltip.List class="grid-cols-[minmax(0,1fr)_max-content] gap-x-8 gap-y-1">
								<Tooltip.Item label={selectedOption.label}>
									<span class="text-foreground font-bold tabular-nums">{context.tooltip.data?.formatted}</span>
								</Tooltip.Item>

								<Tooltip.Item label="Station events">
									{@const eventValue = formatMetricValue(context.tooltip.data?.eventCount, MetricUnit.COUNT)}
									<span class="text-foreground font-bold tabular-nums">{eventValue.value}</span>
								</Tooltip.Item>
							</Tooltip.List>
						</Tooltip.Root>
					{/snippet}
				</BarChart>
			{/if}
		{:catch error}
			<div
				class="bg-secondary/30 border-border flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
			>
				<CircleAlert size={32} class="text-destructive" />
				<p class="text-foreground font-semibold">The transport breakdown could not be loaded.</p>
				<p class="text-foreground/60 max-w-xl text-sm">{error.message}</p>
			</div>
		{/await}
	</Card>
</section>
