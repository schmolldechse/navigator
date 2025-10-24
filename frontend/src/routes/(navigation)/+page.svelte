<script lang="ts">
</script>

<svelte:head>
	<title>Navigator</title>
</svelte:head>

<div class="container mx-auto px-4 py-8 md:py-12">
	<!-- Hero Section -->
	<section class="mb-12 md:mb-16">
		<h1 class="text-text mb-6 text-balance text-4xl font-bold leading-tight md:text-6xl">
			<span class="text-accent font-bold">Visualizing</span> Public Transport Performance
		</h1>
		<p class="text-muted-foreground max-w-3xl text-lg leading-relaxed md:text-xl">
			Clear insights. <span class="text-text font-semibold">Transparency</span> for Germany's Public Transport.
		</p>
	</section>

	<!-- About the Project -->
	<section class="mb-12 md:mb-16">
		<div class="border-muted rounded-lg border p-6 md:p-8">
			<h2 class="text-text mb-4 text-xl font-bold md:text-2xl">About the Project</h2>
			<div class="text-text space-y-4 text-sm leading-relaxed md:text-base">
				<p class="about">
					The Deutsche Bahn does not provide <span>clear, accessible</span> or <span>on-demand statistics</span>
					regarding public transport performance. Users cannot look up performance data for a specific route within a
					<span>user-defined timeframe</span>, as only high-level annual reports are available.
				</p>
				<p class="about">
					Navigator addresses this problem by <span>actively collecting and analyzing journey data</span> directly
					received from stations. Starting in March 2025 with the Stuttgart area, we process journey information to
					provide insights into <span>delays, cancellations</span> and overall <span>service quality</span>.
				</p>
				<p class="about">
					Our dataset is <span>continuously expanding</span> to include more stations, working towards
					<span>nationwide coverage</span>
					across Germany. The ultimate goal is to make public transport data
					<span>accessible and understandable</span>, bringing <span>transparency</span> to Germany's rail network for
					everyone.
				</p>
			</div>
		</div>
	</section>
</div>

<style>
	:global(p.about span) {
		color: var(--color-accent);
		font-weight: 600;
	}
</style>
