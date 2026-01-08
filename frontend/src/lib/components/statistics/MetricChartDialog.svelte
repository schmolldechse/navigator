<script lang="ts">
	import X from "@lucide/svelte/icons/x";
	import { MetricUnit, type MetricDataPoint, type MetricSeries } from "$lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { Area, AreaChart, Axis, chartDataArray, Highlight, LinearGradient, Svg, Tooltip } from "layerchart";
	import { formatBytes, getUnit } from "@lib/util/bytes";
	import { DateTime } from "luxon";
	import type { SeriesTypeTitles } from "../../../routes/(navigation)/+page.svelte";

	interface Props {
		isVisible: boolean;
		title: string;
		metrics: MetricSeries[];
		seriesTitles?: SeriesTypeTitles[];
		onclose?: () => void;
	}

	let { isVisible = $bindable(), title, metrics, seriesTitles, onclose }: Props = $props();

	let isUnitBytes: boolean = $derived(metrics.every((metric: MetricSeries) => metric.unit === MetricUnit.BYTES));
	let isCumulative: boolean = $derived(metrics.every((metric: MetricSeries) => metric.isCumulative));

	const colorScale = scaleOrdinal(schemeTableau10);
	const chartSeries = $derived.by(() => {
		if (!metrics || metrics.length === 0) return [];

		return metrics.map((metric: MetricSeries) => {
			const sortedDataPoints = [...metric.dataPoints].sort(
				(a: MetricDataPoint, b: MetricDataPoint) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
			);

			const rawValues = sortedDataPoints.map((dataPoint: MetricDataPoint) => ({
				date: new Date(dataPoint.timestamp),
				value: Number(dataPoint.value)
			}));

			if (isCumulative)
				return {
					data: rawValues,
					key: metric.seriesType,
					color: colorScale(metric.seriesType)
				};

			const absoluteValues = rawValues.map((currentItem, index) => {
				if (index === 0) return { ...currentItem, value: 0 };

				const difference = currentItem.value - rawValues[index - 1].value;
				return { ...currentItem, value: difference };
			});

			return {
				data: absoluteValues,
				key: metric.seriesType,
				color: colorScale(metric.seriesType)
			};
		});
	});

	let dialog: HTMLDialogElement | undefined = $state(undefined);

	$effect(() => {
		if (!dialog) return;
		isVisible ? dialog.showModal() : dialog.close();
	});

	const handleClose = () => {
		isVisible = false;
		onclose?.();
	};

	let innerWidth: number = $state(0);
</script>

<svelte:window bind:innerWidth />

<dialog
	bind:this={dialog}
	onclose={handleClose}
	onclick={(event) => event.target === dialog && handleClose()}
	class={[
		"m-0 h-fit max-h-none w-full max-w-none bg-transparent p-0 outline-none",
		"fixed inset-x-0 top-auto bottom-0", // mobile: bottom sheet
		"md:inset-auto md:top-1/2 md:left-1/2 md:w-[90vw] md:max-w-5xl md:-translate-x-1/2 md:-translate-y-1/2" // desktop: centered
	]}
>
	<div
		class="border-muted-foreground/20 bg-background flex w-full flex-col space-y-4 rounded-t-2xl border-t-2 p-6 shadow-2xl md:rounded-2xl md:border-2"
	>
		<!-- Header -->
		<div class="flex items-center justify-between">
			<div class="space-y-1">
				<h2 class="text-text text-xl font-bold tracking-tight">{title}</h2>
				<p class="text-muted-foreground text-sm">Historical performance data</p>
			</div>
			<button
				onclick={handleClose}
				class="hover:bg-muted-foreground/10 hover:stroke-accent cursor-pointer rounded-full p-1 transition-colors"
			>
				<X class="stroke-muted-foreground hover:stroke-accent h-5 w-5" />
			</button>
		</div>

		<!-- Settings -->
		<div class="bg-muted/30 flex items-center gap-x-3 rounded-xl p-3">
			<span class="text-text/80 text-sm font-medium">View Mode:</span>
			<div class="flex items-center gap-2">
				{#snippet button(label: string, isActive: boolean, onclick: () => void)}
					<button
						{onclick}
						class={[
							"rounded-md px-3 py-1 text-xs font-bold transition-all",
							isActive ? "bg-accent text-background" : "text-muted-foreground hover:text-text"
						]}
					>
						{label}
					</button>
				{/snippet}

				{@render button("Absolute", !isCumulative, () => (isCumulative = false))}
				{@render button("Cumulative", isCumulative, () => (isCumulative = true))}
			</div>
		</div>

		{#if metrics == null || chartSeries.length === 0}
			<div class="bg-muted/30 flex h-75 animate-pulse items-center justify-center rounded-xl md:h-125">
				<span class="text-muted-foreground italic">Loading chart data...</span>
			</div>
		{:else}
			<div class="h-75 p-4 md:h-125">
				<AreaChart
					data={chartSeries.flatMap((d) => d.data)}
					series={chartSeries}
					x="date"
					y="value"
					padding={{ left: 48, right: -8 }}
					yNice
					yDomain={null}
					tooltip={{ mode: "quadtree-x" }}
				>
					{#snippet children({ context })}
						<Svg>
							<Highlight points lines={{ class: "stroke-muted-foreground/70" }} />
							<Axis
								placement="left"
								grid
								rule
								format={(value) => {
									const formattedValue = Math.abs(value);

									if (isUnitBytes)
										return (value < 0 ? "-" : "") + formatBytes(formattedValue, true) + " " + getUnit(formattedValue, true);
									return value.toLocaleString();
								}}
								classes={{
									tickLabel: "text-xs stroke-0 text-muted-foreground",
									rule: "stroke-muted-foreground/20"
								}}
							/>
							<Axis
								placement="bottom"
								rule
								classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground", rule: "stroke-0" }}
							/>

							{#each chartDataArray(chartSeries) as seriesData}
								<LinearGradient
									vertical
									stops={[
										[0, seriesData.color],
										[1, "transparent"]
									]}
								>
									{#snippet children({ gradient })}
										<Area
											data={seriesData.data}
											fill={gradient}
											fillOpacity={0.5}
											line={{ stroke: seriesData.color, strokeWidth: 2 }}
										/>
									{/snippet}
								</LinearGradient>
							{/each}
						</Svg>

						<!-- Data Tooltip -->
						<Tooltip.Root
							anchor="bottom"
							contained="container"
							class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md"
						>
							{#snippet children({ payload })}
								<div class="flex flex-col gap-y-1">
									{#each [...payload].reverse() as item}
										{@const title =
											seriesTitles?.find((title: SeriesTypeTitles) => title.seriesType === item.rawSeriesData?.key)?.title ??
											item.rawSeriesData?.key}
										<div class="flex justify-between gap-x-8">
											<div class="flex items-center gap-x-2">
												<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
												<span class="text-muted-foreground text-left">{title}</span>
											</div>
											<span class="text-text text-right">
												{isUnitBytes ? formatBytes(item.value) + " " + getUnit(item.value, true) : item.value.toLocaleString()}
											</span>
										</div>
									{/each}
								</div>

								{#if payload.length > 1}
									{@const total = payload.reduce((sum, p) => sum + p.value, 0)}
									<Tooltip.Separator class="text-muted-foreground" />

									<div class="text-foreground flex items-center justify-between text-sm font-bold">
										<span class="text-text">Total</span>
										<span class="text-text tabular-nums">
											{isUnitBytes ? formatBytes(total) + " " + getUnit(total, true) : total.toLocaleString()}
										</span>
									</div>
								{/if}
							{/snippet}
						</Tooltip.Root>

						<!-- Date Tooltip on x-Axis -->
						<Tooltip.Root
							x="pointer"
							y={context.height + context.padding.bottom + 8}
							anchor="top"
							variant="none"
							class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md"
						>
							{#snippet children({ data })}
								<Tooltip.Item class="text-text text-xs">
									{DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
								</Tooltip.Item>
							{/snippet}
						</Tooltip.Root>
					{/snippet}
				</AreaChart>
			</div>
		{/if}
	</div>
</dialog>

<style>
	dialog::backdrop {
		background: rgba(0, 0, 0, 0.7);
		backdrop-filter: blur(8px);
	}

	@media (max-width: 767px) {
		dialog[open] {
			animation: slide-up 0.3s ease-out;
		}
	}

	@keyframes slide-up {
		from {
			transform: translateY(100%);
		}
		to {
			transform: translateY(0);
		}
	}
</style>
