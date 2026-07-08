<script lang="ts">
	import TransportTypePill from "@lib/components/transport-types/TransportTypePill.svelte";
	import { getTransportTypeIconGroups } from "@lib/components/transport-types/transport-type-icons";
	import Button from "@lib/components/ui/Button.svelte";
	import ArrowLeft from "@lucide/svelte/icons/arrow-left";
	import type { PageProps } from "./$types";

	let { data }: PageProps = $props();

	const station = $derived(data.station);
	const pageTitle = $derived(`${station.name} Statistics - Navigator`);
	const transportGroups = $derived(getTransportTypeIconGroups(station?.transports ?? []));
	const ril100 = $derived(station?.ril100?.filter(Boolean) ?? []);
</script>

<svelte:head>
	<title>{pageTitle}</title>
</svelte:head>

<main class="container mx-auto flex min-h-full flex-col gap-y-6 p-3 sm:p-4 sm:py-8">
	<section class="border-border bg-secondary/10 rounded-xl border-2 p-4 sm:p-5">
		<Button href="/statistics" mode="primary" class="mb-5 inline-flex w-full items-center justify-center gap-2 sm:w-fit">
			<ArrowLeft size={16} />
			Back
		</Button>

		<div class="min-w-0">
			<p class="text-accent text-xs font-bold tracking-wider uppercase">Station statistics</p>
			<h1 class="text-foreground mt-1 text-3xl font-bold text-balance sm:text-4xl">{station.name}</h1>
			<p class="text-foreground/60 mt-2 max-w-3xl text-sm leading-relaxed font-semibold">
				Inspect reliability, punctuality and recorded service patterns for this station.
			</p>

			{#if transportGroups.length || ril100.length}
				<div class="mt-4 flex max-w-3xl flex-wrap gap-1.5">
					{#each transportGroups as group (group.key)}
						<TransportTypePill {group} />
					{/each}

					{#each ril100 as code (code)}
						<span
							class="border-border/80 bg-background/70 text-foreground/65 inline-flex items-center rounded-md border px-1.5 py-0.5 text-[0.65rem] leading-none font-bold"
						>
							RIL100 {code}
						</span>
					{/each}
				</div>
			{/if}
		</div>
	</section>
</main>
