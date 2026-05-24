<script lang="ts">
	import { loadMetric } from "@lib/remote/metrics.remote";
	import {
		createLineRankingRequest,
		getLineRankingMetricOption,
		LINE_RANKING_METRIC_OPTIONS,
		type LineRankingMetricOption,
		type LineRankingSettings
	} from "./line-ranking";
	import ChartBar from "@lucide/svelte/icons/chart-bar";
	import Button from "@lib/components/ui/Button.svelte";
	import Input from "@lib/components/ui/Input.svelte";
	import LineRankingList, { type NormalizedPage } from "./LineRankingList.svelte";
	import { getStatisticsScopeContext } from "../statistics-scope-context.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { MetricPage } from "@lib/api";
	import Pagination from "@lib/components/ui/Pagination.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";

	type Props = {
		isLoading: boolean;
		settings: LineRankingSettings;
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
	};
	let { isLoading = $bindable(true), settings: initialSettings, promise: initialPromise }: Props = $props();

	const scopeContext = getStatisticsScopeContext();

	// svelte-ignore state_referenced_locally
	let settings: LineRankingSettings = $state({ ...initialSettings });

	let metricOption = $derived(getLineRankingMetricOption(settings.seriesType));
	let selectedMetricOption: LineRankingMetricOption | undefined = $derived(
		LINE_RANKING_METRIC_OPTIONS.find((option: LineRankingMetricOption) => option.seriesType === settings.seriesType)
	);

	// svelte-ignore state_referenced_locally
	let journeyDescription = $state(settings.journeyDescription);
	// svelte-ignore state_referenced_locally
	let number = $state(settings.number);

	let request = $derived(createLineRankingRequest(settings, scopeContext.current));
	// svelte-ignore state_referenced_locally
	let promise: Promise<Awaited<ReturnType<typeof loadMetric>>> = $state(initialPromise);

	const selectMetric = (option: LineRankingMetricOption) => {
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

	const applyFilters = () => {
		const nextJourneyDescription = journeyDescription.trim();
		const nextNumber = number.trim();
		if (nextJourneyDescription === settings.journeyDescription && nextNumber === settings.number) return;

		settings = { ...settings, journeyDescription: nextJourneyDescription, number: nextNumber, limit: 10, offset: 0 };
		journeyDescription = nextJourneyDescription;
		number = nextNumber;
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
		<h2 class="text-2xl font-semibold">Line Ranking</h2>
	</div>

	<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
		Compare individual lines and routes across the global journey dataset.
		{metricOption?.rankingDescription ?? "Values are ranked by the selected metric."}
	</p>

	<Card class="gap-y-4">
		<ToggleGroup
			mode="single"
			allowEmpty={false}
			selected={selectedMetricOption}
			keyFn={(option: LineRankingMetricOption) => option.seriesType}
			onselect={(option: LineRankingMetricOption | undefined) => {
				if (option) selectMetric(option);
			}}
			class="gap-1.5 xl:justify-end"
		>
			{#each LINE_RANKING_METRIC_OPTIONS as option (option.seriesType)}
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

		<div class="grid gap-2 sm:grid-cols-[minmax(9rem,14rem)_minmax(9rem,14rem)_auto] sm:items-end xl:justify-end">
			<label class="grid gap-1">
				<span class="text-foreground/60 text-xs font-semibold">Journey Description</span>
				<Input type="text" bind:value={journeyDescription} placeholder="RE|S|ICE" maxlength={64} disabled={isLoading} />
			</label>

			<label class="grid gap-1">
				<span class="text-foreground/60 text-xs font-semibold">Number</span>
				<Input type="text" bind:value={number} placeholder="1|8|612" maxlength={64} disabled={isLoading} />
			</label>

			<Button mode="secondary" onclick={applyFilters} disabled={isLoading} aria-label="Apply line ranking filters">
				Apply
			</Button>
		</div>

		<div class="flex flex-col gap-y-4">
			{#await promise}
				<div class="flex flex-col gap-y-2">
					{#each Array.from({ length: 5 }) as _, index (index)}
						<Skeleton class="h-16 w-full" />
					{/each}
				</div>
			{:then metric}
				{@const page = normalizePage(metric.page)}

				<LineRankingList {metric} {page} />

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
					<p class="text-foreground font-semibold">An error occurred while loading the line ranking.</p>
					<p class="text-foreground/60 max-w-xl text-sm">{error.message}</p>
				</div>
			{/await}
		</div>
	</Card>
</section>
