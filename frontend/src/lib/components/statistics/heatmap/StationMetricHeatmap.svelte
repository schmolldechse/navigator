<script module lang="ts">
	import type { BaseStation, MetricSeries, MetricUnit } from "@lib/api";

	type StationHeatmapPoint = {
		station: BaseStation;
		value: number;
	};

	type HeatmapMetricSeries = {
		points: StationHeatmapPoint[];
	} & Pick<MetricSeries, "unit" | "seriesType">;

	type StationHeatmapMarkerData = {
		station: BaseStation;
		value: number;
		unit: MetricUnit;
		seriesType: MetricSeries["seriesType"];
	};

	type HeatmapData = Awaited<ReturnType<typeof loadHeatmap>>;

	export type { StationHeatmapPoint, HeatmapMetricSeries, StationHeatmapMarkerData };
</script>

<script lang="ts">
	import { untrack } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import Map, { type MapHeatmapPoint, type MapMarker } from "@lib/components/ui/map/Map.svelte";
	import LoaderCircle from "@lucide/svelte/icons/loader-circle";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { interpolateTurbo } from "d3-scale-chromatic";
	import * as Tooltip from "@lib/components/ui/tooltip/index";
	import MapPin from "@lucide/svelte/icons/map-pin";
	import Separator from "@lib/components/ui/Separator.svelte";
	import type { loadHeatmap } from "./heatmap.remote";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import { formatHeatmapMetricValue, getHeatmapMetricOption } from "./heatmap";

	type Props = {
		promise: Promise<HeatmapData>;
		class?: ClassValue;
	};
	let { promise, class: classes = "" }: Props = $props();

	let heatmapPoints: MapHeatmapPoint[] = $state([]);
	let markers: MapMarker<StationHeatmapMarkerData>[] = $state([]);

	let scale: [number, number] = $state([0, 100]);
	let legendScale: [number, number] = $state([0, 100]);
	let scaleAnimationFrame: number | undefined;

	const formatScaleValue = (value: number) =>
		value.toLocaleString(undefined, {
			maximumFractionDigits: Math.abs(value) < 10 ? 2 : 0
		});
	const getStationName = (station: BaseStation) => station.name ?? `Station ${station.evaNumber}`;

	const animateLegendScale = (targetScale: [number, number]) => {
		if (scaleAnimationFrame !== undefined) cancelAnimationFrame(scaleAnimationFrame);

		const startScale = untrack(() => legendScale);
		const startedAt = performance.now();
		const duration = 320;

		const animate = (now: number) => {
			const progress = Math.min(1, (now - startedAt) / duration);
			const easedProgress = 1 - Math.pow(1 - progress, 3);

			legendScale = [
				startScale[0] + (targetScale[0] - startScale[0]) * easedProgress,
				startScale[1] + (targetScale[1] - startScale[1]) * easedProgress
			];

			if (progress < 1) {
				scaleAnimationFrame = requestAnimationFrame(animate);
				return;
			}

			scaleAnimationFrame = undefined;
			legendScale = targetScale;
		};

		scaleAnimationFrame = requestAnimationFrame(animate);
	};

	$effect(() => {
		promise.then((data: HeatmapData) => {
			heatmapPoints = data.points.map((point: StationHeatmapPoint) => ({
				id: point.station.evaNumber,
				latitude: Number(point.station.position.latitude),
				longitude: Number(point.station.position.longitude),
				value: point.value
			}));
			markers = data.points.map((point: StationHeatmapPoint) => ({
				id: point.station.evaNumber,
				label: getStationName(point.station),
				latitude: Number(point.station.position.latitude),
				longitude: Number(point.station.position.longitude),
				data: {
					station: point.station,
					value: point.value,
					unit: data.unit,
					seriesType: data.seriesType
				}
			}));
		});
	});

	$effect(() => {
		const nextScale = scale;
		animateLegendScale(nextScale);

		return () => {
			if (scaleAnimationFrame !== undefined) {
				cancelAnimationFrame(scaleAnimationFrame);
				scaleAnimationFrame = undefined;
			}
		};
	});
</script>

<div class={["relative overflow-hidden rounded-xl", classes]}>
	<Card class="bg-background/85! pointer-events-none absolute top-4 left-4 z-10 px-3! py-2! shadow-sm backdrop-blur-md">
		<p class="text-muted-foreground text-xs font-medium">Heatmap metric</p>
		{#await promise}
			<Skeleton class="h-6 w-32" />
		{:then data}
			<h2 class="text-foreground text-sm font-semibold sm:text-base">
				{getHeatmapMetricOption(data.seriesType)?.label ?? "???"}
			</h2>
		{/await}
	</Card>

	<Map
		bind:scale
		heatmap={true}
		{heatmapPoints}
		heatmapGradient={interpolateTurbo}
		{markers}
		markerMinZoom={13.5}
		markerRenderPadding={32}
		markerAnchor="center"
		class="h-full min-h-0"
	>
		{#snippet marker({ marker })}
			{@const metricOption = marker.data ? getHeatmapMetricOption(marker.data.seriesType) : undefined}
			{@const metricValue = marker.data
				? formatHeatmapMetricValue(marker.data.value, marker.data.unit)
				: { value: "N/A", unit: "" }}
			<Tooltip.Root delay={100}>
				<Tooltip.Trigger>
					<div
						class="border-background bg-background/90 text-accent ring-accent/25 hover:bg-accent hover:text-accent-foreground flex items-center justify-center rounded-full border-2 p-2 shadow-lg ring-2 transition-transform duration-150 hover:scale-110 hover:shadow-xl"
						aria-label={`Show ${marker.label ?? "station"} metric`}
					>
						<MapPin size={19} strokeWidth={2.6} />
					</div>
				</Tooltip.Trigger>

				<Tooltip.Content class="z-100 w-72 overflow-hidden p-0">
					<div class="bg-secondary/40 flex flex-col gap-y-1 px-3 py-2.5">
						<div class="flex items-start justify-between gap-x-3">
							<div class="min-w-0">
								<div class="text-foreground truncate text-sm font-semibold">{marker.label}</div>
							</div>

							<div class="bg-accent/15 text-accent rounded-md px-2 py-1 text-xs font-semibold whitespace-nowrap">
								{metricOption?.label ?? "Metric"}
							</div>
						</div>
					</div>

					<Separator class="h-px!" />

					<div class="grid gap-y-2 px-3 py-3">
						<div class="flex items-end justify-between gap-x-4">
							<span class="text-foreground/60 text-xs font-medium">{metricOption?.valueLabel ?? "Value"}</span>
							<span class="text-foreground text-right text-lg leading-none font-bold tabular-nums">
								{metricValue.value}
								{#if metricValue.unit}
									<span class="text-foreground/60 ml-1 text-xs font-semibold">{metricValue.unit}</span>
								{/if}
							</span>
						</div>

						{#if metricOption?.description}
							<p class="text-foreground/55 text-xs leading-relaxed">{metricOption.description}</p>
						{/if}
					</div>
				</Tooltip.Content>
			</Tooltip.Root>
		{/snippet}
	</Map>

	{#await promise}
		<div class="bg-background/50 absolute inset-0 z-10 flex flex-col items-center justify-center backdrop-blur-sm">
			<LoaderCircle size={32} class="text-accent animate-spin" />
			<span class="text-center font-bold">Generating heatmap...</span>
		</div>
	{:catch error}
		<div class="bg-background/80 absolute inset-0 z-10 flex flex-col items-center justify-center backdrop-blur-sm">
			<CircleAlert size={32} class="text-destructive" />
			<span class="text-center font-bold">An error occurred while generating the heatmap: {error.message}</span>
		</div>
	{/await}

	<div class="pointer-events-none absolute bottom-0 left-0 z-10 w-full max-w-md px-8 pb-5">
		<div class="flex flex-col gap-y-2">
			<div
				class="border-background/60 bg-background/70 h-2 overflow-hidden rounded-full border shadow-sm backdrop-blur-sm transition-opacity duration-300 ease-out"
				role="img"
				aria-label={`Heatmap scale from ${formatScaleValue(legendScale[0])} to ${formatScaleValue(legendScale[1])}`}
			>
				<div
					class="h-full w-full transition-all duration-300 ease-out"
					style={`background: linear-gradient(to right, ${[0, 0.2, 0.4, 0.6, 0.8, 1].map((density: number) => interpolateTurbo(density)).join(", ")})`}
				></div>
			</div>

			<div class="text-background grid grid-cols-3 text-xs font-bold tabular-nums drop-shadow-sm">
				<span class="transition-all duration-300 ease-out">{formatScaleValue(legendScale[0])}</span>
				<span class="text-center transition-all duration-300 ease-out">
					{formatScaleValue((legendScale[0] + legendScale[1]) / 2)}
				</span>
				<span class="text-right transition-all duration-300 ease-out">{formatScaleValue(legendScale[1])}</span>
			</div>
		</div>
	</div>
</div>
