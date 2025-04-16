<script lang="ts">
	import { DateTime } from "luxon";
	import type { Station } from "$models/station";
	import TravelMode from "$components/ui/controls/TravelMode.svelte";
	import StationSearch from "$components/ui/controls/StationSearch.svelte";
	import ArrowUpDown from "lucide-svelte/icons/arrow-up-down";
	import TimePicker from "$components/ui/controls/TimePicker.svelte";
	import ProductPicker from "$components/ui/controls/ProductPicker.svelte";

	let type: "departures" | "arrivals" = $state("departures");

	let start: Station | undefined = $state(undefined);
	let destination: Station | undefined = $state(undefined);

	let dateSelected = $state(DateTime.now().set({ second: 0, millisecond: 0 }));

	let disabledProducts = $state<string[]>([]);

	const queryReady = (): boolean => {
		return !!start && !!destination && !!dateSelected;
	};
</script>

<div class="mx-4 flex w-full flex-col gap-y-4 text-base md:w-[45%]">
	<!-- Titlebar -->
	<header class="bg-titlebar-background flex items-center justify-between rounded-t-2xl p-4">
		<span class="text-accent text-base font-bold md:text-3xl">Route Planner</span>
		<TravelMode {type} class="text-xs md:text-base" />
	</header>

	<!-- Start/ Destination -->
	<div class="flex flex-row gap-x-2 px-2">
		<div class="relative flex flex-col justify-around">
			<div class="bg-accent z-1 h-[12px] w-[12px] self-start rounded-full"></div>
			<span class="bg-text/75 absolute z-0 h-1/2 w-[4px] self-center"></span>
			<div class="bg-accent z-1 h-[12px] w-[12px] self-end rounded-full"></div>
		</div>

		<div class="bg-input-background relative z-1 flex w-full flex-col gap-y-0.5 rounded-2xl">
			<StationSearch bind:station={start} placeholder="Start" class="z-0 md:text-xl" />

			<button
				class="group bg-accent hover:bg-accent absolute top-1/2 z-10 mr-4 -translate-y-1/2 cursor-pointer self-end rounded-full p-2"
			>
				<ArrowUpDown class="stroke-background stroke-3 transition-transform duration-300 group-hover:rotate-180" />
			</button>

			<StationSearch bind:station={destination} placeholder="Destination" class="z-0 md:text-xl" />
		</div>
	</div>

	<!-- Time picker, Producttypes -->
	<div class="flex flex-col md:flex-row px-2">
		<TimePicker bind:selectedDate={dateSelected} />
		<ProductPicker bind:disabledProducts />
	</div>
</div>
