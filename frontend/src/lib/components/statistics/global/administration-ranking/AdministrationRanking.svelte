<script lang="ts">
	import { loadMetric } from "@lib/remote/metrics.remote";
	import {
		ADMINISTRATION_RANKING_METRIC_OPTIONS,
		createAdministrationRankingRequest,
		getAdministrationRankingMetricOption,
		type AdministrationRankingMetricOption,
		type AdministrationRankingSettings
	} from "./administration-ranking";
	import type { MetricPage } from "@lib/api";
	import ChartBar from "@lucide/svelte/icons/chart-bar";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import AdministrationRankingList, { type NormalizedPage } from "./AdministrationRankingList.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Pagination from "@lib/components/ui/Pagination.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import { getStatisticsScopeContext } from "../statistics-scope-context.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";

	type Props = {
		isLoading: boolean;
		settings: AdministrationRankingSettings;
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		evaNumber?: number;
	};
	let { isLoading = $bindable(true), settings: initialSettings, promise: initialPromise, evaNumber }: Props = $props();

	const scopeContext = getStatisticsScopeContext();

	// svelte-ignore state_referenced_locally
	let settings: AdministrationRankingSettings = $state({ ...initialSettings });

	let metricOption = $derived(getAdministrationRankingMetricOption(settings.seriesType));
	let selectedMetricOption: AdministrationRankingMetricOption | undefined = $derived(
		ADMINISTRATION_RANKING_METRIC_OPTIONS.find(
			(option: AdministrationRankingMetricOption) => option.seriesType === settings.seriesType
		)
	);

	let request = $derived(
		createAdministrationRankingRequest(settings, scopeContext.current, evaNumber ? [evaNumber] : undefined)
	);
	// svelte-ignore state_referenced_locally
	let promise: Promise<Awaited<ReturnType<typeof loadMetric>>> = $state(initialPromise);

	const selectMetric = (option: AdministrationRankingMetricOption) => {
		if (option.seriesType === settings.seriesType) return;

		settings = { ...settings, seriesType: option.seriesType, limit: 10, offset: 0 };
	};

	const changePage = (offset: number) => {
		settings = { ...settings, limit: 10, offset };
	};

	const normalizePage = (page: MetricPage | null | undefined): NormalizedPage => {
		const limit = Math.max(1, Number(page?.limit ?? settings.limit));
		const offset = Math.max(0, Number(page?.offset ?? settings.offset));
		const totalItems = Math.max(0, Number(page?.totalItems ?? 0));
		const totalPages = Math.max(0, Number(page?.totalPages ?? Math.ceil(totalItems / limit)));

		return {
			offset,
			limit,
			totalItems,
			totalPages,
			hasMore: Boolean(page?.hasMore ?? offset + limit < totalItems)
		};
	};

	let initialized = false;
	$effect(() => {
		const currentRequest = request;
		if (!currentRequest) return;
		if (!initialized) {
			initialized = true;
			return;
		}

		promise = loadMetric({ request: currentRequest });
	});

	$effect(() => {
		const currentPromise = promise;
		if (!currentPromise) return;

		isLoading = "loading" in currentPromise && Boolean(currentPromise.loading);
	});
</script>

<section class="space-y-4">
	<div class="flex items-center gap-2">
		<ChartBar size={22} class="text-accent" />
		<h2 class="text-2xl font-semibold">Administration Ranking</h2>
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		{#if evaNumber}
			Compare operators by recorded station visits for this station. Arrival/departure only affects station-event charts.
		{:else}
			Compare operators across the global journey dataset for a dedicated time range.
		{/if}
		{metricOption?.rankingDescription ?? "Values are ranked by the selected metric."}
	</p>

	<Card class="gap-y-4">
		<ToggleGroup
			mode="single"
			allowEmpty={false}
			selected={selectedMetricOption}
			keyFn={(option: AdministrationRankingMetricOption) => option.seriesType}
			onselect={(option: AdministrationRankingMetricOption | undefined) => {
				if (option) selectMetric(option);
			}}
			class="gap-1.5 xl:justify-end"
		>
			{#each ADMINISTRATION_RANKING_METRIC_OPTIONS as option (option.seriesType)}
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

		<div class="flex flex-col gap-y-4">
			{#await promise}
				<div class="flex flex-col gap-y-2">
					{#each Array.from({ length: 5 }) as _, index (index)}
						<Skeleton class="h-16 w-full" />
					{/each}
				</div>
			{:then metric}
				{@const page = normalizePage(metric.page)}

				<AdministrationRankingList {metric} {page} />

				<Pagination
					offset={page.offset}
					limit={page.limit}
					totalItems={page.totalItems}
					totalPages={page.totalPages}
					hasMore={page.hasMore}
					onpagechange={changePage}
				/>
			{:catch error}
				<div
					class="bg-secondary/30 border-border flex min-h-96 flex-col items-center justify-center gap-2 rounded-lg border text-center"
				>
					<CircleAlert size={32} class="text-destructive" />
					<p class="text-foreground font-semibold">An error occurred while loading the administration ranking.</p>
					<p class="text-foreground/60 max-w-xl text-sm">{error.message}</p>
				</div>
			{/await}
		</div>
	</Card>
</section>
