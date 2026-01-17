<script lang="ts">
	import {
		MetricSeriesType,
		TransportType,
		type MetricDataPoint,
		type MetricDataPointTimestampMetricDataPoint,
		type MetricDataPointTransportTypeMetricDataPoint,
		type MetricSeries
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { onMount } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import MetricCardBase from "../MetricCardBase.svelte";
	import { Axis, ChartClipPath, Highlight, Legend, LineChart, PieChart, Spline, Svg, Tooltip } from "layerchart";
	import { DateTime } from "luxon";

	interface TransportTypeSeries {
		transportType: TransportType;
		value: number;
		color: string;
	}

	interface RecordedJourneysLineChartSeries {
		key: string;
		data: { date: Date; value: number }[];
		color: string;
	}

	interface KeyTitles {
		key: MetricSeriesType | TransportType;
		title: string;
	}

	const keyTitles: KeyTitles[] = [
		{ key: MetricSeriesType.JOURNEY_TOTAL, title: "Total Journeys" },
		{ key: TransportType.HIGH_SPEED_TRAIN, title: "High Speed" },
		{ key: TransportType.INTERCITY_TRAIN, title: "Intercity" },
		{ key: TransportType.INTER_REGIONAL_TRAIN, title: "Inter Regional" },
		{ key: TransportType.REGIONAL_TRAIN, title: "Regional" },
		{ key: TransportType.CITY_TRAIN, title: "Suburban" },
		{ key: TransportType.TRAM, title: "Tram" },
		{ key: TransportType.BUS, title: "Bus" },
		{ key: TransportType.UNKNOWN, title: "Unknown" }
	];

	interface Props {
		journeyPromise: Promise<MetricSeries[]>;
		transportTypePromise: Promise<MetricSeries[]>;
		class?: ClassValue;
	}

	let { journeyPromise, transportTypePromise, class: className }: Props = $props();
	onMount(async () => {
		const metrics = [...(await journeyPromise), ...(await transportTypePromise)];

		if (
			metrics.some(
				(metric: MetricSeries) =>
					metric.seriesType !== MetricSeriesType.JOURNEY_TOTAL && metric.seriesType !== MetricSeriesType.TRANSPORT_TYPES_TOTAL
			)
		)
			throw new Error("RecordedJourneysMetricCard received invalid metric series type.");
	});

	const colorScale = scaleOrdinal(schemeTableau10);
	const getTransportTypeSeries = (metrics: MetricSeries[]): TransportTypeSeries[] => {
		if (!metrics.length) return [];

		return metrics.flatMap((metric: MetricSeries) =>
			metric.dataPoints.map((dataPoint: MetricDataPoint) => {
				const transportTypeDataPoint = dataPoint as MetricDataPointTransportTypeMetricDataPoint;
				return {
					transportType: transportTypeDataPoint.transportType,
					value: Number(transportTypeDataPoint.value),
					color: colorScale(transportTypeDataPoint.transportType)
				};
			})
		);
	};

	const getTotalJourneys = (metrics: MetricSeries[]): RecordedJourneysLineChartSeries[] => {
		if (!metrics.length) return [];

		return metrics.map((metric: MetricSeries) => {
			const dataPoints = [...metric.dataPoints]
				.sort(
					(a: MetricDataPoint, b: MetricDataPoint) =>
						new Date((a as MetricDataPointTimestampMetricDataPoint).timestamp).getTime() -
						new Date((b as MetricDataPointTimestampMetricDataPoint).timestamp).getTime()
				)
				.map((dataPoint: MetricDataPoint) => ({
					date: new Date((dataPoint as MetricDataPointTimestampMetricDataPoint).timestamp),
					value: Number(dataPoint.value)
				}));

			return {
				key: metric.seriesType,
				data: dataPoints,
				color: colorScale(metric.seriesType)
			};
		});
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	{#snippet children()}
		<p class="text-muted-foreground text-xs font-semibold tracking-wider uppercase">Recorded Journeys</p>

		<div class="flex flex-col items-center lg:flex-row">
			<!-- Distribution of Transport Types -->
			{#await transportTypePromise}
				<div class="bg-muted h-64 w-full animate-pulse rounded-md lg:w-1/3"></div>
			{:then transportTypes}
				<div class="h-64 w-full lg:w-1/3">
					<PieChart
						data={getTransportTypeSeries(transportTypes)}
						key="transportType"
						value="value"
						c={(data: TransportTypeSeries) => data.color}
						outerRadius={80}
						innerRadius={-20}
						cornerRadius={4}
						padAngle={0.02}
						padding={{ bottom: 30 }}
					>
						{#snippet legend({ getLegendProps })}
							<Legend
								{...getLegendProps()}
								placement="bottom"
								orientation="horizontal"
								variant="swatches"
								classes={{
									root: "pointer-events-auto w-full",
									label: "font-[Roboto_Mono] text-xs font-medium whitespace-nowrap",
									swatch: "w-2.5 h-2.5",
									item: "gap-x-2 cursor-pointer",
									items: "flex flex-wrap justify-center"
								}}
								tickFormat={(seriesType: string) => {
									const keyTitle = keyTitles.find((keyTitle: KeyTitles) => keyTitle.key === seriesType)?.title ?? seriesType;
									return keyTitle;
								}}
							/>
						{/snippet}

						{#snippet tooltip({ context })}
							{@const total = context.flatData.reduce((acc: number, curr: TransportTypeSeries) => acc + curr.value, 0)}
							{@const formatter = new Intl.NumberFormat(undefined, { notation: "compact", maximumFractionDigits: 1 })}

							<Tooltip.Root class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md">
								{#snippet children({ data }: { data: TransportTypeSeries })}
									{@const title =
										keyTitles.find((keyTitle: KeyTitles) => keyTitle.key === data.transportType)?.title ?? data.transportType}

									{@const percentage = ((data.value / total) * 100).toFixed(2)}
									{@const formattedValue = formatter.format(data.value)}

									<p class="text-base font-bold" style:color={data.color}>{title}</p>
									<Tooltip.Separator class="border-muted-foreground/20 my-1 border-t" />
									<div class="flex flex-row justify-between gap-x-4">
										<span class="text-sm font-medium">{formattedValue}</span>
										<span class="text-xs text-emerald-400 italic">({percentage} %)</span>
									</div>
								{/snippet}
							</Tooltip.Root>
						{/snippet}
					</PieChart>
				</div>
			{/await}

			<!-- Total Recorded Journeys -->
			{#await journeyPromise}
				<div class="bg-muted h-64 w-full animate-pulse rounded-md lg:min-w-2/3"></div>
			{:then journeys}
				<div class="h-64 w-full lg:min-w-2/3">
					<LineChart
						data={getTotalJourneys(journeys).flatMap((series: RecordedJourneysLineChartSeries) => series.data)}
						series={getTotalJourneys(journeys)}
						x="date"
						y="value"
						padding={{ left: 64, top: 16, bottom: 12 }}
						yDomain={null}
						tooltip={{ mode: "quadtree-x" }}
						brush
					>
						{#snippet children({ context, visibleSeries, getSplineProps, getHighlightProps })}
							<Svg>
								<Axis
									placement="left"
									grid
									rule
									classes={{
										tickLabel: "text-xs stroke-0 text-muted-foreground",
										rule: "stroke-muted-foreground/20"
									}}
									format={(value: number) => {
										const formatter = new Intl.NumberFormat(undefined, { notation: "compact", maximumFractionDigits: 1 });
										return formatter.format(value);
									}}
								/>
								<Axis
									placement="bottom"
									rule
									classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground", rule: "stroke-0" }}
								/>

								<ChartClipPath>
									{#each visibleSeries as series, i (series.key)}
										<Spline {...getSplineProps(series, i)} class="stroke-2" />
										<Highlight {...getHighlightProps(series, i)} points lines={{ class: "stroke-muted-foreground/30" }} />
									{/each}
								</ChartClipPath>
							</Svg>

							<!-- Data Tooltip -->
							<Tooltip.Root
								anchor="bottom"
								contained="container"
								class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md"
							>
								{#snippet children({ payload })}
									{@const formatter = new Intl.NumberFormat(undefined, { notation: "compact", maximumFractionDigits: 1 })}

									<div class="flex flex-col gap-y-1">
										{#each [...payload].reverse() as item}
											{@const title =
												keyTitles?.find((title: KeyTitles) => title.key === item.rawSeriesData?.key)?.title ??
												item.rawSeriesData?.key}
											<div class="flex justify-between gap-x-4">
												<div class="flex items-center gap-x-2">
													<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
													<span class="text-muted-foreground text-left">{title}</span>
												</div>

												<span class="text-text">{formatter.format(item.value)}</span>
											</div>
										{/each}
									</div>
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
					</LineChart>
				</div>
			{/await}
		</div>
	{/snippet}
</MetricCardBase>
