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
		statisticPromise.then((stat: MeasuredTimerangeStatistic) => statistic = stat)
	});
</script>

<button
	class="group gap-y-3 flex flex-col text-left cursor-pointer rounded-xl border-2 border-muted-foreground/30 p-6 transition-all duration-300 hover:border-accent/20 hover:shadow-2xl hover:shadow-accent/5 hover:-translate-y-1"
	onclick={() => {
		if (!statistic) return;
		onselect?.(statistic);
	}}
>
	<div class="flex justify-between items-center">
		<p class="uppercase text-xs font-semibold tracking-wider text-muted-foreground">{title}</p>
		
		{#if statistic === null}
			<div class="w-32 animate-pulse rounded-md bg-white/10"></div>
		{:else} 
			{@const isTrendUp = Number(statistic.changedBy) >= 0}
			{@const changePercentage = (Number(statistic.changedBy) / (Number(statistic.total) - Number(statistic.changedBy))) * 100}
			{@const TrendIcon = isTrendUp ? TrendingUp : TrendingDown}

			<div 
				class={[
					"rounded-full px-3 py-1 border flex items-center gap-x-2", 
					{ "border-emerald-500/20 bg-emerald-500/10 text-emerald-500": isTrendUp },
					{ "border-rose-500/20 bg-rose-500/10 text-rose-500": !isTrendUp }
				]}
			>
				<TrendIcon class="h-5 w-5" />
				<span class="font-black tracking-tight text-xs">{changePercentage.toFixed(1)}%</span>
			</div>
		{/if}
	</div>

	{#if statistic === null}
		<div class="w-50 h-10 rounded-xl bg-muted/30 animate-pulse"></div>
	{:else}
		{@const isUnitBytes = statistic.unit === "BYTES"}

		<div class="text-4xl font-bold transition-colors duration-300 group-hover:text-accent">
			{isUnitBytes ? formatBytes(Number(statistic.total), true) : statistic.total.toLocaleString("de-DE")}
			<span class="ml-1 text-2xl text-muted-foreground">
				{isUnitBytes ? getUnit(Number(statistic.total), true) : ""}
			</span>
		</div>
	{/if}
</button>