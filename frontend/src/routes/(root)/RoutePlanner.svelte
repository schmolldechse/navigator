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

<div class="text-base w-full md:w-[45%] mx-4 flex flex-col gap-y-4">
	<!-- Titlebar -->
	<header class="p-4 rounded-t-2xl bg-titlebar-background flex items-center justify-between">
		<span class="text-base md:text-3xl font-bold text-accent">Route Planner</span>
		<TravelMode {type} class="text-xs md:text-base" />
	</header>

	<!-- Start/ Destination -->
	<div class="flex flex-row px-2 gap-x-2">
		<div class="relative flex flex-col justify-around">
			<div class="w-[12px] h-[12px] z-1 bg-accent rounded-full self-start"></div>
			<span class="bg-text/75 absolute z-0 h-1/2 w-[4px] self-center"></span>
			<div class="w-[12px] h-[12px] z-1 bg-accent rounded-full self-end"></div>
		</div>

		<div class="relative flex flex-col w-full z-1 bg-input-background rounded-2xl gap-y-0.5">
			<StationSearch bind:station={start} placeholder="Start" class="z-0 md:text-xl" />

			<button class="group bg-accent hover:bg-accent absolute top-1/2 z-10 mr-4 -translate-y-1/2 cursor-pointer self-end rounded-full p-2">
				<ArrowUpDown class="stroke-background stroke-3 group-hover:rotate-180 transition-transform duration-300" />
			</button>

			<StationSearch bind:station={destination} placeholder="Destination" class="z-0 md:text-xl" />
		</div>
	</div>

	<!-- Time picker, Producttypes -->
	<div class="flex flex-row px-2">
		<TimePicker bind:selectedDate={dateSelected} />
		<ProductPicker bind:disabledProducts={disabledProducts} />
	</div>
</div>
