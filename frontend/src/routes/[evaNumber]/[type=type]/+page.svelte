<script lang="ts">
	import { invalidateAll } from "$app/navigation";
	import { onDestroy, onMount, setContext } from "svelte";
	import { MetaTags } from "svelte-meta-tags";
	import type { PageProps } from "./$types";
	import Clock from "$components/timetable/Clock.svelte";
	import Filter from "$components/timetable/filter/Filter.svelte";
	import type { Connection, Journey } from "$models/connection";
	import ConnectionComponent from "$components/timetable/ConnectionComponent.svelte";
	import { page } from "$app/state";

	let { data }: PageProps = $props();

	setContext("isDeparture", page.params.type === "departures");

	let currentFilter = $state(["*"]);
	const matchesFilter = (journey: Journey) => {
		if (currentFilter.length === 0) return true;
		if (currentFilter.includes("*")) return true;

		const firstConnection: Connection = journey?.connections[0];
		if (firstConnection === undefined) return false;

		return currentFilter.includes(firstConnection?.lineInformation?.type ?? "");
	};

	onMount(() => {
		const interval = setInterval(() => invalidateAll(), 30 * 1000);
		onDestroy(() => clearInterval(interval));
	});
</script>

<MetaTags
        title={data.station.name}
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

<div class="flex flex-col items-center pb-safe overflow-auto scrollbar-hidden">
    <div class="container mx-auto flex items-center justify-between px-4">
        <span class="text-xl font-semibold md:px-4 md:text-4xl">{data.station.name}</span>
        <Clock/>
    </div>

	<div class="scrollbar-hidden overflow-y-scroll container mx-auto flex flex-col">
		{#each data.journeys as journey}
			{#if !matchesFilter(journey)}{:else}
				<ConnectionComponent connection={journey.connections[0]} />
			{/if}
		{/each}
	</div>

	<div class="self-end w-full bottom-0">
		<Filter allowedProducts={data.station.products ? Object.values(data.station.products) : []}
				bind:selected={currentFilter} />
	</div>
</div>

<style>
    .scrollbar-hidden::-webkit-scrollbar {
        display: none;
    }

    .scrollbar-hidden {
        -ms-overflow-style: none;
        scrollbar-width: none;
    }
</style>
