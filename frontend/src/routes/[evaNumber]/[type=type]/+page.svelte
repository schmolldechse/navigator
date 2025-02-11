<script lang="ts">
	import { invalidateAll } from "$app/navigation";
	import { onDestroy, onMount } from "svelte";
	import { MetaTags } from "svelte-meta-tags";
	import type { PageProps } from "./$types";
	import Clock from "$components/timetable/Clock.svelte";
	import Filter from "$components/timetable/Filter.svelte";
	import type { Connection, Journey } from "$models/connection";
	import { DateTime } from "luxon";

	let { data }: PageProps = $props();

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

<div class="flex flex-col items-center">
	<div class="container mx-auto flex items-center justify-between px-4">
		<span class="text-xl font-semibold md:px-4 md:text-4xl">{data.station.name}</span>
		<Clock />
	</div>

	<Filter allowedProducts={data.station.products ? Object.values(data.station.products) : []} bind:selected={currentFilter} />

	<div class="scrollbar-hidden container mx-auto overflow-y-hidden">
		{#each data.journeys as journey}
			{#if !matchesFilter(journey)}{:else}
				<p>
					{journey?.connections[0]?.lineInformation?.lineName}
					@ {journey?.connections[0]?.departure?.plannedTime
						? DateTime.fromISO(journey?.connections[0]?.departure.plannedTime).toFormat("HH:mm")
						: "N/A"}
				</p>
			{/if}
		{/each}
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
