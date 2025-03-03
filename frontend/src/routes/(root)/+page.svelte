<script lang="ts">
	import { MetaTags } from "svelte-meta-tags";
	import TimetableSearch from "$components/timetable/TimetableSearch.svelte";

	let type = $state("timetable");
</script>

<MetaTags
	title="Navigator"
	description="The Navigator for your train journeys."
	openGraph={{
		url: "https://navigator.voldechse.wtf",
		title: "Navigator",
		siteName: "Navigator",
		description: "The Navigator for your train journeys.",
		images: [
			{
				url: "https://navigator.voldechse.wtf/logo.png",
				width: 1024,
				height: 1024,
				alt: "Navigator Logo"
			}
		]
	}}
/>

<div class="flex flex-1 items-center justify-center">
	{#if type === "timetable"}
		<TimetableSearch />
	{:else if type === "route_planner"}
		<p>There is currently no content you're looking for...</p>
	{/if}
</div>
