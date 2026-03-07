<script module lang="ts">
	type DelayAnalysisSeries = {
		key: string;
		data: DelayAnalysisDataPoint[];
		color: string;
		label: string;
	};

	type DelayAnalysisDataPoint = {
		date: Date;
		value: number;
	};
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampTransportTypeMetricDataPoint,
		type MetricSeries,
		type TransportType
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import type { ClassValue } from "svelte/elements";
	import DelayAnalysisOptionsDialog, {
		type DelayMode,
		type ScheduleType,
		type TransportFilter,
		availableTransportTypeFilters
	} from "./DelayAnalysisOptionsDialog.svelte";
	import { DateTime } from "luxon";
	import MetricCardBase from "../../../MetricCardBase.svelte";
	import MetricCardTitle from "../../../MetricCardTitle.svelte";
	import Info from "@lucide/svelte/icons/info";
	import Settings from "@lucide/svelte/icons/settings";
	import DelayAnalysisInformationDialog from "./DelayAnalysisInformationDialog.svelte";
	import { Axis, ChartClipPath, defaultChartPadding, Highlight, LineChart, Spline, Svg, Tooltip } from "layerchart";
	import DateTooltip from "../../../charts/tooltips/DateTooltip.svelte";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	// dialog states
	let optionsDialogVisible: boolean = $state(false);
	let infoDialogVisible: boolean = $state(false);

	// option states
	let selectedTransportTypes: TransportType[] = $state([]);
	let showTotalSeries: boolean = $state(true);
	let scheduleType: ScheduleType = $state("both" as ScheduleType);
	let delayMode: DelayMode = $state("avg" as DelayMode);
	let handleCancelledAsDelayed: boolean = $state(false);
	let delayThreshold: number = $state(359);

	const colorScale = scaleOrdinal(schemeTableau10);

	const buildSeriesDefinitions = (): Pick<DelayAnalysisSeries, "key" | "label">[] => {
		const directions: string[] = [];
		switch (scheduleType) {
			case "arrivals":
				directions.push("Arrival");
				break;
			case "departures":
				directions.push("Departure");
				break;
			case "both":
				directions.push("Arrival");
				directions.push("Departure");
				break;
			default:
				break;
		}

		const groups: { seriesKey: string; seriesLabel: string }[] = selectedTransportTypes.map((transportType: TransportType) => ({
			seriesKey: transportType,
			seriesLabel:
				availableTransportTypeFilters.find(
					(transportFilter: TransportFilter) => transportFilter.transportType === transportType
				)?.label ?? transportType
		}));
		if (showTotalSeries) groups.push({ seriesKey: "TOTAL", seriesLabel: "Total" });

		return groups.flatMap(({ seriesKey, seriesLabel }) =>
			directions.map((direction) => ({
				key: `${seriesKey}#${direction}`,
				label: `${seriesLabel} (${direction})`
			}))
		);
	};

	const getAvailableTransportTypes = (metrics: MetricSeries[]): TransportType[] => [
		...new Set(
			metrics
				.flatMap((metric: MetricSeries) => metric.dataPoints)
				.map(
					(dataPoint: BaseMetricDataPoint) =>
						(dataPoint as BaseMetricDataPointTimestampTransportTypeMetricDataPoint).transportType
				)
				.filter(Boolean) as TransportType[]
		)
	];

	const buildDelayChartSeries = (metrics: MetricSeries[]): DelayAnalysisSeries[] => {
		// lookup: MetricSeriesType -> timestamp -> TransportType -> value
		type DataLookup = Record<string, Record<string, Record<string, number>>>;
		const lookup: DataLookup = {};

		const relevantTypes = [
			MetricSeriesType.GLOBAL_STOP_ARRIVALS,
			MetricSeriesType.GLOBAL_STOP_DEPARTURES,
			MetricSeriesType.GLOBAL_STOP_ARRIVAL_CANCELLATIONS,
			MetricSeriesType.GLOBAL_STOP_DEPARTURE_CANCELLATIONS,
			MetricSeriesType.GLOBAL_STOP_ARRIVAL_DELAY_SUM,
			MetricSeriesType.GLOBAL_STOP_DEPARTURE_DELAY_SUM
		];

		metrics.forEach((metric: MetricSeries) => {
			if (!relevantTypes.includes(metric.seriesType)) return;

			metric.dataPoints.forEach((baseDataPoint: BaseMetricDataPoint) => {
				const dataPoint = baseDataPoint as BaseMetricDataPointTimestampTransportTypeMetricDataPoint;
				if (!dataPoint.timestamp || !dataPoint.transportType) return;

				lookup[metric.seriesType] ??= {};
				lookup[metric.seriesType][dataPoint.timestamp] ??= {};
				lookup[metric.seriesType][dataPoint.timestamp][dataPoint.transportType] = Number(dataPoint.value);
			});
		});

		const sortedTimestamps = [
			...new Set(
				Object.values(lookup)
					.flatMap((timestampMap) => Object.keys(timestampMap))
					.filter((timestamp: string, index: number, self: string[]) => self.indexOf(timestamp) === index)
					.sort((a: string, b: string) => DateTime.fromISO(a).toMillis() - DateTime.fromISO(b).toMillis())
			)
		];

		const availableTransportTypes = getAvailableTransportTypes(metrics);
		return buildSeriesDefinitions().map(({ key, label }) => {
			const [seriesType, direction] = key.split("#");

			const isArrival = direction === "Arrival";
			const isTotal = seriesType === "TOTAL";

			const metricSeriesPerDirection = isArrival
				? {
						delayType: MetricSeriesType.GLOBAL_STOP_ARRIVAL_DELAY_SUM,
						countType: MetricSeriesType.GLOBAL_STOP_ARRIVALS,
						cancelType: MetricSeriesType.GLOBAL_STOP_ARRIVAL_CANCELLATIONS
					}
				: {
						delayType: MetricSeriesType.GLOBAL_STOP_DEPARTURE_DELAY_SUM,
						countType: MetricSeriesType.GLOBAL_STOP_DEPARTURES,
						cancelType: MetricSeriesType.GLOBAL_STOP_DEPARTURE_CANCELLATIONS
					};

			const dataPoints: DelayAnalysisDataPoint[] = sortedTimestamps.map((timestamp: string) => {
				let delaySum: number = 0;
				let stopCount: number = 0;
				let cancellationCount: number = 0;

				if (isTotal)
					availableTransportTypes.forEach((transportType: TransportType) => {
						delaySum += lookup[metricSeriesPerDirection.delayType]?.[timestamp]?.[transportType] ?? 0;
						stopCount += lookup[metricSeriesPerDirection.countType]?.[timestamp]?.[transportType] ?? 0;
						cancellationCount += lookup[metricSeriesPerDirection.cancelType]?.[timestamp]?.[transportType] ?? 0;
					});
				else {
					delaySum = lookup[metricSeriesPerDirection.delayType]?.[timestamp]?.[seriesType!] ?? 0;
					stopCount = lookup[metricSeriesPerDirection.countType]?.[timestamp]?.[seriesType!] ?? 0;
					cancellationCount = lookup[metricSeriesPerDirection.cancelType]?.[timestamp]?.[seriesType!] ?? 0;
				}

				const adjustedDelaySum = handleCancelledAsDelayed ? delaySum + cancellationCount * delayThreshold : delaySum;
				const delayValue = delayMode === "avg" ? (stopCount > 0 ? adjustedDelaySum / stopCount : 0) : adjustedDelaySum;

				return { date: DateTime.fromISO(timestamp).toJSDate(), value: delayValue };
			});

			return { key, label, data: dataPoints, color: colorScale(key) };
		});
	};
</script>

<MetricCardBase class={["gap-y-4", className]}>
	{#snippet head()}
		<MetricCardTitle title="Stop Delay Analysis" class="justify-between">
			<div class="flex items-center gap-x-3">
				<!-- Info Button -->
				<button
					class={[
						"cursor-pointer transition-colors",
						infoDialogVisible && "text-accent",
						!infoDialogVisible && "text-muted-foreground hover:text-accent"
					]}
					onclick={(event: MouseEvent) => {
						event.stopPropagation();
						infoDialogVisible = !infoDialogVisible;
					}}
				>
					<Info size={18} />
				</button>

				<!-- Settings Button -->
				<button
					class={[
						"cursor-pointer transition-colors",
						optionsDialogVisible && "text-accent",
						!optionsDialogVisible && "text-muted-foreground hover:text-accent"
					]}
					onclick={(event: MouseEvent) => {
						event.stopPropagation();
						optionsDialogVisible = !optionsDialogVisible;
					}}
				>
					<Settings size={18} />
				</button>
			</div>

			<DelayAnalysisInformationDialog bind:isVisible={infoDialogVisible} />
		</MetricCardTitle>
	{/snippet}

	{#snippet body()}
		{#await promise}
			<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
		{:then metrics}
			{@const series = buildDelayChartSeries(metrics)}
			{@const availableTransportTypes = getAvailableTransportTypes(metrics)}

			<LineChart
				data={series.flatMap((singleSeries: DelayAnalysisSeries) => singleSeries.data)}
				{series}
				x="date"
				y="value"
				yDomain={null}
				brush
				height={384}
				padding={defaultChartPadding({ left: 48 })}
			>
				{#snippet children({ context, visibleSeries, getSplineProps, getHighlightProps })}
					<Svg>
						<Axis
							placement="left"
							rule
							grid
							tickLabelProps={{
								textAnchor: "start",
								dx: 8
							}}
							classes={{
								tickLabel: "text-xs stroke-0 text-muted-foreground select-none",
								label: "text-xs stroke-0 text-muted-foreground select-none"
							}}
							label="Delay (s)"
						/>
						<Axis
							placement="bottom"
							rule
							grid
							classes={{
								tickLabel: "text-xs stroke-0 text-muted-foreground select-none max-sm:hidden"
							}}
						/>

						<ChartClipPath>
							{#each visibleSeries as series, i (series.key)}
								<Spline {...getSplineProps(series, i)} stroke={series.color} strokeWidth={2} />
								<Highlight
									{...getHighlightProps(series, i)}
									points={{ stroke: series.color }}
									lines={{ class: "stroke-muted-foreground/30" }}
								/>
							{/each}
						</ChartClipPath>
					</Svg>

					<!-- Data Tooltip -->
					<Tooltip.Root
						anchor="bottom"
						contained="container"
						class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md select-none"
					>
						{#snippet children({ payload })}
							<div class="flex flex-col gap-y-1">
								{#each [...payload].reverse() as item}
									<div class="flex items-center justify-between gap-x-4 text-xs">
										<div class="flex items-center gap-x-2">
											<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
											<span class="text-muted-foreground text-left">{item.rawSeriesData?.label}</span>
										</div>

										<span class="text-text">
											{item.value.toLocaleString(undefined, { maximumFractionDigits: 1 })}
										</span>
									</div>
								{/each}
							</div>
						{/snippet}
					</Tooltip.Root>

					<!-- Date Tooltip on x-Axis -->
					<DateTooltip
						{context}
						value={(data: { date: Date; value: number }) =>
							DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
					/>
				{/snippet}
			</LineChart>

			<DelayAnalysisOptionsDialog
				bind:isVisible={optionsDialogVisible}
				{availableTransportTypes}
				bind:selectedTransportTypes
				bind:showTotalSeries
				bind:scheduleType
				bind:delayMode
				bind:handleCancelledAsDelayed
				bind:delayThreshold
			/>
		{/await}
	{/snippet}
</MetricCardBase>
