<script lang="ts">
	import type { ProductType } from "$src/models/product";
	import LongDistance from "$components/ui/transport-types/LongDistance.svelte";
	import Regional from "$components/ui/transport-types/Regional.svelte";
	import Suburban from "$components/ui/transport-types/Suburban.svelte";
	import Bus from "$components/ui/transport-types/Bus.svelte";
	import Ferry from "$components/ui/transport-types/Ferry.svelte";
	import Subway from "$components/ui/transport-types/Subway.svelte";
	import Tram from "$components/ui/transport-types/Tram.svelte";
	import Taxi from "$components/ui/transport-types/Taxi.svelte";

	/**
	 * allowedProducts is a list of all products which are available at the station
	 * selected is a list of all current selected products
	 */
	let {
		allowedProducts,
		selected = $bindable<string[]>(["*"])
	}: {
		allowedProducts: string[];
		selected: string[];
	} = $props();

	const types = $state<ProductType[]>([
		{
			key: "*",
			name: "Every connection",
			component: undefined,
			values: ["*"]
		},
		{
			key: "Longdistance",
			name: "Long distance",
			component: LongDistance,
			values: ["HOCHGESCHWINDIGKEITSZUEGE", "INTERCITYUNDEUROCITYZUEGE"]
		},
		{
			key: "Regional",
			name: "Regional",
			component: Regional,
			values: ["INTERREGIOUNDSCHNELLZUEGE", "NAHVERKEHRSONSTIGEZUEGE"]
		},
		{
			key: "Suburban",
			name: "Suburban",
			component: Suburban,
			values: ["SBAHNEN"]
		},
		{
			key: "Bus",
			name: "Bus",
			component: Bus,
			values: ["BUSSE"]
		},
		{
			key: "Ferry",
			name: "Ferry",
			component: Ferry,
			values: ["SCHIFFE"]
		},
		{
			key: "Subway",
			name: "Subway",
			component: Subway,
			values: ["UBAHN"]
		},
		{
			key: "Tram",
			name: "Tram",
			component: Tram,
			values: ["STRASSENBAHN"]
		},
		{
			key: "Taxi/ Shuttle",
			name: "Taxi/ Shuttle",
			component: Taxi,
			values: ["ANRUFPFLICHTIGEVERKEHRE"]
		}
	]);

	const filteredTypes = $derived(
		types.filter((type) => type.key === "*" || type.values.some((value) => allowedProducts.includes(value)))
	);

	const toggleType = (type: ProductType) => {
		if (type.key === "*") {
			// deselect all other types if "*" is selected
			selected.splice(0, selected.length, "*");
		} else {
			// remove "*" if selected
			const starIndex = selected.indexOf("*");
			if (starIndex > -1) selected.splice(starIndex, 1);

			// check if any value is already selected
			const hasAnyValue = type.values.some((v) => selected.includes(v));

			if (hasAnyValue) {
				// remove all type values
				selected.splice(0, selected.length, ...selected.filter((v) => !type.values.includes(v)));
			} else {
				// add all type values
				selected.push(...type.values);
			}

			// add "*" if no type is selected
			if (selected.length === 0) selected.push("*");
		}
	};
</script>

<div class="scrollbar-none bg-primary-darker flex items-center gap-x-2 overflow-x-auto p-2 md:justify-center">
	{#each filteredTypes as type}
		<button
			class="hover:outline-accent flex shrink-0 cursor-pointer items-center gap-x-2 rounded-full px-4 py-2"
			class:bg-primary-dark={type.values.every((v) => selected.includes(v))}
			onclick={() => toggleType(type)}
		>
			{#if type.component}
				{@const Component = type.component}
				<Component type={type.key === "Subway" || type.key === "Tram" ? "rectangle" : "rounded-corners"} />
			{/if}
			<span>{type.name}</span>
		</button>
	{/each}
</div>
