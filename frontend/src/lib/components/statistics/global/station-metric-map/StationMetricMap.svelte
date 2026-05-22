<script module lang="ts">
	import type { BaseStation, MetricSample, MetricSeries, MetricUnit } from "@lib/api";

	type StationMetricMapPoint = {
		station: BaseStation;
		value: number;
		sample?: MetricSample | null;
	};

	type StationMetricMapSeries = {
		points: StationMetricMapPoint[];
	} & Pick<MetricSeries, "unit" | "seriesType">;

	type StationMetricMapMarkerData = {
		station: BaseStation;
		value: number;
		sample?: MetricSample | null;
	} & Pick<MetricSeries, "unit" | "seriesType">;

	export type { StationMetricMapPoint, StationMetricMapSeries, StationMetricMapMarkerData };
</script>

<script lang="ts">
	import { untrack } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import Map, { type MapHeatmapPoint, type MapMarker, type MapValuePoint } from "@lib/components/ui/map/Map.svelte";
	import LoaderCircle from "@lucide/svelte/icons/loader-circle";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import MapPin from "@lucide/svelte/icons/map-pin";
	import { interpolateRdYlGn, interpolateTurbo, interpolateViridis } from "d3-scale-chromatic";
	import * as Tooltip from "@lib/components/ui/tooltip/index";
	import Separator from "@lib/components/ui/Separator.svelte";
	import type { loadStationMetricMap } from "./station-metric-map.remote";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import { getStationMetricMapOption, type StationMetricMapOption } from "./station-metric-map";
	import { formatMetricValue } from "../../metric-format";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadStationMetricMap>>>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	let heatmapPoints: MapHeatmapPoint[] = $state([]);
	let valuePoints: MapValuePoint[] = $state([]);
	let markers: MapMarker<StationMetricMapMarkerData>[] = $state([]);

	let scale: [number, number] = $state([0, 100]);
	let legendScale: [number, number] = $state([0, 100]);
	let pointDomain: [number, number] = $state([0, 100]);

	let scaleAnimationFrame: number | undefined;

	let promiseVersion = 0;

	const formatScaleValue = (value: number, unit?: MetricUnit) =>
		unit
			? [formatMetricValue(value, unit).value, formatMetricValue(value, unit).unit].filter(Boolean).join(" ")
			: value.toLocaleString(undefined, {
					maximumFractionDigits: Math.abs(value) < 10 ? 2 : 0
				});
	const clamp01 = (value: number) => Math.min(1, Math.max(0, value));

	const colorForNormalizedValue = (normalizedValue: number, metricOption: StationMetricMapOption) => {
		const value = clamp01(normalizedValue);

		if (metricOption.polarity === "positive") return interpolateRdYlGn(value);
		if (metricOption.polarity === "negative") return interpolateRdYlGn(1 - value);
		return interpolateViridis(value);
	};

	const createLegendGradient = (metricOption: StationMetricMapOption) =>
		[0, 0.2, 0.4, 0.6, 0.8, 1].map((density: number) => colorForNormalizedValue(density, metricOption)).join(", ");

	const createDensityGradient = () => [0, 0.2, 0.4, 0.6, 0.8, 1].map((density: number) => interpolateTurbo(density)).join(", ");

	const getPointDomain = (points: StationMetricMapPoint[], metricOption: StationMetricMapOption): [number, number] => {
		if (metricOption.domain !== "data") return metricOption.domain;

		const values = points.map((point: StationMetricMapPoint) => point.value).filter((value: number) => Number.isFinite(value));
		if (values.length === 0) return [0, 100];

		const lower = Math.min(...values);
		const upper = Math.max(...values);

		if (lower === upper) {
			if (upper === 0) return [0, 1];
			return [Math.min(0, lower), upper];
		}

		return [lower, upper];
	};

	const getPointColor = (value: number, domain: [number, number], metricOption: StationMetricMapOption) => {
		const [lower, upper] = domain;
		if (!Number.isFinite(value) || lower === upper) return colorForNormalizedValue(0.5, metricOption);

		return colorForNormalizedValue((value - lower) / (upper - lower), metricOption);
	};

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
		const currentPromise = promise;
		const version = ++promiseVersion;

		currentPromise
			.then((data: Awaited<ReturnType<typeof loadStationMetricMap>>) => {
				if (version !== promiseVersion) return;

				const metricOption = getStationMetricMapOption(data.seriesType);
				const visualization = metricOption?.visualization ?? "density";
				const nextPointDomain: [number, number] = metricOption ? getPointDomain(data.points, metricOption) : [0, 100];

				pointDomain = nextPointDomain;
				heatmapPoints =
					visualization === "density"
						? data.points.map((point: StationMetricMapPoint) => ({
								id: point.station.evaNumber,
								latitude: Number(point.station.position.latitude),
								longitude: Number(point.station.position.longitude),
								value: point.value
							}))
						: [];
				valuePoints =
					visualization === "points" && metricOption
						? data.points.map((point: StationMetricMapPoint) => ({
								id: point.station.evaNumber,
								latitude: Number(point.station.position.latitude),
								longitude: Number(point.station.position.longitude),
								value: point.value,
								color: getPointColor(point.value, nextPointDomain, metricOption)
							}))
						: [];
				markers = data.points.map((point: StationMetricMapPoint) => ({
					id: point.station.evaNumber,
					label: point.station.name ?? `Station ${point.station.evaNumber}`,
					latitude: Number(point.station.position.latitude),
					longitude: Number(point.station.position.longitude),
					data: {
						station: point.station,
						value: point.value,
						unit: data.unit,
						seriesType: data.seriesType,
						sample: point.sample
					}
				}));
			})
			.catch(() => {
				if (version !== promiseVersion) return;

				heatmapPoints = [];
				valuePoints = [];
				markers = [];
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

<div class={["relative overflow-hidden rounded-xl", className]}>
	<Card class="bg-background/85! pointer-events-none absolute top-4 left-4 z-10 px-3! py-2! shadow-sm backdrop-blur-md">
		<p class="text-foreground/60 text-xs font-medium">Map metric</p>
		{#await promise}
			<Skeleton class="h-6 w-32" />
		{:then data}
			<h2 class="text-foreground text-sm font-semibold sm:text-base">
				{getStationMetricMapOption(data.seriesType)?.label ?? "???"}
			</h2>
		{/await}
	</Card>

	<Map
		bind:scale
		heatmap={heatmapPoints.length > 0}
		{heatmapPoints}
		heatmapGradient={interpolateTurbo}
		valuePointLayer={valuePoints.length > 0}
		{valuePoints}
		{markers}
		markerMinZoom={13.5}
		markerRenderPadding={32}
		markerAnchor="center"
		class="h-full min-h-0"
	>
		{#snippet marker({ marker })}
			{@const metricOption = marker.data ? getStationMetricMapOption(marker.data.seriesType) : undefined}
			{@const metricValue = marker.data ? formatMetricValue(marker.data.value, marker.data.unit) : { value: "N/A", unit: "" }}
			{@const pointMode = metricOption?.visualization === "points"}

			<Tooltip.Root delay={100}>
				<Tooltip.Trigger>
					<div
						class={[
							pointMode && "size-5",
							!pointMode &&
								"bg-background/90 border-background text-accent hover:bg-accent hover:text-accent-foreground rounded-full border-2 p-2 transition-transform duration-150 hover:scale-110 hover:shadow-xl"
						]}
						aria-label={`Show ${marker.label ?? "station"} metric`}
					>
						{#if !pointMode}
							<MapPin size={19} strokeWidth={2.6} />
						{/if}
					</div>
				</Tooltip.Trigger>

				<Tooltip.Content class="w-72 overflow-hidden">
					<div class="bg-secondary/40 flex flex-row items-center justify-between gap-x-3 px-3 py-2.5">
						<div class="text-foreground truncate text-sm font-semibold">{marker.label}</div>

						<div class="bg-accent/15 text-accent rounded-md px-2 py-1 text-xs font-semibold whitespace-nowrap">
							{metricOption?.label ?? "Metric"}
						</div>
					</div>

					<Separator />

					<div class="flex flex-col gap-y-2 px-3 py-3">
						<div class="flex justify-between gap-x-4">
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

						{#if marker.data?.sample && metricOption?.sampleLabels}
							<div class="border-border/80 bg-secondary/30 rounded-md border px-2.5 py-2">
								<p class="text-foreground mb-1.5 text-xs font-semibold">Sample</p>

								<ul class="text-foreground/60 flex flex-col gap-y-1 text-xs">
									<li class="flex justify-between gap-x-3">
										<span>{metricOption.sampleLabels.numerator}</span>
										<span class="text-foreground font-medium tabular-nums">
											{Number(marker.data.sample.numerator).toLocaleString()}
										</span>
									</li>

									<li class="flex justify-between gap-x-3">
										<span>{metricOption.sampleLabels.denominator}</span>
										<span class="text-foreground font-medium tabular-nums">
											{Number(marker.data.sample.denominator).toLocaleString()}
										</span>
									</li>
								</ul>
							</div>
						{/if}
					</div>
				</Tooltip.Content>
			</Tooltip.Root>
		{/snippet}
	</Map>

	{#await promise}
		<div class="bg-background/50 absolute inset-0 z-10 flex flex-col items-center justify-center backdrop-blur-sm">
			<LoaderCircle size={32} class="text-accent animate-spin" />
			<span class="text-center font-bold">Generating station metric map...</span>
		</div>
	{:catch error}
		<div class="bg-background/80 absolute inset-0 z-10 flex flex-col items-center justify-center backdrop-blur-sm">
			<CircleAlert size={32} class="text-destructive" />
			<span class="text-center font-bold">An error occurred while generating the station metric map: {error.message}</span>
		</div>
	{/await}

	<div class="pointer-events-none absolute bottom-0 left-0 z-10 w-full max-w-md px-8 pb-5">
		{#await promise}
			<div class="bg-background/70 h-10 rounded-lg backdrop-blur-sm"></div>
		{:then data}
			{@const metricOption = getStationMetricMapOption(data.seriesType)}
			{#if metricOption?.visualization === "points"}
				<div class="flex flex-col gap-y-2">
					<div
						class="border-background/60 bg-background/70 h-2 overflow-hidden rounded-full border shadow-sm backdrop-blur-sm transition-opacity duration-300 ease-out"
						role="img"
						aria-label={`Metric scale from ${formatScaleValue(pointDomain[0], data.unit)} to ${formatScaleValue(pointDomain[1], data.unit)}`}
					>
						<div
							class="h-full w-full transition-all duration-300 ease-out"
							style={`background: linear-gradient(to right, ${createLegendGradient(metricOption)})`}
						></div>
					</div>

					<div class="text-background grid grid-cols-3 text-xs font-bold tabular-nums drop-shadow-sm">
						<span>{formatScaleValue(pointDomain[0], data.unit)}</span>
						<span class="text-center">{formatScaleValue((pointDomain[0] + pointDomain[1]) / 2, data.unit)}</span>
						<span class="text-right">{formatScaleValue(pointDomain[1], data.unit)}</span>
					</div>
				</div>
			{:else}
				<div class="flex flex-col gap-y-2">
					<div
						class="border-background/60 bg-background/70 h-2 overflow-hidden rounded-full border shadow-sm backdrop-blur-sm transition-opacity duration-300 ease-out"
						role="img"
						aria-label="Heatmap density scale from sparse to dense"
					>
						<div
							class="h-full w-full transition-all duration-300 ease-out"
							style={`background: linear-gradient(to right, ${createDensityGradient()})`}
						></div>
					</div>

					<div class="text-background grid grid-cols-2 text-xs font-bold drop-shadow-sm">
						<span>Sparse</span>
						<span class="text-right">Dense</span>
					</div>

					<p class="text-background text-xs font-bold drop-shadow-sm">
						Visible station values: {formatScaleValue(legendScale[0], data.unit)} - {formatScaleValue(
							legendScale[1],
							data.unit
						)}
					</p>
				</div>
			{/if}
		{/await}
	</div>
</div>
