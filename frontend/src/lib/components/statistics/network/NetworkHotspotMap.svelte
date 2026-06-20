<script lang="ts">
	import { goto } from "$app/navigation";
	import type { GeoJsonFeature, GeoJsonFeatureCollection } from "@lib/api";
	import Map, {
		type MapHeatmapGradientStop,
		type MapHeatmapPoint,
		type MapMarker,
		type MapMarkerRenderContext,
		type MapValuePoint
	} from "@lib/components/ui/map/Map.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import MapPinned from "@lucide/svelte/icons/map-pinned";
	import { quantileSorted } from "d3-array";
	import { scaleLinear } from "d3-scale";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import MetricOptionControl from "../shared/options/MetricOptionControl.svelte";
	import type { NetworkMapMetric } from "./network-context.svelte";
	import { formatCount, formatMetric, toNumber } from "../shared/statistics-dashboard";

	type HotspotData = {
		stationEvaNumber: number;
		stationName: string;
		plannedEvents: number | null;
		customerReliability5Rate: number | null;
		operativePunctuality5Rate: number | null;
		cancellationRate: number | null;
		averagePositiveDelayMinutes: number | null;
	};

	type HotspotPoint = {
		id: number;
		longitude: number;
		latitude: number;
		data: HotspotData;
	};

	type Props = {
		promise: RemoteQuery<GeoJsonFeatureCollection>;
		metric: NetworkMapMetric;
		onmetricchange: (metric: NetworkMapMetric) => void;
	};

	type MapMetricOption = {
		value: NetworkMapMetric;
		label: string;
		description: string;
		direction: "higher" | "lower" | "neutral";
		rangeLabels: [string, string];
		getValue: (data: HotspotData) => number | null;
	};

	type StationMarkerData = {
		station: HotspotData;
		formattedValue: string;
		color: string;
	};

	type MetricRanges = {
		observed: [number, number];
		color: [number, number];
	};

	let { promise, metric, onmetricchange }: Props = $props();

	const metricOptions: MapMetricOption[] = [
		{
			value: "reliability5",
			label: "Customer < 6 min",
			description: "Share of all planned station stops that were served with less than six minutes delay.",
			direction: "higher",
			rangeLabels: ["Lower reliability", "Higher reliability"],
			getValue: (data) => data.customerReliability5Rate
		},
		{
			value: "operative5",
			label: "Operative < 6 min",
			description: "Share of served station stops with less than six minutes delay; cancellations are excluded.",
			direction: "higher",
			rangeLabels: ["Lower punctuality", "Higher punctuality"],
			getValue: (data) => data.operativePunctuality5Rate
		},
		{
			value: "cancellation",
			label: "Cancellation rate",
			description: "Share of planned station stops that were cancelled.",
			direction: "lower",
			rangeLabels: ["Lower cancellations", "Higher cancellations"],
			getValue: (data) => data.cancellationRate
		},
		{
			value: "averagePositiveDelay",
			label: "Avg positive delay",
			description: "Positive delay minutes per served stop, normalized so stations with different volumes remain comparable.",
			direction: "lower",
			rangeLabels: ["Lower delay", "Higher delay"],
			getValue: (data) => data.averagePositiveDelayMinutes
		},
		{
			value: "plannedStops",
			label: "Planned stops",
			description: "Spatial concentration of planned station stops; this is service volume, not a quality score.",
			direction: "neutral",
			rangeLabels: ["Fewer stops", "More stops"],
			getValue: (data) => data.plannedEvents
		}
	];
	const selectedMetric = $derived(metricOptions.find((option) => option.value === metric) ?? metricOptions[0]);
	const plannedStopsSelected = $derived(selectedMetric.value === "plannedStops");
	const QUALITY_COLORS: [string, string, string] = ["#be4b5f", "#d4a72c", "#2a9d8f"];
	const VOLUME_COLORS: [string, string, string] = ["#33301e", "#8f7810", "#ffda0a"];
	const NO_DATA_COLOR = "#94a3b8";
	const PLANNED_STOPS_HEATMAP_GRADIENT: MapHeatmapGradientStop[] = [
		{ density: 0, color: "rgba(255, 218, 10, 0)" },
		{ density: 0.015, color: "rgba(255, 218, 10, 0.28)" },
		{ density: 0.06, color: "rgba(255, 218, 10, 0.48)" },
		{ density: 0.18, color: "rgba(255, 218, 10, 0.68)" },
		{ density: 0.38, color: "rgba(255, 218, 10, 0.84)" },
		{ density: 0.65, color: "rgba(255, 218, 10, 0.95)" },
		{ density: 1, color: "#ffda0a" }
	];
	const qualityColorScale = scaleLinear<string>().domain([0, 0.5, 1]).range(QUALITY_COLORS).clamp(true);
	const volumeColorScale = scaleLinear<string>().domain([0, 0.5, 1]).range(VOLUME_COLORS).clamp(true);

	const getProperty = (feature: GeoJsonFeature, key: string): unknown => feature.properties?.[key];
	const clampRate = (value: number): number => Math.min(1, Math.max(0, value));

	const createHotspotData = (feature: GeoJsonFeature): HotspotData | null => {
		const stationEvaNumber = toNumber(getProperty(feature, "stationEvaNumber") as number | string | null | undefined);
		if (stationEvaNumber === null) return null;
		const plannedEvents = toNumber(getProperty(feature, "plannedEvents") as number | string | null | undefined);
		const customerReliability5Rate = toNumber(
			getProperty(feature, "customerReliability5Rate") as number | string | null | undefined
		);
		const cancellationRate = toNumber(getProperty(feature, "cancellationRate") as number | string | null | undefined);
		const delayDebtMinutes = toNumber(getProperty(feature, "delayDebtMinutes") as number | string | null | undefined);
		const servedShare = cancellationRate === null ? null : 1 - clampRate(cancellationRate);
		const servedEvents = plannedEvents !== null && servedShare !== null ? plannedEvents * servedShare : null;

		return {
			stationEvaNumber,
			stationName: String(getProperty(feature, "stationName") ?? "Unnamed station"),
			plannedEvents,
			customerReliability5Rate,
			operativePunctuality5Rate:
				customerReliability5Rate !== null && servedShare !== null && servedShare > 0
					? clampRate(customerReliability5Rate / servedShare)
					: null,
			cancellationRate,
			averagePositiveDelayMinutes:
				delayDebtMinutes !== null && servedEvents !== null && servedEvents > 0 ? delayDebtMinutes / servedEvents : null
		};
	};

	const metricColor = (value: number | null, option: MapMetricOption, range: [number, number]): string => {
		if (value === null) return NO_DATA_COLOR;
		const [min, max] = range;
		const ratio = max === min ? 0.5 : Math.min(1, Math.max(0, (value - min) / (max - min)));
		if (option.direction === "neutral") return volumeColorScale(ratio);
		return qualityColorScale(option.direction === "lower" ? 1 - ratio : ratio);
	};

	const formatMapValue = (value: number | null, option: MapMetricOption, compact = false): string => {
		if (option.value === "reliability5" || option.value === "operative5" || option.value === "cancellation")
			return formatMetric(value, "rate");
		if (option.value === "averagePositiveDelay") return formatMetric(value, "minutes", compact);
		return formatCount(value, compact);
	};

	const createPoints = (features: GeoJsonFeature[]): HotspotPoint[] =>
		features
			.map((feature) => {
				const coordinates = feature.geometry.coordinates;
				const longitude = toNumber(coordinates[0]);
				const latitude = toNumber(coordinates[1]);
				const data = createHotspotData(feature);
				if (longitude === null || latitude === null || !data) return null;

				const point: HotspotPoint = {
					id: data.stationEvaNumber,
					longitude,
					latitude,
					data
				};

				return point;
			})
			.filter((point): point is HotspotPoint => point !== null);

	const createRanges = (points: HotspotPoint[], option: MapMetricOption): MetricRanges => {
		const values = points
			.map((point) => option.getValue(point.data))
			.filter((value): value is number => value !== null)
			.sort((left, right) => left - right);
		if (values.length === 0) return { observed: [0, 1], color: [0, 1] };
		if (values.length === 1) return { observed: [values[0], values[0]], color: [values[0], values[0]] };

		const observed: [number, number] = [values[0], values.at(-1) ?? values[0]];
		const color: [number, number] = [quantileSorted(values, 0.05) ?? observed[0], quantileSorted(values, 0.95) ?? observed[1]];
		return { observed, color };
	};

	const createValuePoints = (points: HotspotPoint[], option: MapMetricOption, range: [number, number]): MapValuePoint[] =>
		points.map((point) => {
			const value = option.getValue(point.data);
			return {
				id: point.id,
				longitude: point.longitude,
				latitude: point.latitude,
				value: value ?? 0,
				color: metricColor(value, option, range)
			};
		});

	const createHeatmapPoints = (points: HotspotPoint[]): MapHeatmapPoint[] =>
		points
			.filter((point) => point.data.plannedEvents !== null)
			.map((point) => ({
				id: point.id,
				longitude: point.longitude,
				latitude: point.latitude,
				value: point.data.plannedEvents ?? 0
			}));

	const createMarkers = (
		points: HotspotPoint[],
		option: MapMetricOption,
		range: [number, number]
	): MapMarker<StationMarkerData>[] =>
		points.map((point) => {
			const value = option.getValue(point.data);
			const formattedValue = formatMapValue(value, option, true);
			return {
				id: point.id,
				longitude: point.longitude,
				latitude: point.latitude,
				label: `${point.data.stationName}, ${option.label}: ${formattedValue}`,
				data: {
					station: point.data,
					formattedValue,
					color: metricColor(value, option, range)
				}
			};
		});

	const legendColors = (option: MapMetricOption): [string, string, string] => {
		if (option.direction === "neutral") return VOLUME_COLORS;
		return option.direction === "higher" ? QUALITY_COLORS : [QUALITY_COLORS[2], QUALITY_COLORS[1], QUALITY_COLORS[0]];
	};

	const rangePosition = (value: number, range: [number, number]): number => {
		const [min, max] = range;
		if (min === max) return 50;
		return Math.min(100, Math.max(0, ((value - min) / (max - min)) * 100));
	};

	const legendGradient = (colors: [string, string, string], ranges: MetricRanges): string => {
		const [colorMin, colorMax] = ranges.color;
		if (ranges.observed[0] === ranges.observed[1] || colorMin === colorMax) return colors[1];

		const lowerStop = rangePosition(colorMin, ranges.observed);
		const middleStop = rangePosition((colorMin + colorMax) / 2, ranges.observed);
		const upperStop = rangePosition(colorMax, ranges.observed);
		return `linear-gradient(to right, ${colors[0]} 0%, ${colors[0]} ${lowerStop}%, ${colors[1]} ${middleStop}%, ${colors[2]} ${upperStop}%, ${colors[2]} 100%)`;
	};

	const heatmapLegendGradient = `linear-gradient(to right, rgba(255, 218, 10, 0.08), rgba(255, 218, 10, 0.58), #ffda0a)`;

	const openStation = async (point: MapValuePoint) => {
		await goto(`/statistics/${point.id}`);
	};

	const openStationMarker = async (marker: MapMarker<StationMarkerData>) => {
		await goto(`/statistics/${marker.id}`);
	};
</script>

{#snippet stationMarker({ marker }: MapMarkerRenderContext<StationMarkerData>)}
	{@const markerData = marker.data}
	<div
		class="border-background bg-background/95 text-foreground flex max-w-52 cursor-pointer items-center gap-2 rounded-full border-2 py-1 pr-3 pl-1.5 shadow-xl backdrop-blur-md transition-transform hover:scale-[1.03]"
	>
		<span
			class="ring-background size-4 shrink-0 rounded-full ring-2"
			style:background-color={markerData?.color ?? NO_DATA_COLOR}
		></span>
		<span class="min-w-0 leading-tight">
			<span class="block truncate text-[0.68rem] font-bold">{markerData?.station.stationName ?? marker.label}</span>
			<span class="text-foreground/60 block text-[0.62rem] font-semibold tabular-nums">
				{markerData?.formattedValue ?? "No data"}
			</span>
		</span>
	</div>
{/snippet}

<DashboardPanel title="Station performance" description={selectedMetric.description} icon={MapPinned} class="overflow-hidden">
	{#snippet actions()}
		<MetricOptionControl value={metric} options={metricOptions} onchange={onmetricchange} />
	{/snippet}
	{#if promise.loading}
		<div class="flex min-h-[22rem] flex-col gap-y-3 sm:min-h-[28rem] xl:min-h-[32rem]">
			<Skeleton class="h-[22rem] w-full sm:h-[28rem] xl:h-[30rem]" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-[22rem] flex-col items-center justify-center gap-2 rounded-lg border text-center sm:min-h-[28rem] xl:min-h-[32rem]"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const points = createPoints(promise.current.features)}
		{@const ranges = createRanges(points, selectedMetric)}
		{@const valuePoints = createValuePoints(points, selectedMetric, ranges.color)}
		{@const heatmapPoints = createHeatmapPoints(points)}
		{@const markers = createMarkers(points, selectedMetric, ranges.color)}
		{@const colors = legendColors(selectedMetric)}
		{#if points.length === 0}
			<div
				class="border-border bg-secondary/25 flex min-h-[22rem] items-center justify-center rounded-lg border text-center sm:min-h-[28rem] xl:min-h-[32rem]"
			>
				<p class="text-foreground/60 text-sm font-semibold">No station hotspots available.</p>
			</div>
		{:else}
			<Map
				scale={ranges.color}
				{valuePoints}
				valuePointLayer={!plannedStopsSelected}
				heatmap={plannedStopsSelected}
				heatmapPoints={plannedStopsSelected ? heatmapPoints : []}
				heatmapGradient={PLANNED_STOPS_HEATMAP_GRADIENT}
				heatmapIntensityMultiplier={2.25}
				heatmapRadiusMultiplier={1.6}
				{markers}
				marker={stationMarker}
				markerMinZoom={12.5}
				markerAnchor="bottom"
				markerOffset={[0, -7]}
				markerRenderPadding={96}
				center={{ latitude: 51.1657, longitude: 10.4515 }}
				zoom={5.4}
				onvaluepointselect={openStation}
				onmarkerselect={openStationMarker}
				class="min-h-[22rem] overflow-hidden rounded-lg sm:min-h-[28rem] xl:min-h-[32rem]"
				ariaLabel={`Station performance map showing ${selectedMetric.label}`}
			/>
			<div class="mt-3 grid gap-1.5" aria-label={`${selectedMetric.label} color scale`}>
				{#if plannedStopsSelected}
					<div class="h-1.5 rounded-full" style:background={heatmapLegendGradient} aria-hidden="true"></div>
					<div class="text-foreground/55 flex items-start justify-between gap-3 text-xs font-semibold">
						<span class="text-foreground font-bold">Lower service concentration</span>
						<span class="text-center">Planned stops · spatial density</span>
						<span class="text-foreground text-right font-bold">Higher service concentration</span>
					</div>
					<p class="text-foreground/45 text-center text-[0.65rem] font-semibold">
						Heat intensity combines nearby stations and their planned stop volume. Zoom in closely to show individual station
						values.
					</p>
				{:else}
					<div class="h-1.5 rounded-full" style:background={legendGradient(colors, ranges)} aria-hidden="true"></div>
					<div class="text-foreground/55 flex items-start justify-between gap-3 text-xs font-semibold">
						<span class="min-w-0">
							<span class="text-foreground block font-bold">{selectedMetric.rangeLabels[0]}</span>
							{formatMapValue(ranges.observed[0], selectedMetric, true)}
						</span>
						<span class="text-center">All stations · {selectedMetric.label}</span>
						<span class="min-w-0 text-right">
							<span class="text-foreground block font-bold">{selectedMetric.rangeLabels[1]}</span>
							{formatMapValue(ranges.observed[1], selectedMetric, true)}
						</span>
					</div>
					<p class="text-foreground/45 text-center text-[0.65rem] font-semibold">
						Legend values show the observed minimum and maximum; colors clamp the outer 5% at each end. Zoom in closely to show
						station names and values.
					</p>
				{/if}
			</div>
		{/if}
	{/if}
</DashboardPanel>
