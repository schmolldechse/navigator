<script module lang="ts">
	import type {
		AdministrationMetricSubject,
		BaseMetricDataPointLineRankingDataPoint,
		LineMetricSubject,
		MetricSample,
		MetricSeries,
		StationMetricSubject
	} from "@lib/api";

	type LineRankingRow = {
		key: string;
		rank: number;
		line: LineMetricSubject;
		administration: AdministrationMetricSubject;
		startStation: StationMetricSubject;
		endStation: StationMetricSubject;
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

	export type { LineRankingRow, NormalizedPage };
</script>

<script lang="ts">
	import type { ClassValue } from "svelte/elements";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import Info from "@lucide/svelte/icons/info";
	import Trophy from "@lucide/svelte/icons/trophy";
	import Route from "@lucide/svelte/icons/route";
	import * as Tooltip from "@lib/components/ui/tooltip/index";
	import { getLineRankingMetricOption } from "./line-ranking";
	import { formatMetricValue } from "../../metric-format";

	type Props = {
		metric: MetricSeries;
		page: NormalizedPage;
		class?: ClassValue;
	};
	let { metric, page, class: className }: Props = $props();

	const getStationLabel = (station: StationMetricSubject) => station.name ?? `Station ${station.evaNumber}`;

	const formatTransportType = (value: string) =>
		value
			.toLowerCase()
			.split("_")
			.map((part: string) => part.charAt(0).toUpperCase() + part.slice(1))
			.join(" ");

	const createRows = (metric: MetricSeries, page: NormalizedPage): LineRankingRow[] => {
		const rows: LineRankingRow[] = [];

		for (const baseDataPoint of metric.dataPoints) {
			if (!("line" in baseDataPoint) || !baseDataPoint.line) continue;

			const dataPoint = baseDataPoint as BaseMetricDataPointLineRankingDataPoint;

			const index = rows.length;
			const rank = page.offset + index + 1;
			const line = dataPoint.line;
			const administration = dataPoint.administration;
			const startStation = dataPoint.startStation;
			const endStation = dataPoint.endStation;

			rows.push({
				key: `${rank}-${line.number}-${line.journeyDescription}-${line.transportType}-${administration.operatorCode}-${administration.administrationId}-${startStation.evaNumber}-${endStation.evaNumber}`,
				rank,
				line,
				administration,
				startStation,
				endStation,
				value: Number(dataPoint.value),
				unit: metric.unit,
				sample: dataPoint.sample
			});
		}

		return rows;
	};

	let rows = $derived(createRows(metric, page));
	let maxValue = $derived(Math.max(0, ...rows.map((row: LineRankingRow) => row.value)));
	let metricOption = $derived(getLineRankingMetricOption(metric.seriesType));
</script>

<div class={["flex flex-col gap-y-2", className]}>
	{#if rows.length === 0}
		<div class="bg-secondary/30 border-border flex min-h-96 flex-col items-center justify-center rounded-lg border text-center">
			<p class="text-foreground font-semibold">No line ranking data available</p>
			<p class="text-foreground/60 max-w-sm text-sm">Try a wider time range or another ranking metric.</p>
		</div>
	{:else}
		<div class="flex flex-col gap-y-2">
			<div
				class={[
					"text-foreground/45 gap-3 px-3 text-xs font-semibold tracking-wide uppercase",
					"hidden grid-cols-[4rem_minmax(0,1fr)_minmax(6rem,8rem)] md:grid"
				]}
			>
				<span class="text-right">Place</span>
				<span>Line Information</span>
				<span class="text-right">{metricOption?.valueLabel ?? "Value"}</span>
			</div>

			{#each rows as row (row.key)}
				{@const percentage = maxValue === 0 ? 0 : (Math.max(0, row.value) / maxValue) * 100}
				{@const { value, unit } = formatMetricValue(row.value, row.unit)}

				<article
					class={[
						"border-border bg-secondary/20 hover:bg-secondary/35 gap-x-4 gap-y-3 rounded-lg border p-4 transition-colors md:gap-x-3 md:gap-y-2 md:p-3",
						"grid grid-cols-[minmax(0,1fr)_auto] md:grid-cols-[4rem_minmax(0,1fr)_minmax(6rem,8rem)] md:grid-rows-[auto_auto_auto]"
					]}
				>
					<!-- Place -->
					<div class={["flex items-center gap-x-2 md:justify-end md:self-center", "md:row-span-3"]}>
						{#if row.rank <= 3 && metricOption?.polarity === "positive"}
							<Trophy size={16} class="text-accent" />
						{:else if row.rank <= 3 && metricOption?.polarity === "negative"}
							<CircleAlert size={16} class="text-destructive" />
						{/if}

						<span
							class="bg-background border-border text-foreground rounded-md border px-2 py-1 text-sm font-bold tabular-nums"
						>
							#{row.rank}
						</span>
					</div>

					<!-- Line Information -->
					<div class={["flex flex-col gap-y-1", "col-span-2 md:col-span-1 md:col-start-2 md:row-start-1 md:self-center"]}>
						<div class="flex flex-wrap items-center gap-x-2 gap-y-1">
							<Route size={15} class="text-accent shrink-0" />

							<span class="text-foreground min-w-0 font-semibold break-words">{row.line.journeyDescription}</span>
							<span class="bg-background text-foreground/60 rounded-md px-1.5 py-0.5 text-xs font-medium">
								{row.line.number}
							</span>
							<span class="bg-background text-foreground/60 rounded-md px-1.5 py-0.5 text-xs font-medium">
								{formatTransportType(row.line.transportType)}
							</span>
						</div>

						<div class="text-foreground/55 flex flex-wrap items-baseline gap-x-2 gap-y-1 text-xs">
							<span class="truncate">{row.administration.operatorName}</span>
							<span class="bg-background text-foreground/60 rounded-md px-1.5 py-0.5 text-xs font-medium">
								{row.administration.operatorCode}
							</span>
						</div>
					</div>

					<!-- Route -->
					<div
						class={[
							"text-foreground/65 gap-x-2 gap-y-1 text-sm",
							"col-span-2 grid grid-cols-[0.75rem_minmax(0,1fr)] md:col-span-1 md:col-start-2 md:row-start-2"
						]}
					>
						<div class="relative row-span-2 flex justify-center" aria-hidden="true">
							<span class="bg-accent absolute top-1.5 size-1.5 rounded-full"></span>
							<span class="bg-foreground/20 absolute top-3 bottom-3 w-px"></span>
							<span class="bg-foreground/35 absolute bottom-1.5 size-1.5 rounded-full"></span>
						</div>

						<span class="min-w-0 truncate">{getStationLabel(row.startStation)}</span>
						<span class="text-foreground/55 min-w-0 truncate">{getStationLabel(row.endStation)}</span>
					</div>

					<!-- Value -->
					<div
						class={[
							"flex items-center justify-end gap-x-2 md:self-center",
							"col-start-2 row-start-1 md:col-start-3 md:row-span-3 md:row-start-1"
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

					<!-- Value Bar -->
					<div
						class={["bg-background h-2 overflow-hidden rounded-full", "col-span-2 md:col-span-1 md:col-start-2 md:row-start-3"]}
						role="img"
						aria-label={`${row.line.journeyDescription} from ${getStationLabel(row.startStation)} to ${getStationLabel(row.endStation)} relative value ${percentage.toLocaleString(undefined, { maximumFractionDigits: 0 })}%`}
					>
						<div class="bg-accent h-full rounded-full transition-[width] duration-300" style:width={`${percentage}%`}></div>
					</div>
				</article>
			{/each}
		</div>
	{/if}
</div>
