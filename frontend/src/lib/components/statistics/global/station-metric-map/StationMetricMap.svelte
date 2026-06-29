<script lang="ts">
	import StationMetricMapView from "./StationMetricMapView.svelte";
	import { loadStationMetricMap } from "./station-metric-map.remote";
	import {
		createStationMetricMapRequest,
		STATION_METRIC_MAP_OPTIONS,
		type StationMetricMapOption,
		type StationMetricMapSettings
	} from "./station-metric-map";
	import { getStatisticsScopeContext } from "../statistics-scope-context.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";

	type Props = {
		isLoading: boolean;
		settings: StationMetricMapSettings;
		promise: Promise<Awaited<ReturnType<typeof loadStationMetricMap>>>;
	};
	let { isLoading = $bindable(true), settings: initialSettings, promise: initialPromise }: Props = $props();

	const scopeContext = getStatisticsScopeContext();

	// svelte-ignore state_referenced_locally
	let settings: StationMetricMapSettings = $state({ ...initialSettings });

	let request: ReturnType<typeof createStationMetricMapRequest> = $derived(
		createStationMetricMapRequest(settings, scopeContext.current)
	);
	// svelte-ignore state_referenced_locally
	let promise: Promise<Awaited<ReturnType<typeof loadStationMetricMap>>> = $state(initialPromise);

	let initialized: boolean = false;

	$effect(() => {
		const currentRequest = request;
		if (!currentRequest) return;

		if (!initialized) {
			initialized = true;
			return;
		}

		promise = loadStationMetricMap({ request: currentRequest });
	});

	$effect(() => {
		const currentPromise = promise;
		if (!currentPromise) return;

		isLoading = "loading" in currentPromise && Boolean(currentPromise.loading);
	});

	let selectedMetricOption: StationMetricMapOption | undefined = $derived(
		STATION_METRIC_MAP_OPTIONS.find((option: StationMetricMapOption) => option.seriesType === settings.seriesType)
	);

	const selectMetric = (option: StationMetricMapOption) => {
		if (option.seriesType === settings.seriesType) return;

		settings = { seriesType: option.seriesType };
	};
</script>

<section class="space-y-4">
	<div class="relative flex flex-col gap-3 xl:flex-row xl:items-start xl:justify-between">
		<h2 class="text-2xl font-semibold">Station Quality Map</h2>

		<ToggleGroup
			mode="single"
			allowEmpty={false}
			selected={selectedMetricOption}
			keyFn={(option: StationMetricMapOption) => option.seriesType}
			onselect={(option: StationMetricMapOption | undefined) => {
				if (option) selectMetric(option);
			}}
			class="gap-1.5 xl:justify-end"
		>
			{#each STATION_METRIC_MAP_OPTIONS as option (option.seriesType)}
				<ToggleGroupItem
					item={option}
					disabled={isLoading}
					title={option.description}
					aria-label={`${option.label}: ${option.description}`}
					class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground px-2.5 py-1.5 text-xs font-semibold transition-colors"
				>
					{option.label}
				</ToggleGroupItem>
			{/each}
		</ToggleGroup>
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		Compare station activity as density and station quality as value-colored points, then tune the time range, schedule
		direction, and transport mix when you need a sharper view.
	</p>

	<StationMetricMapView {promise} class="h-[62vh] max-h-[48rem] min-h-[30rem]" />
</section>
