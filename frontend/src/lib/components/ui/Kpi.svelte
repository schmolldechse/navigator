<script module lang="ts">
	type KpiMetric = {
		value: string | number | null;
		unit?: string;
	};

	type KpiTrend = {
		value: string | number;
		label?: string;
		direction?: "up" | "down" | "neutral";
		tone?: "positive" | "negative" | "neutral";
	};

	export type { KpiMetric, KpiTrend };
</script>

<script lang="ts">
	import TrendingDown from "@lucide/svelte/icons/trending-down";
	import TrendingUp from "@lucide/svelte/icons/trending-up";
	import Minus from "@lucide/svelte/icons/minus";
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import Card from "./card/Card.svelte";
	import Skeleton from "./Skeleton.svelte";
	import type { LucideIcon } from "@lucide/svelte";

	type Props = {
		title: string;
		metric?: KpiMetric;
		trend?: KpiTrend;
		loading?: boolean;
		class?: ClassValue;
		icon?: LucideIcon;
		actions?: Snippet;
		footer?: Snippet;
	};

	let { title, metric, trend, loading = false, class: className, icon: Icon, actions, footer }: Props = $props();

	const getTrendTone = (trend: KpiTrend): "positive" | "negative" | "neutral" => {
		if (trend.tone) return trend.tone;
		if (trend.direction === "up") return "positive";
		if (trend.direction === "down") return "negative";
		return "neutral";
	};

	const formatValue = (value: string | number | null | undefined): string | number => {
		if (value === null || value === undefined || value === "") return "-";
		return value;
	};
</script>

<Card class={["gap-y-3", className]}>
	<!-- Header -->
	<div class="flex items-center justify-between gap-x-3">
		<div class="flex items-center justify-center gap-x-2">
			{#if Icon}
				<Icon class="text-foreground/60 shrink-0" size={18} />
			{/if}

			<p class="text-foreground/60 truncate text-sm font-semibold tracking-wider uppercase">{title}</p>
		</div>

		{@render actions?.()}
	</div>

	{#if loading}
		<div class="flex flex-col gap-y-3">
			<Skeleton class="h-9 w-2/3 max-w-56" />
			<Skeleton class="h-4 w-1/2 max-w-40" />
		</div>
	{:else}
		{#if metric}
			<div class="flex flex-wrap items-baseline gap-x-2">
				<p class="text-foreground text-3xl leading-none font-bold">{formatValue(metric.value)}</p>

				{#if metric.unit}
					<span class="text-foreground/60 text-sm font-semibold">{metric.unit}</span>
				{/if}
			</div>
		{/if}

		{#if trend}
			{@const tone = getTrendTone(trend)}
			<div
				class={[
					"flex w-fit items-center gap-x-1.5 rounded-full border px-2 py-0.5 text-xs font-semibold",
					tone === "positive" && "border-emerald-400/30 bg-emerald-500/10 text-emerald-500",
					tone === "negative" && "border-rose-400/30 bg-rose-500/10 text-rose-500",
					tone === "neutral" && "border-border bg-secondary text-secondary-foreground"
				]}
			>
				{#if trend.direction === "up"}
					<TrendingUp size={14} />
				{:else if trend.direction === "down"}
					<TrendingDown size={14} />
				{:else if trend.direction === "neutral"}
					<Minus size={14} />
				{/if}

				<span>{trend.value}</span>

				{#if trend.label}
					<span class="opacity-80">{trend.label}</span>
				{/if}
			</div>
		{/if}

		{@render footer?.()}
	{/if}
</Card>
