<script lang="ts">
	import NationalExpress from "$components/timetable/filter/icons/NationalExpress.svelte";
	import Regional from "$components/timetable/filter/icons/Regional.svelte";
	import Suburban from "$components/timetable/filter/icons/Suburban.svelte";
	import Subway from "$components/timetable/filter/icons/Subway.svelte";
	import Tram from "$components/timetable/filter/icons/Tram.svelte";
	import Bus from "$components/timetable/filter/icons/Bus.svelte";
	import Ferry from "$components/timetable/filter/icons/Ferry.svelte";
	import type { ProductType } from "$src/models/product";

	let { allowedProducts, onFilterChange }: {
		allowedProducts: string[],
		onFilterChange: (enabledTypes: string[]) => void
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
			component: NationalExpress,
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
			name: "Busses",
			component: Bus,
			values: ["BUSSE"]
		},
		{
			key: "Ferry",
			name: "Ferry",
			component: Ferry,
			values: ["FERRY"]
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
		}
	]);

	const selectedTypes = $state<string[]>(["*"]);
	const filteredTypes = $derived(
		types.filter(type =>
			type.key === "*" ||
			type.values.some(value => allowedProducts.includes(value))
		)
	);

	const toggleType = (type: ProductType) => {
		if (type.key === "*") {
			// deselect other types if selecting "*"
			selectedTypes.splice(0, selectedTypes.length, "*");
		} else {
			// remove "*" if any other type is selected
			const allIndex = selectedTypes.indexOf("*");
			if (allIndex > -1) selectedTypes.splice(allIndex, 1);

			// toggle specific type
			const index = selectedTypes.indexOf(type.key);
			if (index > -1) {
				selectedTypes.splice(index, 1); // deselect if already selected
				if (selectedTypes.length === 0) {
					// default to "*" if no type is selected anymore
					selectedTypes.push("*");
				}
			} else {
				selectedTypes.push(type.key); // select the type
			}
		}

		// map selected keys to their corresponding values
		onFilterChange(selectedTypes.includes("*") ? ["*"] : types.filter(type => selectedTypes.includes(type.key)).flatMap(type => type.values));
	};
</script>

<style>
    .scrollbar-hidden::-webkit-scrollbar {
        display: none;
    }

    .scrollbar-hidden {
        -ms-overflow-style: none;
        scrollbar-width: none;
    }
</style>

<div class="container mx-auto flex items-center md:gap-x-4 bg-primary-darker py-2 overflow-x-auto md:justify-center scrollbar-hidden">
	{#each filteredTypes as type}
		<button
			class="flex items-center gap-x-2 px-4 py-4 rounded-full shrink-0"
			class:bg-primary-dark={selectedTypes.includes(type.key)}
			onclick={() => toggleType(type)}
		>
			{#if type.component}
				{@const Component = type.component}
				<Component />
			{/if}
			<span>{type.name}</span>
		</button>
	{/each}
</div>