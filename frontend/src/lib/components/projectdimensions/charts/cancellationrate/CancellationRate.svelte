<script module lang="ts">
	type CancellationRateSeries = {
		key: string;
		data: CancellationRateDataPoint[];
		color: string;
		label: string;
	};

	type CancellationRateDataPoint = {
		date: Date;
		value: number;
	};
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampTransportTypeDataPoint,
		type MetricSeries,
		type TransportType
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import type { ClassValue } from "svelte/elements";
	import CancellationRateOptionsDialog, {
		availableTransportFilters as availableCancellationRateTransportFilters,
		type RateMode,
		type ScheduleType,
		type CancellationRateTransportFilter
	} from "./CancellationRateOptionsDialog.svelte";
	import { DateTime } from "luxon";
	import Settings from "@lucide/svelte/icons/settings";
	import { Axis, ChartClipPath, defaultChartPadding, Highlight, LineChart, Spline, Svg, Tooltip } from "layerchart";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import DateTooltip from "@lib/components/layerchart/tooltips/DateTooltip.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	// dialog states
	let optionsDialogVisible: boolean = $state(false);

	// option states
	let selectedTransportTypes: TransportType[] = $state([]);
	let showTotalSeries: boolean = $state(true);
	let scheduleType: ScheduleType = $state("both" as ScheduleType);
	let rateMode: RateMode = $state("relative" as RateMode);

	const colorScale = scaleOrdinal(schemeTableau10);

	const buildSeriesDefinitions = (): Pick<CancellationRateSeries, "key" | "label">[] => {
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
				availableCancellationRateTransportFilters.find(
					(transportFilter: CancellationRateTransportFilter) => transportFilter.transportType === transportType
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
					(dataPoint: BaseMetricDataPoint) => (dataPoint as BaseMetricDataPointTimestampTransportTypeDataPoint).transportType
				)
				.filter(Boolean) as TransportType[]
		)
	];

	const buildCancellationRateChartSeries = (metrics: MetricSeries[]): CancellationRateSeries[] => {
		// lookup: MetricSeriesType -> timestamp -> TransportType -> value
		type DataLookup = Record<string, Record<string, Record<string, number>>>;
		const lookup: DataLookup = {};

		const relevantTypes = [
			MetricSeriesType.HOURLY_GLOBAL_ARRIVALS,
			MetricSeriesType.HOURLY_GLOBAL_DEPARTURES,
			MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_CANCELLATIONS,
			MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_CANCELLATIONS
		];

		metrics.forEach((metric: MetricSeries) => {
			if (!relevantTypes.includes(metric.seriesType)) return;

			metric.dataPoints.forEach((baseDataPoint: BaseMetricDataPoint) => {
				const dataPoint = baseDataPoint as BaseMetricDataPointTimestampTransportTypeDataPoint;
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
						countType: MetricSeriesType.HOURLY_GLOBAL_ARRIVALS,
						cancelType: MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_CANCELLATIONS
					}
				: {
						countType: MetricSeriesType.HOURLY_GLOBAL_DEPARTURES,
						cancelType: MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_CANCELLATIONS
					};

			const dataPoints: CancellationRateDataPoint[] = sortedTimestamps.map((timestamp: string) => {
				let cancellationCount: number = 0;
				let stopCount: number = 0;

				if (isTotal)
					availableTransportTypes.forEach((transportType: TransportType) => {
						stopCount += lookup[metricSeriesPerDirection.countType]?.[timestamp]?.[transportType] ?? 0;
						cancellationCount += lookup[metricSeriesPerDirection.cancelType]?.[timestamp]?.[transportType] ?? 0;
					});
				else {
					stopCount = lookup[metricSeriesPerDirection.countType]?.[timestamp]?.[seriesType!] ?? 0;
					cancellationCount = lookup[metricSeriesPerDirection.cancelType]?.[timestamp]?.[seriesType!] ?? 0;
				}

				let cancellationRate: number = 0;
				switch (rateMode) {
					case "relative":
						cancellationRate = stopCount > 0 ? (cancellationCount / stopCount) * 100 : 0;
						break;
					case "absolute":
						cancellationRate = cancellationCount;
						break;
					default:
						break;
				}

				return { date: DateTime.fromISO(timestamp).toJSDate(), value: cancellationRate };
			});

			return { key, label, data: dataPoints, color: colorScale(key) };
		});
	};
</script>

<MetricCardBase class={["gap-y-4", className]}>
	{#snippet head()}
		<MetricCardTitle title="Stop Cancellation Rate" class="justify-between">
			<div class="flex items-center gap-x-3">
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
		</MetricCardTitle>
	{/snippet}

	{#snippet body()}
		{#await promise}
			<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
		{:then metrics}
			{@const series = buildCancellationRateChartSeries(metrics)}
			{@const availableTransportTypes = getAvailableTransportTypes(metrics)}

			<LineChart
				data={series.flatMap((singleSeries: CancellationRateSeries) => singleSeries.data)}
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
							label={rateMode === "relative" ? "Rate (%)" : "Cancellations"}
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
											{item.value.toLocaleString(undefined, { maximumFractionDigits: 1 })}{rateMode === "relative" ? "%" : ""}
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

			<CancellationRateOptionsDialog
				bind:isVisible={optionsDialogVisible}
				{availableTransportTypes}
				bind:selectedTransportTypes
				bind:showTotalSeries
				bind:scheduleType
				bind:rateMode
			/>
		{:catch}
			<MetricLoadingFailedWarning />
		{/await}
	{/snippet}
</MetricCardBase>
