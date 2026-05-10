<script lang="ts">
	import Activity from "@lucide/svelte/icons/activity";
	import Clock from "@lucide/svelte/icons/clock";
	import Database from "@lucide/svelte/icons/database";
	import RefreshCw from "@lucide/svelte/icons/refresh-cw";
	import TrainFront from "@lucide/svelte/icons/train-front";
	import Kpi from "@lib/components/ui/Kpi.svelte";
	import Button from "@lib/components/ui/Button.svelte";
</script>

<svelte:head>
	<title>KPI Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">KPI Test</h1>
		<p class="text-foreground/60 text-sm">Static KPI examples for layout, states, snippets, and trend tones.</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-4 md:grid-cols-2 xl:grid-cols-3">
		<Kpi
			title="Recorded Journeys"
			metric={{ value: "12,847,392" }}
			trend={{ value: "+8.42%", label: "vs previous range", direction: "up" }}
			icon={TrainFront}
		/>

		<Kpi
			title="Punctuality Rate"
			metric={{ value: "91.8", unit: "%" }}
			trend={{ value: "-1.12%", label: "last 24h", direction: "down", tone: "negative" }}
			icon={Activity}
		/>

		<Kpi
			title="Database Size"
			metric={{ value: "42.73", unit: "GB" }}
			trend={{ value: "+684 MB", label: "today", direction: "up", tone: "neutral" }}
			icon={Database}
		/>

		<Kpi
			title="Average Delay"
			metric={{ value: "4.6", unit: "min" }}
			trend={{ value: "0.0 min", label: "unchanged", direction: "neutral" }}
			icon={Clock}
		/>

		<Kpi title="Active RIS IDs" metric={{ value: null }}>
			{#snippet footer()}
				<span class="text-foreground/60">Last successful update: 09:15</span>
			{/snippet}
		</Kpi>

		<Kpi title="Refresh Queue" metric={{ value: 248, unit: "jobs" }}>
			{#snippet actions()}
				<Button mode="tertiary" class="px-2 py-1">
					<RefreshCw size={14} />
				</Button>
			{/snippet}
		</Kpi>

		<Kpi title="Loading Metric" loading />
	</section>
</main>
