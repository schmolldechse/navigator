<script lang="ts">
	import { MetaTags } from "svelte-meta-tags";
	import { getContext } from "svelte";
	import TimeTable from "$components/TimeTable.svelte";

	const type: () => string = getContext("type");
</script>

<MetaTags
	title="NAVIGATOR"
	description="The Navigator for your train journeys."
	openGraph={{
		url: "https://navigator.voldechse.wtf",
		title: "NAVIGATOR",
		siteName: "NAVIGATOR",
		description: "The Navigator for your train journeys.",
		images: [
			{
				url: "https://navigator.voldechse.wtf/logo.png",
				width: 1024,
				height: 1024,
				alt: "NAVIGATOR Logo"
			}
		]
	}} />

<div class="flex items-center justify-center">

	<div class="px-4 py-7 md:px-10 md:w-[40%] flex flex-col space-y-4 bg-zinc-700 text-white">
		{#if type() === "timetable"}
			<TimeTable />
		{/if}
	</div>

</div>
