<script lang="ts">
	import type { PageProps } from "./$types";
	import TimetableStationPresentation from "$components/timetable/TimetableStationPresentation.svelte";
	import TimetableEntrySkeleton from "$components/timetable/TimetableEntrySkeleton.svelte";
	import TimetableEntrySeparator from "$components/timetable/TimetableEntrySeparator.svelte";
	import TimetableEntriesEmptyWarning from "$components/timetable/TimetableEntriesEmptyWarning.svelte";
	import TimetableEntry from "$components/timetable/TimetableEntry.svelte";

	let { data }: PageProps = $props();
</script>

<div class="flex flex-col px-2 md:px-0">
	<TimetableStationPresentation station={data.station} />

	<!-- Spacer -->
	<span class="h-[15px]"></span>

	{#await data.timetable}
		{#each Array(5) as _, index}
			{#if index !== 0}
				<TimetableEntrySeparator />
			{/if}
			<TimetableEntrySkeleton />
		{/each}
	{:then timetable}
		{#if timetable.length === 0}
			<TimetableEntriesEmptyWarning />
		{:else}
			{#each timetable as timetableEntry, index}
				{#if index !== 0}
					<TimetableEntrySeparator />
				{/if}
				<TimetableEntry {timetableEntry} />
			{/each}
		{/if}
	{/await}
</div>
