<script lang="ts">
	import { DateTime } from "luxon";
	import type { Station, StationGatheringInfo } from "@lib/api";
	import Button from "@lib/components/ui/Button.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import Activity from "@lucide/svelte/icons/activity";
	import ArrowLeft from "@lucide/svelte/icons/arrow-left";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import RadioTower from "@lucide/svelte/icons/radio-tower";

	type Props = {
		station: Station;
		gatheringPromise: Promise<StationGatheringInfo>;
	};
	let { station, gatheringPromise }: Props = $props();
</script>

<section class="grid gap-4 xl:grid-cols-[minmax(0,1fr)_minmax(18rem,24rem)] xl:items-start">
	<div class="relative">
		<div class="bg-accent absolute top-1 bottom-1 w-0.5 sm:-left-4 sm:w-1"></div>
		<div class="flex flex-col gap-y-4 pl-4 sm:pl-8">
			<Button href="/statistics" mode="tertiary" class="-ml-3 inline-flex w-fit items-center gap-2 text-sm">
				<ArrowLeft size={16} />
				Back to statistics
			</Button>

			<div class="grid gap-2">
				<h1 class="text-3xl font-bold text-balance sm:text-4xl md:text-5xl">{station.name}</h1>
				<p class="text-foreground/65 max-w-3xl text-sm leading-relaxed sm:text-base">
					Station-specific punctuality, delay, cancellation, operator, and line signals for the selected scope.
				</p>
			</div>
		</div>
	</div>

	<Card class="bg-secondary/20 gap-y-3">
		<div class="flex items-center gap-2">
			<RadioTower size={18} class="text-accent" />
			<h2 class="text-base font-semibold">Gathering status</h2>
		</div>

		{#await gatheringPromise}
			<Skeleton class="h-5 w-32" />
			<Skeleton class="h-5 w-full" />
		{:then gathering}
			<div class="grid gap-2 text-sm">
				<div class="flex items-center gap-2">
					<Activity size={16} class={gathering.queryingEnabled ? "text-accent" : "text-foreground/45"} />
					<span class="font-semibold">{gathering.queryingEnabled ? "Querying enabled" : "Querying disabled"}</span>
				</div>

				{#if gathering.lastQueried}
					<p class="text-foreground/60">
						Last queried {DateTime.fromISO(gathering.lastQueried).toLocaleString(DateTime.DATETIME_MED)}
					</p>
				{:else}
					<p class="text-foreground/60">No last query timestamp recorded.</p>
				{/if}
			</div>
		{:catch error}
			<div class="flex items-start gap-2 text-sm">
				<CircleAlert size={18} class="text-destructive mt-0.5 shrink-0" />
				<div>
					<p class="font-semibold">Gathering status unavailable</p>
					<p class="text-foreground/60 text-xs">{error.message}</p>
				</div>
			</div>
		{/await}
	</Card>
</section>
