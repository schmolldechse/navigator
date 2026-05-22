<script module lang="ts">
	import type { AdministrationMetricSubject, MetricSample, MetricSeries } from "@lib/api";

	type AdministrationRankingRow = {
		key: string;
		rank: number;
		administration: AdministrationMetricSubject;
		value: number;
		unit: MetricSeries["unit"];
		sample?: MetricSample | null;
	};

	type NormalizedPage = {
		offset: number;
		limit: number;
		totalItems: number;
		totalPages: number;
		hasMore: boolean;
	};

	export type { AdministrationRankingRow, NormalizedPage };
</script>

<script lang="ts">
	import type { ClassValue } from "svelte/elements";
	import type { MetricPage } from "@lib/api";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import Info from "@lucide/svelte/icons/info";
	import Trophy from "@lucide/svelte/icons/trophy";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Pagination from "@lib/components/ui/Pagination.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import * as Tooltip from "@lib/components/ui/tooltip/index";
	import { loadMetric } from "@lib/remote/metrics.remote";
	import { formatAdministrationRankingValue, getAdministrationRankingMetricOption } from "./administration-ranking";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		onpagechange: (offset: number) => void;
		class?: ClassValue;
	};
	let { promise, onpagechange, class: className }: Props = $props();

	const normalizePage = (page: MetricPage | null | undefined): NormalizedPage => {
		const limit = Math.max(1, Number(page?.limit ?? 10));
		const offset = Math.max(0, Number(page?.offset ?? 0));
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

	const createRows = (metric: MetricSeries, page: NormalizedPage): AdministrationRankingRow[] => {
		const rows: AdministrationRankingRow[] = [];

		for (const dataPoint of metric.dataPoints) {
			if (!("administration" in dataPoint) || !dataPoint.administration) continue;

			const index = rows.length;
			const administration = dataPoint.administration;

			rows.push({
				key: `${administration.operatorCode}-${administration.administrationId}`,
				rank: page.offset + index + 1,
				administration,
				value: Number(dataPoint.value),
				unit: metric.unit,
				sample: dataPoint.sample
			});
		}

		return rows;
	};
</script>

<Card class={["gap-y-4", className]}>
	<Card class="bg-background/85! pointer-events-none z-10 w-fit px-3! py-2! shadow-sm backdrop-blur-md">
		<p class="text-foreground/60 text-xs font-medium">Ranking metric</p>
		{#await promise}
			<Skeleton class="h-6 w-32" />
		{:then data}
			<h2 class="text-foreground text-sm font-semibold sm:text-base">
				{getAdministrationRankingMetricOption(data.seriesType)?.label ?? "???"}
			</h2>
		{/await}
	</Card>

	<div class="flex flex-col gap-y-4">
		{#await promise}
			<div class="flex flex-col gap-y-2">
				{#each Array.from({ length: 5 }) as _}
					<Skeleton class="h-16 w-full" />
				{/each}
			</div>
		{:then metric}
			{@const page = normalizePage(metric.page)}
			{@const rows = createRows(metric, page)}

			{#if rows.length === 0}
				<div
					class="bg-secondary/30 border-border flex min-h-96 flex-col items-center justify-center rounded-lg border text-center"
				>
					<p class="text-foreground font-semibold">No ranking data available</p>
					<p class="text-foreground/60 max-w-sm text-sm">Try a wider time range or another ranking metric.</p>
				</div>
			{:else}
				{@const maxValue = Math.max(0, ...rows.map((row) => row.value))}
				{@const metricOption = getAdministrationRankingMetricOption(metric.seriesType)}

				<div class="grid gap-y-2">
					<div
						class="text-foreground/45 grid grid-cols-[4rem_minmax(0,1fr)_minmax(6rem,8rem)] gap-3 px-3 text-xs font-semibold tracking-wide uppercase"
					>
						<span class="text-right">Place</span>
						<span>Administration / Operator</span>
						<span class="text-right">Value</span>
					</div>

					{#each rows as row (row.key)}
						{@const administration = row.administration}
						{@const formatted = formatAdministrationRankingValue(row.value, row.unit)}
						{@const percentage = maxValue === 0 ? 0 : (Math.max(0, row.value) / maxValue) * 100}

						<article
							class="border-border bg-secondary/20 hover:bg-secondary/35 grid min-h-16 grid-cols-[4rem_minmax(0,1fr)_minmax(6rem,8rem)] items-center gap-3 rounded-lg border p-3 transition-colors"
						>
							<div class="text-foreground/60 flex items-center justify-end gap-x-2 text-sm font-semibold tabular-nums">
								{#if row.rank <= 3}
									<Trophy size={16} class="text-accent" />
								{/if}
								<span>#{row.rank}</span>
							</div>

							<div class="flex min-w-0 flex-col gap-y-2">
								<div class="flex min-w-0 flex-wrap items-baseline gap-x-2 gap-y-1">
									<span class="text-foreground truncate font-semibold">{administration.operatorName}</span>
									<span class="bg-background text-foreground/60 rounded-md px-1.5 py-0.5 text-xs font-medium">
										{administration.operatorCode}
									</span>
								</div>

								<div
									class="bg-background h-2 overflow-hidden rounded-full"
									role="img"
									aria-label={`${administration.operatorName} relative value ${percentage.toLocaleString(undefined, { maximumFractionDigits: 0 })}%`}
								>
									<div
										class="bg-accent h-full rounded-full transition-[width] duration-300"
										style:width={`${percentage}%`}
									></div>
								</div>
							</div>

							<div class="flex items-center justify-end gap-x-2">
								<div class="text-foreground text-right font-bold tabular-nums">
									{formatted.value}
									{#if formatted.unit}
										<span class="text-foreground/60 ml-1 text-xs font-semibold">{formatted.unit}</span>
									{/if}
								</div>

								{#if row.sample && metricOption?.sampleLabels}
									<Tooltip.Root delay={100}>
										<Tooltip.Trigger>
											<button
												type="button"
												class="text-foreground/45 hover:bg-accent/15 hover:text-accent focus-visible:outline-accent flex size-7 items-center justify-center rounded-lg transition-colors focus-visible:outline-2 focus-visible:outline-offset-2"
												aria-label={`Show sample for ${administration.operatorName}`}
											>
												<Info size={15} />
											</button>
										</Tooltip.Trigger>

										<Tooltip.Content position="left" class="w-64">
											<div class="grid gap-y-1">
												<p class="text-foreground text-sm font-semibold">Sample basis</p>
												<p class="text-foreground/60 text-xs leading-relaxed">
													{metricOption.sampleLabels.numerator}: {Number(row.sample.numerator).toLocaleString()}.
													{metricOption.sampleLabels.denominator}: {Number(row.sample.denominator).toLocaleString()}.
												</p>
											</div>
										</Tooltip.Content>
									</Tooltip.Root>
								{/if}
							</div>
						</article>
					{/each}
				</div>
			{/if}

			<Pagination
				offset={page.offset}
				limit={page.limit}
				totalItems={page.totalItems}
				totalPages={page.totalPages}
				hasMore={page.hasMore}
				{onpagechange}
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
