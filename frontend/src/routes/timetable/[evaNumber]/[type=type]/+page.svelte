<script lang="ts">
	import type { PageProps } from "./$types";
	import TimetableStationPresentation from "$components/timetable/TimetableStationPresentation.svelte";

	let { data }: PageProps = $props();
</script>

<div class="">
	<TimetableStationPresentation station={data.station} />

	{#await data.timetable}
		<p>loading</p>
	{:then timetable}
		<p>{JSON.stringify(timetable)}</p>
	{/await}
</div>
