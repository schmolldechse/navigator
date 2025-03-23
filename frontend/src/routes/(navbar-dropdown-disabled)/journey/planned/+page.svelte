<script lang="ts">
	import type { PageProps } from "./$types";
	import Route from "$components/route-planner/Route.svelte";
	import { MetaTags } from "svelte-meta-tags";
	import ArrowDown from "lucide-svelte/icons/arrow-down";
	import ArrowUp from "lucide-svelte/icons/arrow-up";
	import RouteSkeleton from "$components/route-planner/RouteSkeleton.svelte";
	import RouteRequest from "$components/route-planner/RouteRequest.svelte";

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

<div class="mx-auto flex min-h-full w-full flex-1 flex-col gap-y-4 px-2 md:max-w-[65%] md:px-0">
	<!-- Route Request Info -->
	<RouteRequest stations={data.stations} />

	<!-- Routes -->
	<div class="flex flex-col gap-y-2">
		<div class="flex flex-row items-center gap-x-2">
			<span class="text-lg">Earlier connections</span>
			<ArrowUp color="#ffda0a" />
		</div>
		<div class="space-y-2">
			{#await data.plannedRoute}
				{#each Array(5) as _, i}
					<RouteSkeleton />
				{/each}
			{:then plannedRoute}
				{#each plannedRoute?.journeys as route}
					<Route {route} />
				{/each}
			{/await}
		</div>
		<div class="flex flex-row items-center gap-x-2">
			<span class="text-lg">Later connections</span>
			<ArrowDown color="#ffda0a" />
		</div>
	</div>
</div>
