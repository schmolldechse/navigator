<script lang="ts">
	import type {
		EventMetrics,
		MetricPage,
		StationDirectionItem,
		StationLineRankingItem,
		StatisticsMetricResponse
	} from "@lib/api";
	import Pagination from "@lib/components/ui/Pagination.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import ListOrdered from "@lucide/svelte/icons/list-ordered";
	import { BarChart } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import {
		formatCount,
		formatMetric,
		pageHasMore,
		pageTotalPages,
		pageValue,
		toNumber,
		transportTypeShortLabel
	} from "../shared/statistics-dashboard";

	type RankingMode = "lines" | "directions";

	type RankingRow = {
		id: string;
		label: string;
		subtitle?: string;
		value: number;
		displayValue: string;
		volume: string;
	};

	type Props = {
		title: string;
		description?: string;
		promise: RemoteQuery<StatisticsMetricResponse>;
		mode: RankingMode;
		onpagechange?: (offset: number) => void;
	};

	let { title, description, promise, mode, onpagechange }: Props = $props();

	const eventValue = (metrics: EventMetrics) => toNumber(metrics.delayDebtMinutes) ?? 0;

	const createRows = (response: StatisticsMetricResponse, rankingMode: RankingMode): RankingRow[] => {
		if (!("items" in response.result)) return [];

		if (rankingMode === "directions") {
			return (response.result.items as StationDirectionItem[]).map((item, index) => ({
				id: `${item.directionType}-${item.station.stationEvaNumber}-${index}`,
				label: item.station.stationName ?? "Unnamed station",
				subtitle: item.directionType.toLowerCase(),
				value: eventValue(item.eventMetrics),
				displayValue: formatMetric(item.eventMetrics.delayDebtMinutes, "minutes", true),
				volume: `${formatCount(item.eventMetrics.plannedEvents, true)} events`
			}));
		}

		return (response.result.items as StationLineRankingItem[]).map((item, index) => ({
			id: `${item.line.lineName}-${index}`,
			label: item.line.lineName,
			subtitle: transportTypeShortLabel(item.line.transportType),
			value: eventValue(item.eventMetrics),
			displayValue: formatMetric(item.eventMetrics.delayDebtMinutes, "minutes", true),
			volume: `${formatCount(item.eventMetrics.plannedEvents, true)} events`
		}));
	};

	const getPage = (response: StatisticsMetricResponse): MetricPage | null =>
		"page" in response.result ? (response.result.page as MetricPage) : null;
</script>

<DashboardPanel {title} {description} icon={ListOrdered}>
	{#if promise.loading}
		<div class="flex min-h-96 flex-col gap-y-3">
			<Skeleton class="h-52 w-full" />
			{#each Array.from({ length: 5 }) as _, index (index)}
				<Skeleton class="h-12 w-full" />
			{/each}
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const rows = createRows(promise.current, mode)}
		{@const page = getPage(promise.current)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No ranking rows available.</p>
			</div>
		{:else}
			<div class="flex flex-col gap-4">
				<div class="bg-secondary/20 min-h-56 rounded-lg p-2">
					<BarChart
						data={rows}
						x="value"
						y="label"
						orientation="horizontal"
						yDomain={rows.map((row) => row.label)}
						height={230}
						padding={{ top: 10, right: 18, bottom: 30, left: 120 }}
						tooltipContext={{ mode: "band" }}
						series={[{ key: "value", label: "Delay debt", color: "var(--color-accent)" }]}
					/>
				</div>

				<div class="flex flex-col gap-2">
					{#each rows as row, index (row.id)}
						<div
							class="border-border bg-secondary/10 grid grid-cols-[2rem_minmax(0,1fr)_auto] items-center gap-3 rounded-lg border p-3"
						>
							<div class="text-foreground/45 text-right text-sm font-bold tabular-nums">
								{page ? pageValue(page.offset) + index + 1 : index + 1}
							</div>
							<div class="min-w-0">
								<div class="flex min-w-0 items-baseline gap-2">
									<p class="text-foreground truncate font-semibold">{row.label}</p>
									<p class="text-foreground/45 shrink-0 text-xs font-semibold">{row.volume}</p>
								</div>
								{#if row.subtitle}
									<p class="text-foreground/55 truncate text-xs">{row.subtitle}</p>
								{/if}
							</div>
							<p class="text-foreground text-sm font-bold tabular-nums">{row.displayValue}</p>
						</div>
					{/each}
				</div>

				{#if page && onpagechange}
					<Pagination
						offset={pageValue(page.offset)}
						limit={Math.max(1, pageValue(page.limit))}
						totalItems={pageValue(page.totalItems)}
						totalPages={pageTotalPages(page)}
						hasMore={pageHasMore(page)}
						{onpagechange}
					/>
				{/if}
			</div>
		{/if}
	{/if}
</DashboardPanel>
