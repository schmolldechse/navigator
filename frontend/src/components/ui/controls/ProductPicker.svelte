<script lang="ts">
	import LongDistance from "$components/ui/transport-types/LongDistance.svelte";
	import { type Component, onMount } from "svelte";
	import X from "lucide-svelte/icons/x";
	import Regional from "$components/ui/transport-types/Regional.svelte";
	import Suburban from "$components/ui/transport-types/Suburban.svelte";
	import Bus from "$components/ui/transport-types/Bus.svelte";
	import Ferry from "$components/ui/transport-types/Ferry.svelte";
	import Subway from "$components/ui/transport-types/Subway.svelte";
	import Tram from "$components/ui/transport-types/Tram.svelte";
	import Switch from "$components/ui/interactive/Switch.svelte";
	import Taxi from "$components/ui/transport-types/Taxi.svelte";
	import ChevronDown from "lucide-svelte/icons/chevron-down";
	import Button from "$components/ui/interactive/Button.svelte";

	export interface Product {
		key: string;
		display: string;
		possibilities: string[];
		icon?: {
			component: Component;
			type: string;
		};
	}

	let { disabledProducts = $bindable<string[]>([]) }: { disabledProducts: string[] } = $props();
	let dropdownOpen = $state<boolean>(false);
	let dropdownElement: HTMLDivElement | undefined = $state<HTMLDivElement | undefined>(undefined);

	const availableProducts: Product[] = [
		{
			key: "long-distance",
			display: "Long Distance",
			possibilities: ["HOCHGESCHWINDIGKEITSZUEGE", "INTERCITYUNDEUROCITYZUEGE"],
			icon: {
				component: LongDistance,
				type: "rounded-corners"
			}
		},
		{
			key: "regional",
			display: "Regional",
			possibilities: ["INTERREGIOUNDSCHNELLZUEGE", "NAHVERKEHRSONSTIGEZUEGE"],
			icon: {
				component: Regional,
				type: "rounded-corners"
			}
		},
		{
			key: "suburban",
			display: "Suburban",
			possibilities: ["SBAHNEN"],
			icon: {
				component: Suburban,
				type: "rounded-corners"
			}
		},
		{
			key: "bus",
			display: "Bus",
			possibilities: ["BUSSE"],
			icon: {
				component: Bus,
				type: "rounded-corners"
			}
		},
		{
			key: "ferry",
			display: "Ferry",
			possibilities: ["SCHIFFE"],
			icon: {
				component: Ferry,
				type: "rounded-corners"
			}
		},
		{
			key: "subway",
			display: "Subway",
			possibilities: ["UBAHN"],
			icon: {
				component: Subway,
				type: "rectangle"
			}
		},
		{
			key: "tram",
			display: "Tram",
			possibilities: ["STRASSENBAHN"],
			icon: {
				component: Tram,
				type: "rectangle"
			}
		},
		{
			key: "taxi",
			display: "Taxi/ Shuttle",
			possibilities: ["ANRUFPFLICHTIGEVERKEHRE"],
			icon: {
				component: Taxi,
				type: "rectangle"
			}
		}
	];

	onMount(() => {
		const clickOutside = (event: MouseEvent) => {
			if (!dropdownElement) return;
			if (dropdownElement.contains(event.target as Node)) return;
			dropdownOpen = false;
		};
		document.addEventListener("click", clickOutside);
		return () => document.removeEventListener("click", clickOutside);
	});

	let selectedDisplay = $derived(() => {
		const selectedProducts = availableProducts.filter(
			(product) => !disabledProducts.some((p) => product.possibilities.includes(p))
		);
		if (selectedProducts.length === availableProducts.length) return "Any";
		if (selectedProducts.length >= 1) return `${selectedProducts.length} selected`;
		return "None";
	});

	const toggleProduct = (product: Product) => {
		const anyDisabled = product.possibilities.some((p) => disabledProducts.includes(p));
		if (anyDisabled) {
			// remove all product.possibilities from disabledProducts
			disabledProducts = disabledProducts.filter((p) => !product.possibilities.includes(p));
			return;
		}

		// add all product.possibilities to disabledProducts (if not already present)
		disabledProducts = [...disabledProducts, ...product.possibilities.filter((p) => !disabledProducts.includes(p))];
	};
</script>

<!-- overflow-hidden needed for highlight effect! -->
<button
	class="bg-input-background group relative flex w-full cursor-pointer items-center justify-between overflow-hidden rounded-2xl px-4 py-3 shadow-sm"
	onclick={(event: MouseEvent) => {
		event.stopPropagation();
		dropdownOpen = !dropdownOpen;
	}}
	aria-expanded={dropdownOpen}
	aria-haspopup="true"
>
	<!-- highlight effect on hover -->
	<div class="bg-accent/5 absolute inset-0 opacity-0 transition-opacity group-hover:opacity-100"></div>

	<div class="flex items-center gap-x-2">
		<LongDistance type="circle" width="30px" height="30px" />
		<span class="font-medium">Transport products</span>
	</div>

	<div class="flex items-center gap-x-2">
		<span class="text-white/75 italic">{selectedDisplay()}</span>
		<ChevronDown class={`stroke-accent transition-transform duration-200 ${dropdownOpen ? "rotate-180" : ""}`} />
	</div>
</button>

{#snippet renderProduct(product: Product)}
	<div class="hover:bg-accent/10 flex justify-between rounded-lg px-4 py-2 transition-colors">
		<div class="flex items-center gap-x-4 text-lg">
			{#if product?.icon}
				{@const Icon = product.icon.component}
				<Icon type={product.icon.type} />
			{/if}
			<span>{product?.display}</span>
		</div>
		<Switch
			checked={!product.possibilities.some((p) => disabledProducts.includes(p))}
			onchecked={() => toggleProduct(product)}
		/>
	</div>
{/snippet}

{#if dropdownOpen}
	<div
		bind:this={dropdownElement}
		class="bg-input-background border-primary absolute top-0 left-0 z-999 flex w-full flex-col gap-y-6 border px-4 py-3 md:left-1/2 md:w-[40%] md:-translate-x-1/2 md:translate-y-1/4 md:rounded-2xl"
		role="menu"
		aria-orientation="vertical"
	>
		<div class="flex flex-row items-center justify-between">
			<span class="text-xl font-bold">Choose transport type</span>
			<X
				class="hover:stroke-accent cursor-pointer transition-colors"
				onclick={() => {
					dropdownOpen = false;
				}}
			/>
		</div>

		<div class="flex flex-col gap-y-2">
			{#each availableProducts as product, index (product)}
				{@render renderProduct(product)}
			{/each}
		</div>

		<Button
			class="self-end rounded-md px-5 py-2 font-bold text-black"
			onclick={() => {
				dropdownOpen = false;
			}}
		>
			Append
		</Button>
	</div>
{/if}
