<script lang="ts">
	import { getContext, setContext } from "svelte";
	import { MetaTags } from "svelte-meta-tags";
	import type { PageProps } from "./$types";
	import Clock from "$components/timetable/Clock.svelte";
	import Filter from "$components/timetable/filter/Filter.svelte";
	import type { Connection, Journey } from "$models/connection";
	import ConnectionComponent from "$components/timetable/ConnectionComponent.svelte";
	import { page } from "$app/state";
	import WingTrain from "$components/timetable/WingTrain.svelte";
	import type { Station } from "$models/station";

	let { data }: PageProps = $props();

	setContext<boolean>("isDeparture", page.params.type === "departures");
	let isDeparture = getContext<boolean>("isDeparture");
	setContext<Station>("station", data.station);
	let station = getContext<Station>("station");

	let currentFilter = $state(["*"]);
	const matchesFilter = (journey: Journey) => {
		if (currentFilter.length === 0) return true;
		if (currentFilter.includes("*")) return true;

		const firstConnection: Connection = journey?.connections[0];
		if (firstConnection === undefined) return false;

		return currentFilter.includes(firstConnection?.lineInformation?.type ?? "");
	};

	const navigate = async () => {
		const type = isDeparture ? "arrivals" : "departures";
		const startDate = page.url.searchParams.get("startDate") ?? new Date().toISOString();

		// TODO: inspect why "goto" does not reload the page, even using invalidateAll
		window.location.href = `/${station?.evaNumber}/${type}?startDate=${encodeURIComponent(startDate)}`;
	}
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

<div class="bg-background sticky top-20 z-10 container mx-auto flex-col w-full px-8 pt-4">
	<div class="flex justify-between">
		<span class="text-xl font-semibold md:text-4xl">{data.station.name}</span>
		<Clock />
	</div>

	<button onclick={navigate}>
		{isDeparture ? "Departures" : "Arrivals"}
	</button>
</div>

<div class="scrollbar-track-sky-300 container mx-auto flex flex-1 flex-col divide-y-2 md:divide-y-0">
	{#each data.journeys as journey}
		{#if !matchesFilter(journey)}{:else if journey.connections.length > 1}
			<WingTrain {journey} />
		{:else}
			<ConnectionComponent connection={journey.connections[0]} renderInformation={true} renderBorder={true} />
		{/if}
	{/each}
</div>

<div class="sticky bottom-0 mt-auto w-full">
	<Filter allowedProducts={data.station.products ? Object.values(data.station.products) : []} bind:selected={currentFilter} />
</div>
