<script lang="ts">
	import { DateTime } from "luxon";
	import StationSearch from "$components/ui/controls/StationSearch.svelte";
	import type { Station } from "$models/station";
	import TimePicker from "$components/ui/controls/TimePicker.svelte";
	import Clock from "$components/ui/icons/Clock.svelte";
	import { goto } from "$app/navigation";
	import ArrowUpDown from "lucide-svelte/icons/arrow-up-down";
	import MapPinIcon from "lucide-svelte/icons/map-pin";

	let stationFrom: Station | undefined = $state();
	let stationTo: Station | undefined = $state();

	let dateSelected = $state(DateTime.now().set({ second: 0, millisecond: 0 }));

	const queryReady = (): boolean => {
		return !!stationFrom && !!stationTo && !!dateSelected;
	};
</script>

<div class="mt-[-15rem] flex flex-col gap-y-2 md:w-[40%]">
	<div class="relative flex w-full flex-col gap-y-2">
		<StationSearch bind:station={stationFrom} placeholder="From">
			<MapPinIcon size={44} />
		</StationSearch>

		<button
			class="group bg-primary-darker hover:bg-accent absolute top-1/2 z-10 mr-4 -translate-y-1/2 cursor-pointer self-end rounded-full border-2 border-black p-2"
			onclick={() => {
				const temp = stationFrom;
				stationFrom = stationTo;
				stationTo = temp;
			}}
		>
			<ArrowUpDown class="fill-current group-hover:stroke-black" size={35} />
		</button>

		<StationSearch bind:station={stationTo} placeholder="To">
			<MapPinIcon size={44} />
		</StationSearch>
	</div>

	<div class="flex flex-row">
		<TimePicker bind:selectedDate={dateSelected} />

		<div class="ml-auto flex gap-x-1 md:gap-x-3">
			<!-- Reset time -->
			<button
				class="bg-primary-dark flex cursor-pointer flex-row items-center rounded-3xl px-2 md:gap-x-1 md:px-4"
				onclick={() => (dateSelected = DateTime.now().set({ second: 0, millisecond: 0 }))}
			>
				<Clock height="25px" width="25px" />
				<span class="hidden text-xl md:block">Now</span>
			</button>

			<!-- Query departures/ arrivals of a station -->
			<button
				class="{queryReady()
					? 'bg-accent text-black'
					: 'bg-primary-dark text-text hover:bg-secondary'} text-background cursor-pointer rounded-3xl px-4 font-bold md:text-2xl"
				onclick={async () => {
					if (!queryReady()) return;
					await goto(
						`journey/planned?from=${stationFrom?.evaNumber}&to=${stationTo?.evaNumber}&departure=${encodeURIComponent(dateSelected.toISO())}`
					);
				}}
			>
				Search
			</button>
		</div>
	</div>
</div>
