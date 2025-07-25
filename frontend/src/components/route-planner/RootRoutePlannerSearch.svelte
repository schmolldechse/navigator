<script lang="ts">
	import ProductPicker from "$components/ui/controls/ProductPicker.svelte";
	import StationSearch from "$components/ui/controls/StationSearch.svelte";
	import TimePicker from "$components/ui/controls/TimePicker.svelte";
	import TravelMode from "$components/ui/controls/TravelMode.svelte";
	import type { Station } from "$models/models";
	import ArrowDownUp from "@lucide/svelte/icons/arrow-down-up";
	import { DateTime } from "luxon";
	import type { Valid } from "luxon/src/_util";

	interface Props {
		onSearch?: (
			start: Station,
			destination: Station,
			date: DateTime<Valid>,
			type: "departure" | "arrival",
			disabledProducts: string[]
		) => void;
	}

	let { onSearch }: Props = $props();

	// settings
	let type = $state<"departure" | "arrival">("departure");
	let start = $state<Station | undefined>(undefined);
	let destination = $state<Station | undefined>(undefined);
	let date = $state(DateTime.now().set({ second: 0, millisecond: 0 }));
	let disabledProducts = $state<string[]>([]);
</script>

<div class="mx-4 flex w-full flex-col gap-y-4 text-base md:w-[720px]">
	<!-- Titlebar -->
	<header class="bg-primary/35 flex items-center justify-between rounded-t-2xl p-4">
		<span class="text-accent text-base font-bold md:text-3xl">Route Planner</span>
		<TravelMode bind:type class="text-xs md:text-base" />
	</header>

	<!-- Start/ Destination -->
	<div class="flex flex-row gap-x-2 px-2">
		<div class="relative flex flex-col justify-around">
			<div class="bg-accent z-1 h-[12px] w-[12px] self-start rounded-full"></div>
			<span class="bg-text/75 absolute z-0 h-1/2 w-[4px] self-center"></span>
			<div class="bg-accent z-1 h-[12px] w-[12px] self-end rounded-full"></div>
		</div>

		<div class="bg-input-background relative z-1 flex w-full flex-col gap-y-0.5 rounded-2xl">
			<StationSearch bind:station={start} placeholder="Start" />

			<button
				class="bg-accent hover:bg-accent absolute top-1/2 z-5 mr-4 -translate-y-1/2 cursor-pointer self-end rounded-full p-2 transition-transform duration-300 hover:rotate-180"
				onclick={() => {
					if (!start || !destination) return;
					if (start.evaNumber === destination.evaNumber) return;

					const temp = start;
					start = destination;
					destination = temp;
				}}
			>
				<ArrowDownUp class="stroke-background stroke-3" />
			</button>

			<StationSearch bind:station={destination} placeholder="Destination" />
		</div>
	</div>

	<!-- Time picker, Producttypes -->
	<div class="flex flex-col gap-y-2 px-2 md:flex-row md:gap-0 md:gap-x-2">
		<TimePicker bind:date />
		<ProductPicker bind:disabledProducts />
	</div>

	<!-- Search button -->
	<button
		class="bg-accent hover:bg-accent/90 disabled:bg-accent/50 cursor-pointer rounded-md px-5 py-2 font-bold text-black transition-colors disabled:cursor-not-allowed md:w-fit md:self-end"
		disabled={disabledProducts.length === 10 ||
			!start ||
			!destination ||
			start.evaNumber === destination.evaNumber ||
			!date}
		onclick={() => {
			if (!onSearch) return;

			if (disabledProducts.length === 10) return;

			if (!start || !destination) return;
			if (start.evaNumber === destination.evaNumber) return;

			if (!date) return;

			onSearch(start, destination, date, type, disabledProducts);
		}}>Search</button
	>
</div>
