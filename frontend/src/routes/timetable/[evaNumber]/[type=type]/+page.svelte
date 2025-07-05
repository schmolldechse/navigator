<script lang="ts">
	import type { PageProps } from "./$types";
	import TimetableStationPresentation from "$components/timetable/TimetableStationPresentation.svelte";
	import TimetableEntrySkeleton from "$components/timetable/TimetableEntrySkeleton.svelte";
	import TimetableEntrySeparator from "$components/timetable/TimetableEntrySeparator.svelte";
	import TimetableEntriesEmptyWarning from "$components/timetable/TimetableEntriesEmptyWarning.svelte";
	import TimetableEntry from "$components/timetable/TimetableEntry.svelte";
	import type { GroupedTimetableEntry, Station } from "$models/models";
	import Filter from "$components/timetable/filter/Filter.svelte";

	let { data }: PageProps = $props();

	let currentFilter = $state<string[]>(["*"]);
	const matchesFilter = (timetableEntry: GroupedTimetableEntry) => {
		if (currentFilter.length === 0 || currentFilter.includes("*")) return true;
		return timetableEntry.entries?.some((connection) =>
			currentFilter.includes(connection.lineInformation.productType ?? "UNKNOWN")
		);
	};

	// returns all products that are used in the Timetable entries
	const allowedProducts = (station: Station, entries: GroupedTimetableEntry[]): string[] => {
		const availableProducts = (station.products || []).flatMap((product) => product.toUpperCase());
		const timetableProducts = entries
			.flatMap((timetableEntry: GroupedTimetableEntry) =>
				timetableEntry.entries.flatMap((single) => single.lineInformation.productType).filter(Boolean)
			)
			.map((product) => product.toUpperCase());
		return availableProducts.filter((product: string) => timetableProducts.includes(product.toUpperCase()));
	};
</script>

<div class="flex min-h-screen flex-col px-2 md:px-0">
	<TimetableStationPresentation station={data.station} />

	<!-- Spacer -->
	<span class="h-[15px]"></span>

	{#await Promise.all([data.timetable, data.station])}
		<div class="flex flex-col gap-y-4">
			{#each Array(5) as _, index}
				{#if index !== 0}
					<TimetableEntrySeparator />
				{/if}
				<TimetableEntrySkeleton />
			{/each}
		</div>
	{:then [timetable, station]}
		<div class="flex flex-1 flex-col">
			{#if timetable.length === 0 || !timetable.some(matchesFilter)}
				<TimetableEntriesEmptyWarning />
			{:else}
				<div class="flex flex-col gap-y-2">
					{#each timetable.filter((entry) => matchesFilter(entry)) as timetableEntry, index}
						{#if index !== 0}
							<TimetableEntrySeparator />
						{/if}
						<TimetableEntry {timetableEntry} />
					{/each}
				</div>
			{/if}
		</div>

		<div class="sticky bottom-0 mt-auto w-full">
			<Filter allowedProducts={allowedProducts(station, timetable)} bind:selected={currentFilter} />
		</div>
	{/await}
</div>
