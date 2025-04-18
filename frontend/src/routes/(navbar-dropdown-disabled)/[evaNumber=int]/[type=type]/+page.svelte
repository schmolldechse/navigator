<script lang="ts">
	import type { PageProps } from "./$types";
	import Filter from "$components/timetable/filter/Filter.svelte";
	import type { Journey } from "$models/connection";
	import ConnectionComponent from "$components/timetable/ConnectionComponent.svelte";
	import WingTrain from "$components/timetable/WingTrain.svelte";
	import Loader from "$components/Loader.svelte";
	import WarningNoConnections from "$components/timetable/WarningNoConnections.svelte";

	let { data }: PageProps = $props();

	let currentFilter = $state<string[]>(["*"]);
	const matchesFilter = (journey: Journey) => {
		if (currentFilter.length === 0 || currentFilter.includes("*")) return true;
		return journey?.connections?.some(connection => currentFilter.includes(connection?.lineInformation?.type ?? "UNKNOWN"));
	};

	// returns all products that are used in the Connections
	const allowedProducts = (journeys: Journey[]): string[] => {
		const availableProducts = data.station?.products || [];
		const connectionProducts = journeys.flatMap(journey => journey.connections
			.flatMap(connection => connection.lineInformation?.type)
			.filter(Boolean)
		);
		return availableProducts.filter((product: string) => connectionProducts.includes(product));
	};
</script>

<!-- TODO: make Connection Skeleton -->
{#await data.journeys}
	<div class="mx-auto flex-1">
		<Loader />
	</div>
{:then journeys}
	<div class="container mx-auto flex flex-1 flex-col pt-2 divide-y-2 md:divide-y-0">
		{#if journeys?.length === 0 || !journeys.some(matchesFilter)}
			<div class="flex-1 flex flex-col justify-end md:justify-center md:items-center min-h-[300px]">
				<WarningNoConnections />
			</div>
		{:else}
			{#each journeys as journey}
				{#if !matchesFilter(journey)}{:else if journey.connections.length > 1}
					<WingTrain {journey} />
				{:else}
					<ConnectionComponent connection={journey.connections[0]} renderInformation={true}
										 renderBorder={true} />
				{/if}
			{/each}
		{/if}
	</div>

	<div class="sticky bottom-0 mt-auto w-full">
		<Filter allowedProducts={allowedProducts(journeys)} bind:selected={currentFilter} />
	</div>
{/await}
