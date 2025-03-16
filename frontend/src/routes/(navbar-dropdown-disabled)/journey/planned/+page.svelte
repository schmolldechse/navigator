<script lang="ts">
	import type { PageProps } from "./$types";
	import Route from "$components/route-planner/Route.svelte";
	import { MetaTags } from "svelte-meta-tags";

	let { data }: PageProps = $props();
</script>

<MetaTags
	title={`Route Planner`}
	description={`Plan your journey`}
	openGraph={{
		url: "https://navigator.voldechse.wtf/",
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

<div class="md:max-w-[75%] w-full mx-auto flex flex-1 bg-primary-darker min-h-full rounded-t-2xl">
	{#await data.plannedRoute}
		<p>loading...</p>
	{:then plannedRoute}
		{#each plannedRoute?.journeys as route}
			<Route {route} />
		{/each}
	{/await}
</div>