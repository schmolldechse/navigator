<script lang="ts">
	import LongDistance from "$components/ui/transport-types/LongDistance.svelte";
	import { type Component, onMount } from "svelte";
	import { X } from "lucide-svelte";
	import Regional from "$components/ui/transport-types/Regional.svelte";
	import Suburban from "$components/ui/transport-types/Suburban.svelte";
	import Bus from "$components/ui/transport-types/Bus.svelte";
	import Ferry from "$components/ui/transport-types/Ferry.svelte";
	import Subway from "$components/ui/transport-types/Subway.svelte";
	import Tram from "$components/ui/transport-types/Tram.svelte";
	import Switch from "$components/ui/controls/Switch.svelte";

	export interface Product {
		key: string;
		display: string;
		possibilities: string[];
		selected: boolean;
		icon?: {
			component: Component;
			type: string;
		};
	}

	let {
		disabledProducts = $bindable<string[]>([])
	}: {
		disabledProducts: string[];
	} = $props();
	let dropdownOpen = $state<boolean>(false);

	let availableProducts = $state<Product[]>([
		{
			key: "long-distance",
			display: "Long Distance",
			possibilities: ["HOCHGESCHWINDIGKEITSZUEGE", "INTERCITYUNDEUROCITYZUEGE"],
			selected: true,
			icon: {
				component: LongDistance,
				type: "rounded-corners"
			}
		},
		{
			key: "regional",
			display: "Regional",
			possibilities: ["INTERREGIOUNDSCHNELLZUEGE", "NAHVERKEHRSONSTIGEZUEGE"],
			selected: true,
			icon: {
				component: Regional,
				type: "rounded-corners"
			}
		},
		{
			key: "suburban",
			display: "Suburban",
			possibilities: ["SBAHNEN"],
			selected: true,
			icon: {
				component: Suburban,
				type: "rounded-corners"
			}
		},
		{
			key: "bus",
			display: "Bus",
			possibilities: ["BUSSE"],
			selected: true,
			icon: {
				component: Bus,
				type: "rounded-corners"
			}
		},
		{
			key: "ferry",
			display: "Ferry",
			possibilities: ["SCHIFFE"],
			selected: true,
			icon: {
				component: Ferry,
				type: "rounded-corners"
			}
		},
		{
			key: "subway",
			display: "Subway",
			possibilities: ["UBAHN"],
			selected: true,
			icon: {
				component: Subway,
				type: "rectangle"
			}
		},
		{
			key: "tram",
			display: "Tram",
			possibilities: ["STRASSENBAHN"],
			selected: true,
			icon: {
				component: Tram,
				type: "rectangle"
			}
		},
		{ key: "taxi", display: "Taxi/ Shuttle", possibilities: ["ANRUFPFLICHTIGEVERKEHRE"], selected: true }
	]);

	onMount(() => {
		availableProducts = availableProducts.map((product: Product) => ({ ...product, selected: true }));
		disabledProducts = availableProducts
			.filter((product: Product) => !product.selected)
			.flatMap((product: Product) => product.possibilities);
	});

	let selectedDisplay = $derived(() => {
		const selectedProducts = availableProducts.filter((product) => product.selected);
		if (selectedProducts.length === availableProducts.length) return "Any";
		if (selectedProducts.length >= 1) return `${selectedProducts.length} selected`;
		return "None";
	});
</script>

<button
	class="bg-input-background flex w-full cursor-pointer items-center justify-between rounded-2xl px-4 py-2"
	onclick={() => (dropdownOpen = !dropdownOpen)}
>
	<div class="flex items-center gap-x-2">
		<LongDistance type="circle" />
		<span>Transport products</span>
	</div>

	<span class="text-white/75 italic">{selectedDisplay()}</span>
</button>

{#snippet renderProduct(product: Product)}
	<div class="flex justify-between hover:bg-accent/10 px-4 py-2 rounded-lg transition-colors">
		<div class="flex items-center gap-x-4 text-lg">
			{#if product?.icon}
				{@const Icon = product.icon.component}
				<Icon type={product.icon.type} />
			{/if}
			<span>{product?.display}</span>
		</div>
		<Switch bind:checked={product.selected} />
	</div>
{/snippet}

{#if dropdownOpen}
	<div
		class="bg-input-background absolute top-0 left-0 z-999 flex w-full translate-y-1/2 flex-col gap-y-6 px-4 py-3 md:left-1/2 md:w-[40%] md:-translate-x-1/2 md:rounded-2xl"
	>
		<div class="flex flex-row items-center justify-between">
			<span class="text-xl font-bold">Choose transport type</span>
			<X
				class="stroke-accent cursor-pointer"
				onclick={() => {
					dropdownOpen = false;
					disabledProducts = availableProducts
						.filter((product: Product) => !product.selected)
						.flatMap((product: Product) => product.possibilities);
				}}
			/>
		</div>

		<div class="flex flex-col gap-y-2">
			{#each availableProducts as product, index (product)}
				{@render renderProduct(product)}
			{/each}
		</div>

		<button
			class="bg-accent cursor-pointer self-end rounded-sm px-4 py-2 font-bold text-black"
			onclick={() => {
				dropdownOpen = false;
				disabledProducts = availableProducts
					.filter((product: Product) => !product.selected)
					.flatMap((product: Product) => product.possibilities);
			}}
			>Append
		</button>
	</div>
{/if}
