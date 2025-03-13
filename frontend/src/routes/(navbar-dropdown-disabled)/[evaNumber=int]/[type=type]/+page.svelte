<script lang="ts">
	import type { PageProps } from "./$types";
	import Filter from "$components/timetable/filter/Filter.svelte";
	import type { Connection, Journey } from "$models/connection";
	import ConnectionComponent from "$components/timetable/ConnectionComponent.svelte";
	import WingTrain from "$components/timetable/WingTrain.svelte";
	import Loader from "$components/Loader.svelte";

	let { data }: PageProps = $props();

	let currentFilter = $state(["*"]);

	const matchesFilter = (journey: Journey) => {
		if (currentFilter.length === 0) return true;
		if (currentFilter.includes("*")) return true;

		const firstConnection: Connection = journey?.connections[0];
		if (firstConnection === undefined) return false;

		return currentFilter.includes(firstConnection?.lineInformation?.type ?? "");
	};

	const allowedProducts = (journeys: Journey[] | undefined): string[] => {
		const products = data.station?.products || [];

		const types = journeys!
			.flatMap((journey) => journey.connections.map((conn) => conn.lineInformation?.type))
			.filter(Boolean);

		return products.filter((product) => types.includes(product));
	};
</script>

{#await data.journeys}
	<div class="mx-auto flex-1">
		<Loader />
	</div>
{:then journeys}
	<div class="scrollbar-track-sky-300 container mx-auto flex flex-1 flex-col divide-y-2 md:divide-y-0">
		{#each journeys as journey}
			{#if !matchesFilter(journey)}{:else if journey.connections.length > 1}
				<WingTrain {journey} />
			{:else}
				<ConnectionComponent connection={journey.connections[0]} renderInformation={true} renderBorder={true} />
			{/if}
		{/each}
	</div>

	<div class="sticky bottom-0 mt-auto w-full">
		<Filter allowedProducts={allowedProducts(journeys)} bind:selected={currentFilter} />
	</div>
{/await}
