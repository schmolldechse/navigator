<script lang="ts">
	import type { StationEventDetailItem, StatisticsMetricResponse } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import TableProperties from "@lucide/svelte/icons/table-properties";
	import DashboardPanel from "./DashboardPanel.svelte";
	import { formatLocalDateTime, formatMetric, transportTypeShortLabel } from "./statistics-dashboard";

	type Props = {
		promise: Promise<StatisticsMetricResponse>;
	};

	let { promise }: Props = $props();

	const getItems = (response: StatisticsMetricResponse): StationEventDetailItem[] =>
		"items" in response.result ? (response.result.items as StationEventDetailItem[]) : [];
</script>

<DashboardPanel
	title="Event details"
	description="Exact stop-event samples with planned stop time and journey start/end."
	icon={TableProperties}
>
	{#await promise}
		<div class="flex min-h-80 flex-col gap-y-3">
			{#each Array.from({ length: 8 }) as _, index (index)}
				<Skeleton class="h-10 w-full" />
			{/each}
		</div>
	{:then response}
		{@const items = getItems(response)}
		{#if items.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No event details available.</p>
			</div>
		{:else}
			<div class="overflow-x-auto">
				<table class="w-full min-w-[56rem] border-separate border-spacing-0 text-sm">
					<thead>
						<tr class="text-foreground/55 text-left text-xs font-bold uppercase">
							<th class="border-border border-b px-3 py-2">Planned</th>
							<th class="border-border border-b px-3 py-2">Type</th>
							<th class="border-border border-b px-3 py-2">Line</th>
							<th class="border-border border-b px-3 py-2">Journey</th>
							<th class="border-border border-b px-3 py-2">Start</th>
							<th class="border-border border-b px-3 py-2">End</th>
							<th class="border-border border-b px-3 py-2 text-right">Delay</th>
							<th class="border-border border-b px-3 py-2">State</th>
						</tr>
					</thead>
					<tbody>
						{#each items as item (item.stopPlaceId + item.journeyId + item.plannedTime)}
							<tr class="hover:bg-secondary/25">
								<td class="border-border/70 border-b px-3 py-2 font-semibold tabular-nums">
									{formatLocalDateTime(item.plannedTime)}
								</td>
								<td class="border-border/70 border-b px-3 py-2">
									{item.scheduleType === "ARRIVAL" ? "Arrival" : "Departure"}
								</td>
								<td class="border-border/70 border-b px-3 py-2">
									<div class="flex items-center gap-2">
										<span class="border-border bg-secondary rounded px-1.5 py-0.5 text-xs font-bold">
											{transportTypeShortLabel(item.transportType)}
										</span>
										<span class="font-semibold">{item.lineName}</span>
									</div>
								</td>
								<td class="border-border/70 border-b px-3 py-2 tabular-nums">{item.journeyNumber}</td>
								<td class="border-border/70 border-b px-3 py-2 tabular-nums">
									{formatLocalDateTime(item.journeyStartTime)}
								</td>
								<td class="border-border/70 border-b px-3 py-2 tabular-nums">
									{formatLocalDateTime(item.journeyEndTime)}
								</td>
								<td class="border-border/70 border-b px-3 py-2 text-right font-semibold tabular-nums">
									{formatMetric(item.eventDelaySeconds, "seconds")}
								</td>
								<td class="border-border/70 border-b px-3 py-2">
									<span
										class={[
											"rounded-full px-2 py-0.5 text-xs font-bold",
											item.stopCancelled ? "bg-destructive/15 text-destructive" : "bg-emerald-500/10 text-emerald-600"
										]}
									>
										{item.stopCancelled ? "Cancelled" : "Served"}
									</span>
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>
		{/if}
	{:catch error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{error.message}</p>
		</div>
	{/await}
</DashboardPanel>
