<script module lang="ts">
	import type { AdministrationMetricSubject, MetricSample, MetricSeries } from "@lib/api";

	type AdministrationRankingRow = {
		key: string;
		rank: number;
		administration: AdministrationMetricSubject;
		value: number;
		unit: MetricSeries["unit"];
		sample?: MetricSample | null;
	};

	type NormalizedPage = {
		offset: number;
		limit: number;
		totalItems: number;
		totalPages: number;
		hasMore: boolean;
	};

	export type { AdministrationRankingRow, NormalizedPage };
</script>

<script lang="ts">
	import type { ClassValue } from "svelte/elements";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import Info from "@lucide/svelte/icons/info";
	import Trophy from "@lucide/svelte/icons/trophy";
	import * as Tooltip from "@lib/components/ui/tooltip/index";
	import { getAdministrationRankingMetricOption } from "./administration-ranking";
	import { formatMetricValue } from "../../metric-format";

	type Props = {
		metric: MetricSeries;
		page: NormalizedPage;
		stationScoped?: boolean;
		class?: ClassValue;
	};
	let { metric, page, stationScoped = false, class: className }: Props = $props();

	const createRows = (metric: MetricSeries, page: NormalizedPage): AdministrationRankingRow[] => {
		const rows: AdministrationRankingRow[] = [];

		for (const dataPoint of metric.dataPoints) {
			if (!("administration" in dataPoint) || !dataPoint.administration) continue;

			const index = rows.length;
			const administration = dataPoint.administration;
			const rank = page.offset + index + 1;

			rows.push({
				key: `${rank}-${administration.operatorCode}-${administration.administrationId}-${administration.operatorName}`,
				rank,
				administration,
				value: Number(dataPoint.value),
				unit: metric.unit,
				sample: dataPoint.sample
			});
		}

		return rows;
	};

	let rows = $derived(createRows(metric, page));
	let maxValue = $derived(Math.max(0, ...rows.map((row: AdministrationRankingRow) => row.value)));
	let metricOption = $derived(getAdministrationRankingMetricOption(metric.seriesType));
	let valueLabel = $derived(
		stationScoped && metricOption?.valueLabel === "Journeys" ? "Station events" : (metricOption?.valueLabel ?? "Value")
	);
</script>

<div class={["flex flex-col gap-y-2", className]}>
	{#if rows.length === 0}
		<div class="bg-secondary/30 border-border flex min-h-96 flex-col items-center justify-center rounded-lg border text-center">
			<p class="text-foreground font-semibold">No ranking data available</p>
			<p class="text-foreground/60 max-w-sm text-sm">Try a wider time range or another ranking metric.</p>
		</div>
	{:else}
		<div
			class={[
				"text-foreground/45 gap-3 px-3 text-xs font-semibold tracking-wide uppercase",
				"hidden grid-cols-[4rem_minmax(0,1fr)_minmax(6rem,8rem)] sm:grid"
			]}
		>
			<span class="text-right">Place</span>
			<span>Administration / Operator</span>
			<span class="text-right">{valueLabel}</span>
		</div>

		{#each rows as row (row.key)}
			{@const percentage = maxValue === 0 ? 0 : (Math.max(0, row.value) / maxValue) * 100}
			{@const { value, unit } = formatMetricValue(row.value, row.unit)}

			<article
				class={[
					"border-border bg-secondary/20 hover:bg-secondary/35 gap-x-4 gap-y-3 rounded-lg border p-4 transition-colors sm:gap-x-3 sm:gap-y-2 sm:p-3",
					"grid grid-cols-[minmax(0,1fr)_auto] sm:grid-cols-[4rem_minmax(0,1fr)_minmax(6rem,8rem)] sm:grid-rows-[auto_auto]"
				]}
			>
				<div class={["flex items-center gap-x-2 sm:justify-end sm:self-center", "sm:row-span-2"]}>
					{#if row.rank <= 3 && metricOption?.polarity === "positive"}
						<Trophy size={16} class="text-accent" />
					{:else if row.rank <= 3 && metricOption?.polarity === "negative"}
						<CircleAlert size={16} class="text-destructive" />
					{/if}

					<span class="bg-background border-border text-foreground rounded-md border px-2 py-1 text-sm font-bold tabular-nums">
						#{row.rank}
					</span>
				</div>

				<div
					class={[
						"flex flex-row flex-wrap items-baseline gap-x-2 gap-y-1",
						"col-span-2 sm:col-span-1 sm:col-start-2 sm:row-start-1"
					]}
				>
					<span class="text-foreground max-w-full min-w-0 font-semibold break-words">
						{row.administration.operatorName}
					</span>
					<span class="bg-background text-foreground/60 max-w-full rounded-md px-1.5 py-0.5 text-xs font-medium break-all">
						{row.administration.operatorCode}
					</span>
				</div>

				<div
					class={[
						"flex items-center justify-end gap-x-2 sm:self-center",
						"col-start-2 row-start-1 sm:col-start-3 sm:row-span-2 sm:row-start-1"
					]}
				>
					<div class="text-foreground text-right font-bold whitespace-nowrap tabular-nums">
						{value}
						{#if unit}
							<span class="text-foreground/60 ml-1 text-xs font-semibold">{unit}</span>
						{/if}
					</div>

					{#if row.sample && metricOption?.sampleLabels}
						<Tooltip.Root delay={100}>
							<Tooltip.Trigger>
								<button
									type="button"
									class="text-foreground/45 hover:bg-accent/15 hover:text-accent focus-visible:outline-accent flex size-7 shrink-0 items-center justify-center rounded-lg transition-colors focus-visible:outline-2 focus-visible:outline-offset-2"
									aria-label={`Show sample for ${row.administration.operatorName}`}
								>
									<Info size={15} />
								</button>
							</Tooltip.Trigger>

							<Tooltip.Content position="left" class="w-64">
								<div class="grid gap-y-1">
									<p class="text-foreground text-sm font-semibold">Sample basis</p>
									<p class="text-foreground/60 text-xs leading-relaxed">
										{metricOption.sampleLabels.numerator}: {Number(row.sample.numerator).toLocaleString()}.
										{metricOption.sampleLabels.denominator}: {Number(row.sample.denominator).toLocaleString()}.
									</p>
								</div>
							</Tooltip.Content>
						</Tooltip.Root>
					{/if}
				</div>

				<div
					class={["bg-background h-2 overflow-hidden rounded-full", "col-span-2 sm:col-span-1 sm:col-start-2 sm:row-start-2"]}
					role="img"
					aria-label={`${row.administration.operatorName} relative value ${percentage.toLocaleString(undefined, { maximumFractionDigits: 0 })}%`}
				>
					<div class="bg-accent h-full rounded-full transition-[width] duration-300" style:width={`${percentage}%`}></div>
				</div>
			</article>
		{/each}
	{/if}
</div>
