<script lang="ts" module>
	const hoursArray: DateTime[] = Array.from({ length: 24 }, (_, index: number) =>
		DateTime.local().startOf("day").plus({ hour: index })
	);

	// 24H Stop Histogram
	type HourlyStopSeries = {
		key: MetricSeriesType.GLOBAL_STOP_ARRIVALS | MetricSeriesType.GLOBAL_STOP_DEPARTURES;
		color: string;
	};

	type HourlyStopDataPoint = {
		date: Date;
		[MetricSeriesType.GLOBAL_STOP_ARRIVALS]: number;
		[MetricSeriesType.GLOBAL_STOP_DEPARTURES]: number;
	};

	// Delay Analysis LineChart
	interface DelaySeries {
		key: string;
		data: { date: Date; value: number }[];
		color: string;
	}

	// Key Titles
	interface KeyTitles {
		key: string;
		title: string;
	}

	// Static key titles for the bar chart
	const barChartKeyTitles: KeyTitles[] = [
		{ key: MetricSeriesType.GLOBAL_STOP_ARRIVALS, title: "Arrival stops" },
		{ key: MetricSeriesType.GLOBAL_STOP_DEPARTURES, title: "Departure stops" }
	];
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		TransportType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampTransportTypeMetricDataPoint,
		type MetricSeries
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { onMount } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import MetricCardBase from "../../MetricCardBase.svelte";
	import MetricCardTitle from "../../MetricCardTitle.svelte";
	import MetricLoadingFailedWarning from "../../MetricLoadingFailedWarning.svelte";
	import { DateTime } from "luxon";
	import {
		Axis,
		BarChart,
		Bars,
		ChartClipPath,
		defaultChartPadding,
		Highlight,
		LineChart,
		Spline,
		Svg,
		Tooltip
	} from "layerchart";
	import DateTooltip from "../../charts/tooltips/DateTooltip.svelte";
	import DelayChartOptionsDialog, {
		type ScheduleType,
		type DelayMode,
		DelayAnalysisSeriesType,
		delayAnalysisKeyTitles
	} from "./DelayAnalysisOptionsDialog.svelte";
	import DelayAnalysisInformationDialog from "./DelayAnalysisInformationDialog.svelte";
	import Settings from "@lucide/svelte/icons/settings";
	import Info from "@lucide/svelte/icons/info";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	// dialog visible states
	let delayOptionsVisible: boolean = $state(false);
	let delayInfoVisible: boolean = $state(false);

	// delay analysis options states
	let scheduleType: ScheduleType = $state("both");
	let delayMode: DelayMode = $state("avg");
	let selectedSeries: DelayAnalysisSeriesType[] = $state([
		DelayAnalysisSeriesType.TOTAL_ARRIVAL,
		DelayAnalysisSeriesType.TOTAL_DEPARTURE
	]);
	let handleCancelledAsDelayed: boolean = $state(false);
	let delayThreshold: number = $state(359);

	onMount(async () => {
		const metrics = await promise;

		const validTypes = [
			MetricSeriesType.GLOBAL_STOP_ARRIVALS,
			MetricSeriesType.GLOBAL_STOP_ARRIVAL_CANCELLATIONS,
			MetricSeriesType.GLOBAL_STOP_ARRIVAL_DELAY_SUM,
			MetricSeriesType.GLOBAL_STOP_DEPARTURES,
			MetricSeriesType.GLOBAL_STOP_DEPARTURE_CANCELLATIONS,
			MetricSeriesType.GLOBAL_STOP_DEPARTURE_DELAY_SUM
		];

		if (metrics.some((metric: MetricSeries) => !validTypes.includes(metric.seriesType)))
			throw new Error("GlobalStopSummaryMetricCard received invalid metric series type.");
	});

	const colorScale = scaleOrdinal(schemeTableau10);

	const hourlyStopHistogram = (metrics: MetricSeries[]): { series: HourlyStopSeries[]; chartData: HourlyStopDataPoint[] } => {
		const targetTypes = [MetricSeriesType.GLOBAL_STOP_ARRIVALS, MetricSeriesType.GLOBAL_STOP_DEPARTURES];
		const targetMetrics: MetricSeries[] = metrics.filter((metric: MetricSeries) => targetTypes.includes(metric.seriesType));

		const series: HourlyStopSeries[] = targetMetrics.map((metric: MetricSeries) => ({
			key: metric.seriesType as MetricSeriesType.GLOBAL_STOP_ARRIVALS | MetricSeriesType.GLOBAL_STOP_DEPARTURES,
			color: colorScale(metric.seriesType)
		}));

		type GroupedSums = {
			[T in MetricSeriesType]?: Record<number, number>;
		};

		const groupedSums = targetMetrics.reduce<GroupedSums>((acc: GroupedSums, metric: MetricSeries) => {
			acc[metric.seriesType] ??= {};

			metric.dataPoints.forEach((baseDataPoint: BaseMetricDataPoint) => {
				const dataPointTimestamp = baseDataPoint as BaseMetricDataPointTimestampTransportTypeMetricDataPoint;
				if (dataPointTimestamp.timestamp === undefined) return;

				const hour = DateTime.fromISO(dataPointTimestamp.timestamp).hour;

				// initialized before so it's safe to use non-null assertion
				const currentSeries = acc[metric.seriesType]!;
				currentSeries[hour] = (currentSeries[hour] ?? 0) + Number(dataPointTimestamp.value);
			});
			return acc;
		}, {} as GroupedSums);

		const chartData: HourlyStopDataPoint[] = hoursArray.map((hour: DateTime) => {
			const hourNum = hour.hour;
			const point: HourlyStopDataPoint = {
				date: hour.toJSDate(),
				[MetricSeriesType.GLOBAL_STOP_ARRIVALS]: 0,
				[MetricSeriesType.GLOBAL_STOP_DEPARTURES]: 0
			};

			targetMetrics.forEach((metric: MetricSeries) => {
				const seriesType = metric.seriesType as MetricSeriesType.GLOBAL_STOP_ARRIVALS | MetricSeriesType.GLOBAL_STOP_DEPARTURES;
				point[seriesType] = groupedSums[seriesType]?.[hourNum] ?? 0;
			});

			return point;
		});

		return { series, chartData };
	};

	const getAvailableTransportTypes = (metrics: MetricSeries[]): TransportType[] => [
		...new Set(
			metrics
				.flatMap((metric: MetricSeries) => metric.dataPoints)
				.map((dataPoint) => (dataPoint as BaseMetricDataPointTimestampTransportTypeMetricDataPoint).transportType)
				.filter(Boolean) as TransportType[]
		)
	];

	// Build delay chart series from metrics
	const buildDelayChart = (metrics: MetricSeries[]): DelaySeries[] => {
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

		// determine which TransportType to include
		const availableTypes = getAvailableTransportTypes(metrics);

		// filter selected series by scheduleType
		const activeSeries = selectedSeries.filter((delayAnalysis: DelayAnalysisSeriesType) => {
			const isArrival = delayAnalysis.endsWith("_ARRIVAL");
			if (scheduleType === "arrivals") return isArrival;
			if (scheduleType === "departures") return !isArrival;
			return true;
		});

		const result: DelaySeries[] = activeSeries.map((delayAnalysis: DelayAnalysisSeriesType) => {
			if (!Object.values(DelayAnalysisSeriesType).includes(delayAnalysis))
				throw new Error(`Invalid series type selected: ${delayAnalysis}`);

			const isArrival = delayAnalysis.endsWith("_ARRIVAL");
			const isTotal =
				delayAnalysis === DelayAnalysisSeriesType.TOTAL_ARRIVAL || delayAnalysis === DelayAnalysisSeriesType.TOTAL_DEPARTURE;

			const direction = isArrival
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

			const specificTransportType = !isTotal ? delayAnalysis.replace(/_ARRIVAL$|_DEPARTURE$/, "") : null;

			const dataPoints: { date: Date; value: number }[] = sortedTimestamps.map((timestamp: string) => {
				let delaySum: number = 0;
				let stopCount: number = 0;
				let cancellationCount: number = 0;

				if (isTotal) {
					availableTypes.forEach((transportType: TransportType) => {
						delaySum += lookup[direction.delayType]?.[timestamp]?.[transportType] ?? 0;
						stopCount += lookup[direction.countType]?.[timestamp]?.[transportType] ?? 0;
						cancellationCount += lookup[direction.cancelType]?.[timestamp]?.[transportType] ?? 0;
					});
				} else {
					delaySum = lookup[direction.delayType]?.[timestamp]?.[specificTransportType!] ?? 0;
					stopCount = lookup[direction.countType]?.[timestamp]?.[specificTransportType!] ?? 0;
					cancellationCount = lookup[direction.cancelType]?.[timestamp]?.[specificTransportType!] ?? 0;
				}

				const adjustedDelaySum = handleCancelledAsDelayed ? delaySum + cancellationCount * delayThreshold : delaySum;
				const delayValue = delayMode === "avg" ? (stopCount > 0 ? adjustedDelaySum / stopCount : 0) : adjustedDelaySum;

				return { date: DateTime.fromISO(timestamp).toJSDate(), value: delayValue };
			});

			return {
				key: delayAnalysis,
				data: dataPoints,
				color: colorScale(delayAnalysis)
			};
		});
		return result;
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	{#snippet head()}
		{#await promise}
			<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
		{:then metrics}
			<!-- Hourly Stop Rate -->
			{@const { series, chartData } = hourlyStopHistogram(metrics)}

			<!-- Delay Analysis -->
			{@const availableTransportTypes = getAvailableTransportTypes(metrics)}
			{@const delaySeries = buildDelayChart(metrics)}

			<div class="flex flex-col gap-y-8 sm:gap-y-20">
				<!-- Hourly Stop Rate -->
				<div class="flex flex-col gap-y-6">
					<MetricCardTitle title="Hourly Stop Rate" />
					<BarChart
						data={chartData}
						{series}
						x="date"
						height={256}
						seriesLayout="stack"
						bandPadding={0.2}
						padding={defaultChartPadding({ left: 48 })}
					>
						{#snippet children({ visibleSeries, getBarsProps, getHighlightProps })}
							<Svg>
								<Axis
									placement="left"
									rule
									grid
									classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground select-none" }}
								/>
								<Axis
									placement="bottom"
									rule
									classes={{
										tickLabel: "text-xs stroke-0 text-muted-foreground select-none max-sm:hidden"
									}}
									tickLabelProps={{
										rotate: 315,
										textAnchor: "end"
									}}
								/>

								<ChartClipPath>
									{#each visibleSeries as series, i (series.key)}
										<Bars {...getBarsProps(series, i)} />
									{/each}
								</ChartClipPath>

								<ChartClipPath full>
									<Highlight {...getHighlightProps()} />
								</ChartClipPath>
							</Svg>

							<!-- Data Tooltip -->
							<Tooltip.Root
								anchor="bottom"
								contained="container"
								class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md select-none"
							>
								{#snippet children({ data, payload })}
									{@const total = payload.reduce((acc: number, current) => acc + current.value, 0)}

									<Tooltip.Header>
										{@const startDate = DateTime.fromJSDate(data.date)}
										{@const endDate = startDate.plus({ hour: 1 })}

										<span class="text-text text-xs">
											{startDate.toLocaleString(DateTime.TIME_SIMPLE)} – {endDate.toLocaleString(DateTime.TIME_SIMPLE)}
										</span>
									</Tooltip.Header>

									<div class="flex flex-col gap-y-1">
										{#each [...payload].reverse() as item}
											{@const title = barChartKeyTitles?.find((title: KeyTitles) => title.key === item.key)!.title}
											<div class="flex justify-between gap-x-4 text-xs">
												<div class="flex items-center gap-x-2">
													<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
													<span class="text-muted-foreground text-left">{title}</span>
												</div>

												<span class="text-text">{item.value.toLocaleString()}</span>
											</div>
										{/each}

										<Tooltip.Separator class="border-muted-foreground/20 my-1 border-t" />

										<div class="flex flex-row justify-between gap-x-4 text-xs">
											<span>Total</span>
											<span>{total.toLocaleString()}</span>
										</div>
									</div>
								{/snippet}
							</Tooltip.Root>
						{/snippet}
					</BarChart>
				</div>

				<!-- Delay Analysis -->
				<div class="flex flex-col gap-y-6">
					<MetricCardTitle title="Delay Analysis" class="justify-between">
						<div class="flex items-center gap-x-3">
							<!-- Info Button -->
							<button
								class={[
									"cursor-pointer transition-colors",
									delayInfoVisible && "text-accent",
									!delayInfoVisible && "text-muted-foreground hover:text-accent"
								]}
								onclick={(event: MouseEvent) => {
									event.stopPropagation();
									delayInfoVisible = !delayInfoVisible;
								}}
							>
								<Info size={18} />
							</button>

							<!-- Settings Button -->
							<button
								class={[
									"cursor-pointer transition-colors",
									delayOptionsVisible && "text-accent",
									!delayOptionsVisible && "text-muted-foreground hover:text-accent"
								]}
								onclick={(event: MouseEvent) => {
									event.stopPropagation();
									delayOptionsVisible = !delayOptionsVisible;
								}}
							>
								<Settings size={18} />
							</button>
						</div>
					</MetricCardTitle>

					<DelayAnalysisInformationDialog bind:isVisible={delayInfoVisible} />

					<DelayChartOptionsDialog
						bind:isVisible={delayOptionsVisible}
						bind:scheduleType
						bind:delayMode
						bind:selectedSeries
						bind:handleCancelledAsDelayed
						bind:delayThreshold
						{availableTransportTypes}
					/>

					<LineChart
						data={delaySeries.flatMap((s: DelaySeries) => s.data)}
						series={delaySeries}
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
											{@const title =
												delayAnalysisKeyTitles[item.rawSeriesData?.key as DelayAnalysisSeriesType] ?? item.rawSeriesData?.key}
											<div class="flex items-center justify-between gap-x-4 text-xs">
												<div class="flex items-center gap-x-2">
													<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
													<span class="text-muted-foreground text-left">{title}</span>
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
				</div>
			</div>
		{:catch}
			<MetricLoadingFailedWarning />
		{/await}
	{/snippet}
</MetricCardBase>
