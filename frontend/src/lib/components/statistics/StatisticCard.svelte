<script lang="ts">
	import TrendingUp from "@lucide/svelte/icons/trending-up";
	import TrendingDown from "@lucide/svelte/icons/trending-down";
	import { formatBytes, getUnit } from "$lib/util/bytes";
	import type { MeasuredTimerangeStatistic } from "$lib/api/types.gen";

	interface Props {
		title: string;
		statisticPromise: Promise<MeasuredTimerangeStatistic>;
		onselect?: (statistic: MeasuredTimerangeStatistic) => void;
	}

	let { title, statisticPromise, onselect }: Props = $props();

	let statistic: MeasuredTimerangeStatistic | null = $state(null);
	$effect(() => {
		statisticPromise.then((stat: MeasuredTimerangeStatistic) => (statistic = stat));
	});
</script>

<button
	class="group border-muted-foreground/30 hover:border-accent/20 hover:shadow-accent/5 flex cursor-pointer flex-col gap-y-3 rounded-xl border-2 p-6 text-left transition-all duration-300 hover:-translate-y-1 hover:shadow-2xl"
	onclick={() => {
		if (!statistic) return;
		onselect?.(statistic);
	}}
>
	<div class="flex items-center justify-between">
		<p class="text-muted-foreground text-xs font-semibold tracking-wider uppercase">{title}</p>

		{#if statistic === null}
			<div class="w-32 animate-pulse rounded-md bg-white/10"></div>
		{:else}
			{@const isTrendUp = Number(statistic.changedBy) >= 0}
			{@const changePercentage = (Number(statistic.changedBy) / (Number(statistic.total) - Number(statistic.changedBy))) * 100}
			{@const TrendIcon = isTrendUp ? TrendingUp : TrendingDown}

			<div
				class={[
					"flex items-center gap-x-2 rounded-full border px-3 py-1",
					{ "border-emerald-500/20 bg-emerald-500/10 text-emerald-500": isTrendUp },
					{ "border-rose-500/20 bg-rose-500/10 text-rose-500": !isTrendUp }
				]}
			>
				<TrendIcon class="h-5 w-5" />
				<span class="text-xs font-black tracking-tight">{changePercentage.toFixed(1)}%</span>
			</div>
		{/if}
	</div>

	{#if statistic === null}
		<div class="bg-muted/30 h-10 w-50 animate-pulse rounded-xl"></div>
	{:else}
		{@const isUnitBytes = statistic.unit === "BYTES"}

		<div class="group-hover:text-accent text-4xl font-bold transition-colors duration-300">
			{isUnitBytes ? formatBytes(Number(statistic.total), true) : statistic.total.toLocaleString("de-DE")}
			<span class="text-muted-foreground ml-1 text-2xl">
				{isUnitBytes ? getUnit(Number(statistic.total), true) : ""}
			</span>
		</div>
	{/if}
</button>
