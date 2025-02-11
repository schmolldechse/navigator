<script lang="ts">
	import { invalidateAll } from "$app/navigation";
	import { onDestroy, onMount } from "svelte";
	import { DateTime } from "luxon";
	import { MetaTags } from "svelte-meta-tags";
	import type { PageProps } from "./$types";
	import Clock from "$components/timetable/Clock.svelte";
	import Filter from "$components/timetable/Filter.svelte";

	let { data }: PageProps = $props();

	onMount(() => {
		const interval = setInterval(() => invalidateAll(), 30 * 1000);

		onDestroy(() => clearInterval(interval));
	});
</script>

<MetaTags
	title={data.stationName}
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

	<Filter />
</div>

<div class="scrollbar-hidden overflow-y-scroll">
	{#each data.journeys as journey}
		{#each journey.connections as connection}
			<p>
				{connection.lineInformation?.lineName}
				@{connection.departure?.plannedTime
					? DateTime.fromISO(connection.departure.plannedTime).toFormat("HH:mm")
					: "N/A"}
			</p>
		{/each}
	{/each}
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
