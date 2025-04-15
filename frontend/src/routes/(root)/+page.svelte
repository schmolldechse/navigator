<script lang="ts">
	import { MetaTags } from "svelte-meta-tags";
	import Timetable from "./Timetable.svelte";
	import RoutePlanner from "./RoutePlanner.svelte";
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
		<Timetable />
	{:else if currentType() === "route_planner"}
		<RoutePlanner />
	{/if}
</div>
