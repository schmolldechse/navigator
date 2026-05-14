<script lang="ts">
	import DistributionBar, { type DistributionBarItem } from "@lib/components/ui/DistributionBar.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";

	const risIds: DistributionBarItem[] = [
		{ key: "active", label: "Active", value: 492960, color: "#59a14f" },
		{ key: "inactive", label: "Inactive", value: 23064, color: "rgb(64 64 64)" }
	];

	const transportShare: DistributionBarItem[] = [
		{ key: "ice", label: "ICE", value: 42, color: "#4e79a7" },
		{ key: "regional", label: "Regional", value: 31, color: "#f28e2b" },
		{ key: "s-bahn", label: "S-Bahn", value: 18, color: "#59a14f" },
		{ key: "bus", label: "Bus", value: 9, color: "#e15759" }
	];

	const skewedDistribution: DistributionBarItem[] = [
		{ key: "processed", label: "Processed", value: 9987, color: "#00bc7d" },
		{ key: "queued", label: "Queued", value: 13, color: "#f59e0b" }
	];

	const emptyDistribution: DistributionBarItem[] = [
		{ key: "active", label: "Active", value: 0, color: "#59a14f" },
		{ key: "inactive", label: "Inactive", value: 0, color: "rgb(64 64 64)" }
	];

	const formatCompact = (value: number) =>
		value.toLocaleString(undefined, {
			notation: "compact",
			maximumFractionDigits: 1
		});
</script>

<svelte:head>
	<title>Distribution Bar Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">Distribution Bar Test</h1>
		<p class="text-foreground/60 text-sm">Static examples for distribution states, legends, values, and constrained layouts.</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-4 lg:grid-cols-2">
		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">RIS ID Distribution</h2>
				<p class="text-foreground/60 text-sm">Default legend with percentage labels.</p>
			</div>

			<DistributionBar items={risIds} ariaLabel="RIS ID active and inactive distribution" />
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Transport Share</h2>
				<p class="text-foreground/60 text-sm">Four categories with evenly readable legend columns.</p>
			</div>

			<DistributionBar items={transportShare} ariaLabel="Transport type distribution" legendClass="sm:grid-cols-4" />
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Absolute Values</h2>
				<p class="text-foreground/60 text-sm">Legend can show formatted item values instead of percentages.</p>
			</div>

			<DistributionBar
				items={risIds}
				ariaLabel="RIS ID active and inactive totals"
				showValues
				valueFormatter={(value: number) => value.toLocaleString()}
			/>
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Compact Bar</h2>
				<p class="text-foreground/60 text-sm">Legend can be hidden for very dense contexts.</p>
			</div>

			<DistributionBar items={transportShare} ariaLabel="Compact transport distribution" showLegend={false} barClass="h-2" />
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Skewed Values</h2>
				<p class="text-foreground/60 text-sm">Tiny non-zero segments keep a visible minimum width.</p>
			</div>

			<DistributionBar
				items={skewedDistribution}
				ariaLabel="Processed and queued distribution"
				showValues
				valueFormatter={formatCompact}
			/>
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Empty Values</h2>
				<p class="text-foreground/60 text-sm">Zero totals remain stable without false segment widths.</p>
			</div>

			<DistributionBar items={emptyDistribution} ariaLabel="Empty active and inactive distribution" />
		</Card>

		<Card class="gap-y-4 lg:col-span-2">
			<div>
				<h2 class="text-lg font-semibold">Constrained Width</h2>
				<p class="text-foreground/60 text-sm">Labels truncate while numeric values remain aligned.</p>
			</div>

			<div class="max-w-xs">
				<DistributionBar
					items={[
						{ key: "large", label: "Very long active category", value: 78, color: "#59a14f" },
						{ key: "small", label: "Very long inactive category", value: 22, color: "rgb(64 64 64)" }
					]}
					ariaLabel="Constrained width distribution"
				/>
			</div>
		</Card>
	</section>
</main>
