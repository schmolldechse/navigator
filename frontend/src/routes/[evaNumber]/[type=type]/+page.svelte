<script lang="ts">
	import { invalidateAll } from "$app/navigation";
	import { onDestroy, onMount } from "svelte";
	import { DateTime } from "luxon";
	import { MetaTags } from "svelte-meta-tags";
	import type { PageLoad } from "./$types";

	const { data }: PageLoad = $props();
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

{#each data.journeys as journey}
	{#each journey.connections as connection}
		<p>
			{connection.lineInformation.lineName}
			@{DateTime.fromISO(connection.departure.plannedTime).toFormat("HH:mm")}
		</p>
	{/each}
{/each}
