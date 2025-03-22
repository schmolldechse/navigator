<script lang="ts">
	import type { PageProps } from "./$types";
	import Route from "$components/route-planner/Route.svelte";
	import { MetaTags } from "svelte-meta-tags";
	import { ArrowDown, ArrowUp } from "lucide-svelte";

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

<div class="md:max-w-[65%] w-full px-2 md:px-0 mx-auto flex flex-1 min-h-full flex-col">
	{#await data.plannedRoute}
		<p>loading...</p>
	{:then plannedRoute}
		<div class="flex flex-col gap-y-2">
			<div class="flex flex-row gap-x-2 items-center">
				<span class="text-lg">Earlier connections</span>
				<ArrowUp color="#ffda0a" />
			</div>
			<div class="space-y-2">
				{#each plannedRoute?.journeys as route}
					<Route {route} />
				{/each}
			</div>
			<div class="flex flex-row gap-x-2 items-center">
				<span class="text-lg">Later connections</span>
				<ArrowDown color="#ffda0a" />
			</div>
		</div>
	{/await}
</div>