<script lang="ts">
	import { MetaTags } from "svelte-meta-tags";
	import { getContext } from "svelte";
	import TimeTable from "$components/timetable/TimeTable.svelte";

	const type: () => string = getContext("type");
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
	}} />

<div class="flex items-center justify-center">

		{#if type() === "timetable"}
			<TimeTable />
		{/if}

</div>
