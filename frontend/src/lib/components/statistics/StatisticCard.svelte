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

	const calculateChange = (statistic: MeasuredTimerangeStatistic): { percentage: number; isUp: boolean } => {
		const total = Number(statistic.total);
		const startedWith = Number(statistic.startedWith);
		const changedBy = Number(statistic.changedBy);

		let percentage: number;
		if (startedWith === 0) percentage = total > 0 ? 100 : 0;
		else percentage = (changedBy / startedWith) * 100;

		return {
			percentage: Math.abs(percentage),
			isUp: changedBy >= 0
		};
	};
</script>

<button
	class="group border-muted-foreground/30 hover:border-accent/20 hover:shadow-accent/5 flex cursor-pointer flex-col gap-y-3 rounded-xl border-2 p-6 text-left transition-all duration-300 hover:-translate-y-1 hover:shadow-2xl"
	onclick={async () => {
		const statistic = await statisticPromise;
		onselect?.(statistic);
	}}
>
	<div class="flex items-center justify-between">
		<p class="text-muted-foreground text-xs font-semibold tracking-wider uppercase">{title}</p>

		{#await statisticPromise}
			<div class="bg-muted/20 h-6 w-16 animate-pulse rounded-full"></div>
		{:then statistic}
			{@const trend: { percentage: number, isUp: boolean } = calculateChange(statistic)}
			{@const TrendIcon = trend.isUp ? TrendingUp : TrendingDown}

			<div
				class={[
					"flex items-center gap-x-2 rounded-full border px-3 py-1",
					{ "border-emerald-500/20 bg-emerald-500/10 text-emerald-500": trend.isUp },
					{ "border-rose-500/20 bg-rose-500/10 text-rose-500": !trend.isUp }
				]}
			>
				<TrendIcon class="h-3.5 w-3.5" />
				<span class="text-xs font-black tracking-tight">{trend.percentage.toFixed(2)}%</span>
			</div>
		{/await}
	</div>

	{#await statisticPromise}
		<div class="flex items-baseline gap-2">
			<div class="bg-muted/20 h-10 w-32 animate-pulse rounded-lg"></div>
			<div class="bg-muted/10 h-6 w-12 animate-pulse rounded-lg"></div>
		</div>
	{:then statistic}
		{@const isUnitBytes = statistic.unit === "BYTES"}

		<div class="group-hover:text-accent text-4xl font-bold transition-colors duration-300">
			{isUnitBytes ? formatBytes(Number(statistic.total), true) : statistic.total.toLocaleString("de-DE")}
			<span class="text-muted-foreground ml-1 text-2xl">
				{isUnitBytes ? getUnit(Number(statistic.total), true) : ""}
			</span>
		</div>
	{/await}
</button>
