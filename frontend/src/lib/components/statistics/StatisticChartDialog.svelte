<script lang="ts">
	import { formatBytes, getUnit } from "$lib/util/bytes";
	import X from "@lucide/svelte/icons/x";
	import { Svg, Axis, Spline, Highlight, LineChart, Tooltip } from "layerchart";
	import type { MeasuredStatisticValue, MeasuredTimerangeStatistic } from "$lib/api";
	import { DateTime } from "luxon";

	interface Props {
		isVisible: boolean;
		title: string;
		measuredStatistic: MeasuredTimerangeStatistic;
		onclose?: () => void;
	}

	let { isVisible = $bindable(), title, measuredStatistic, onclose }: Props = $props();

	let isCumulative: boolean = $state(true);

	const chartData = $derived.by(() => {
		if (!measuredStatistic || !measuredStatistic.values) return [];

		const rawValues = measuredStatistic.values.map((statisticValue: MeasuredStatisticValue) => ({
			date: DateTime.fromISO(statisticValue.date).toJSDate(),
			value: Number(statisticValue.value)
		}));
		if (isCumulative) return rawValues;

		return rawValues.map((currentItem, index) => {
			if (index === 0) return { ...currentItem, value: 0 };

			const difference = currentItem.value - rawValues[index - 1].value;
			return { ...currentItem, value: Math.max(0, difference) };
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

		{#if !measuredStatistic || chartData.length === 0}
			<div class="bg-muted/30 flex h-75 animate-pulse items-center justify-center rounded-xl md:h-125">
				<span class="text-muted-foreground italic">Loading chart data...</span>
			</div>
		{:else}
			<div class="h-75 p-4 md:h-125">
				<LineChart data={chartData} x="date" y="value" yNice yDomain={null} padding={{ left: 16, bottom: 24, right: -16 }}>
					<Svg>
						<Axis placement="left" grid rule format={(value) => String(formatBytes(value, true))} class="text-xs" />
						<Axis placement="bottom" rule class="text-xs" />
						<Spline class="stroke-accent stroke-3" />
						<Highlight
							points={{ r: 5, class: "fill-accent stroke-background stroke-2" }}
							lines={{ class: "stroke-accent/50" }}
						/>
					</Svg>

					<Tooltip.Root class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md">
						{#snippet children({ data })}
							{@const isUnitBytes = measuredStatistic.unit === "BYTES"}

							<Tooltip.Header class="text-muted-foreground! mb-1 text-xs font-medium"
								>{DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}</Tooltip.Header
							>
							<Tooltip.List>
								<div class="flex flex-row items-center gap-x-2">
									<div class="bg-accent h-2 w-2 rounded-full"></div>
									<Tooltip.Item
										class="text-text! text-sm font-bold"
										value={isUnitBytes ? formatBytes(data.value, true) + " " + getUnit(data.value, true) : data.value}
									/>
								</div>
							</Tooltip.List>
						{/snippet}
					</Tooltip.Root>
				</LineChart>
			</div>
		{/if}
	</div>
</dialog>

<style>
	/** axis-line */
	:global(g > g > line) {
		stroke: var(--color-muted-foreground) !important;
	}

	/** left-axis tick */
	:global(g > svg:last-child > text > tspan) {
		stroke-width: 0 !important;
		fill: var(--color-text) !important;
	}

	/** bottom-axis tick */
	:global(g:nth-child(2) svg > text > tspan) {
		stroke-width: 0 !important;
		fill: var(--color-text) !important;
	}

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
