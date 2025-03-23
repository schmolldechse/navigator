<script lang="ts">
	import { MetaTags } from "svelte-meta-tags";
	import TimetableSearch from "$components/timetable/TimetableSearch.svelte";
	import RoutePlannerSearch from "$components/route-planner/RoutePlannerSearch.svelte";
	import { DateTime } from "luxon";
	import GitHub from "$components/ui/icons/GitHub.svelte";
	import { getContext } from "svelte";

	let currentType = getContext<() => string>("currentType");
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
	{#if currentType() === "timetable"}
		<TimetableSearch />
	{:else if currentType() === "route_planner"}
		<RoutePlannerSearch />
	{/if}
</div>

<footer class="bg-primary-darker sticky bottom-0 flex w-full flex-row items-center justify-between p-4 text-xs md:text-base">
	<span>&copy; {DateTime.now().year} - Schmolldechse & Contributors</span>

	<a class="flex flex-row items-center gap-2" href="https://github.com/schmolldechse/navigator"
		>GitHub <GitHub width="24px" height="24px" /></a
	>
</footer>
