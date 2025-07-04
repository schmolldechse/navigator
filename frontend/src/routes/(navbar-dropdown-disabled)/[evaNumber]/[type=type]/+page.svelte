<script lang="ts">
	import type { PageProps } from "./$types";
	import Filter from "$components/timetable/filter/Filter.svelte";
	import ConnectionComponent from "$components/timetable/ConnectionComponent.svelte";
	import WingTrain from "$components/timetable/WingTrain.svelte";
	import Loader from "$components/Loader.svelte";
	import WarningNoConnections from "$components/timetable/WarningNoConnections.svelte";
	import type { TimetableEntry } from "$models/models";

	let { data }: PageProps = $props();

	let currentFilter = $state<string[]>(["*"]);
	const matchesFilter = (timetableEntry: TimetableEntry) => {
		if (currentFilter.length === 0 || currentFilter.includes("*")) return true;
		return timetableEntry.entries?.some((connection) =>
			currentFilter.includes(connection.lineInformation.productType ?? "UNKNOWN")
		);
	};

	// returns all products that are used in the Timetable entries
	const allowedProducts = (entries: TimetableEntry[]): string[] => {
		const availableProducts = data.station?.products || [];
		const connectionProducts = entries.flatMap((timetableEntry: TimetableEntry) =>
			timetableEntry.entries.flatMap((single) => single.lineInformation.productType).filter(Boolean)
		);
		return availableProducts.filter((product: string) => connectionProducts.includes(product));
	};
</script>

<!-- TODO: make Connection Skeleton -->
{#await data.timetable}
	<div class="mx-auto flex-1">
		<Loader />
	</div>
{:then timetable}
	<div class="container mx-auto flex flex-1 flex-col divide-y-2 pt-2 md:divide-y-0">
		{#if timetable?.length === 0 || !timetable.some(matchesFilter)}
			<div class="flex min-h-[300px] flex-1 flex-col justify-end md:items-center md:justify-center">
				<WarningNoConnections />
			</div>
		{:else}
			{#each timetable as timetableEntry}
				{#if !matchesFilter(timetableEntry)}{:else if timetableEntry.entries.length > 1}
					<WingTrain {timetableEntry} />
					<!-- TODO: -->
				{:else}
					<ConnectionComponent connection={timetableEntry.entries[0]} renderInformation={true} renderBorder={true} />
					<!-- TODO: -->
				{/if}
			{/each}
		{/if}
	</div>

	<div class="sticky bottom-0 mt-auto w-full">
		<Filter allowedProducts={allowedProducts(timetable)} bind:selected={currentFilter} />
	</div>
{/await}
