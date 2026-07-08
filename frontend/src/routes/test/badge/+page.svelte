<script lang="ts">
	import Badge from "@lib/components/ui/Badge.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import CircleCheck from "@lucide/svelte/icons/circle-check";
	import Info from "@lucide/svelte/icons/info";
	import TriangleAlert from "@lucide/svelte/icons/triangle-alert";
</script>

<svelte:head>
	<title>Badge Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">Badge Test</h1>
		<p class="text-foreground/60 text-sm">
			Visual examples for tones, icon composition, wrapping, custom classes and forwarded attributes.
		</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-4 lg:grid-cols-2">
		<Card class="gap-y-5">
			<div>
				<h2 class="text-lg font-semibold">Tones</h2>
				<p class="text-foreground/60 text-sm">The four visual tones supported by the component.</p>
			</div>

			<div class="flex flex-wrap gap-2">
				<Badge>Neutral</Badge>
				<Badge tone="accent">Accent</Badge>
				<Badge tone="positive">Positive</Badge>
				<Badge tone="negative">Negative</Badge>
			</div>
		</Card>

		<Card class="gap-y-5">
			<div>
				<h2 class="text-lg font-semibold">Icons</h2>
				<p class="text-foreground/60 text-sm">Callers compose familiar icons and labels directly.</p>
			</div>

			<div class="flex flex-wrap gap-2">
				<Badge><Info size={14} class="shrink-0" />Recorded sample</Badge>
				<Badge tone="accent"><CalendarDays size={14} class="shrink-0" />Last 30 days</Badge>
				<Badge tone="positive"><CircleCheck size={14} class="shrink-0" />Available</Badge>
				<Badge tone="negative"><TriangleAlert size={14} class="shrink-0" />Disrupted</Badge>
			</div>
		</Card>

		<Card class="gap-y-5">
			<div>
				<h2 class="text-lg font-semibold">Wrapping</h2>
				<p class="text-foreground/60 text-sm">Badge groups wrap while each badge keeps its contents aligned.</p>
			</div>

			<div class="flex max-w-sm flex-wrap gap-2">
				<Badge>18 May 2026 - 31 May 2026</Badge>
				<Badge>High speed</Badge>
				<Badge>Intercity</Badge>
				<Badge>Regional</Badge>
				<Badge>This longer badge remains readable when the available space becomes narrow.</Badge>
			</div>
		</Card>

		<Card class="gap-y-5">
			<div>
				<h2 class="text-lg font-semibold">Customization and attributes</h2>
				<p class="text-foreground/60 text-sm">Classes and native span attributes are forwarded to the rendered element.</p>
			</div>

			<div class="flex flex-wrap gap-2">
				<Badge class="rounded-full! px-3! py-1.5! text-sm!" title="Customized badge">Rounded custom badge</Badge>
				<Badge tone="accent" data-testid="forwarded-badge" aria-label="Badge with forwarded attributes">
					Forwarded attributes
				</Badge>
			</div>
		</Card>
	</section>
</main>
