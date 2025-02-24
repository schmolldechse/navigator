<script lang="ts">
	import { DateTime } from "luxon";
	import StationSearch from "$components/StationSearch.svelte";
	import type { Station } from "$models/station";
	import TimePicker from "$components/TimePicker.svelte";
	import Clock from "$components/svg/Clock.svelte";
	import { gotoTimetable } from "$lib";

	let typeSelected: "departures" | "arrivals" = $state("departures");

	let stationSelected: Station | undefined = $state();
	let dateSelected = $state(DateTime.now().set({ second: 0, millisecond: 0 }));
</script>

<div class="mt-[-15rem] flex flex-col gap-y-2 md:w-[40%]">
	<div class="flex flex-row items-center justify-end gap-x-2">
		<button
			type="button"
			class="cursor-pointer font-bold"
			class:underline={typeSelected === "departures"}
			class:underline-offset-2={typeSelected === "departures"}
			class:decoration-2={typeSelected === "departures"}
			class:text-accent={typeSelected === "departures"}
			class:text-text={typeSelected !== "departures"}
			onclick={() => (typeSelected = "departures")}
		>
			Departures
		</button>
		<span class="nd-fg-white text-xl">|</span>
		<button
			type="button"
			class="cursor-pointer font-bold"
			class:underline={typeSelected === "arrivals"}
			class:underline-offset-2={typeSelected === "arrivals"}
			class:decoration-2={typeSelected === "arrivals"}
			class:text-accent={typeSelected === "arrivals"}
			class:text-text={typeSelected !== "arrivals"}
			onclick={() => (typeSelected = "arrivals")}
		>
			Arrivals
		</button>
	</div>

	<StationSearch bind:station={stationSelected} />

	<div class="flex flex-row">
		<TimePicker bind:selectedDate={dateSelected} />

		<div class="ml-auto flex gap-1 md:gap-3">
			<!-- Reset time -->
			<button
				class="flex flex-row items-center rounded-3xl bg-primary px-2 md:gap-x-1 md:px-4"
				onclick={() => (dateSelected = DateTime.now().set({ second: 0, millisecond: 0 }))}
			>
				<Clock height="25px" width="25px" />
				<span class="hidden text-xl md:block">Now</span>
			</button>

			<!-- Query departures/ arrivals of a station -->
			<button
				class="{stationSelected && dateSelected
					? 'bg-accent text-black'
					: 'bg-primary text-text hover:bg-secondary'} flex items-center justify-center rounded-3xl px-4 font-bold text-background md:text-2xl"
				onclick={async () => {
					if (!stationSelected || !dateSelected) return;
					await gotoTimetable(stationSelected?.evaNumber, typeSelected, dateSelected.toISO());
				}}
			>
				Search
			</button>
		</div>
	</div>
</div>
