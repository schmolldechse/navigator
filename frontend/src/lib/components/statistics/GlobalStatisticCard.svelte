<script lang="ts">
	import type { MeasuredTimeframeStatisticDTO } from "$lib/models/MeasuredTimeframeStatisticDTO";
	import TrendingUp from "@lucide/svelte/icons/trending-up";
	import TrendingDown from "@lucide/svelte/icons/trending-down";
	import { DateTime } from "luxon";
	import { formatBytes, getUnit, isUnitBytes } from "$lib/util/bytes";

	interface Props {
		title: string;
		fetch: (params: { start: DateTime; end: DateTime }) => Promise<MeasuredTimeframeStatisticDTO>;
		onclick?: () => void;
		onmeasured?: (data: MeasuredTimeframeStatisticDTO) => void;
		timerange?: { start: DateTime; end: DateTime };
	}

	let { title, fetch, onclick, onmeasured, timerange = $bindable() }: Props = $props();
	let measuredStatistic: MeasuredTimeframeStatisticDTO | null = $state(null);

	$effect(() => {
		if (!timerange) return;

		let dataPromise: Promise<MeasuredTimeframeStatisticDTO> = fetch({
			start: timerange.start,
			end: timerange.end
		});
		dataPromise.then((data) => {
			measuredStatistic = data;
			if (onmeasured) onmeasured(measuredStatistic);
		});
	});

	let dataPromise: Promise<MeasuredTimeframeStatisticDTO> = fetch({
		start: DateTime.now().minus({ days: 1 }),
		end: DateTime.now()
	});
	dataPromise.then((data) => {
		measuredStatistic = data;
		if (onmeasured) onmeasured(measuredStatistic);
	});
</script>

<button
	class="group relative cursor-pointer overflow-hidden rounded-xl border-2 border-muted-foreground/30 p-6 text-left transition-all duration-300 hover:border-accent/60"
	onclick={() => {
		if (onmeasured && measuredStatistic) onmeasured($state.snapshot(measuredStatistic));
		onclick?.();
	}}
>
	<div class="space-y-3">
		<p class="mb-4 text-xs font-semibold tracking-wider text-muted-foreground uppercase">{title}</p>

		{#await dataPromise}
			<div class="h-12 w-32 animate-pulse rounded-md bg-muted-foreground/20"></div>
			<div class="h-4 w-40 animate-pulse rounded-md bg-muted-foreground/20"></div>
		{:then data}
			{@const isTrendUp = data.change >= 0}
			{@const changePercentage = (data.change / (data.total - data.change)) * 100}

			{@const duration = DateTime.fromISO(data.timeframe.end).diff(DateTime.fromISO(data.timeframe.start), ["days"]).days}

			<div class="mb-3 text-4xl font-bold transition-colors duration-300 group-hover:text-accent">
				{isUnitBytes(data.unit) ? formatBytes(data.total, true) : data.total.toLocaleString("de-DE")}
				<span class="ml-1 text-2xl text-muted-foreground">
					{isUnitBytes(data.unit) ? getUnit(data.total, true) : ""}
				</span>
			</div>

			<div
				class={[
					"flex items-center gap-1.5 text-sm font-semibold",
					{ "text-accent": isTrendUp },
					{ "text-red-500": !isTrendUp }
				]}
			>
				{#if isTrendUp}
					<TrendingUp class="h-4 w-4" />
				{:else}
					<TrendingDown class="h-4 w-4" />
				{/if}

				<span>
					{changePercentage.toFixed(1)}% {duration <= 1
						? `in last ${duration * 24}h`
						: `in last ${Math.round(duration)} days`}</span
				>
			</div>
		{/await}
	</div>
</button>
