<script lang="ts">
	import { DateTime } from "luxon";
	import StationSearch from "$components/StationSearch.svelte";
	import type { Station } from "$models/station";
	import TimePicker from "$components/timetable/TimePicker.svelte";
	import Clock from "$components/svg/Clock.svelte";

	let typeSelected: "departures" | "arrivals" = $state("departures");

	let stationSelected: Station | undefined = $state(undefined);
	let dateSelected = $state(DateTime.now().set({ second: 0, millisecond: 0 }));

	const gotoRequest = () => {
		if (stationSelected !== undefined) {
			console.log(stationSelected);
			window.location.href = `/${stationSelected?.evaNr}/${typeSelected}?startDate=${encodeURIComponent(dateSelected.toISO())}`;
		}
	};
</script>

<div class="flex flex-col md:w-[40%]">
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

	<StationSearch onStationSelect={(station) => (stationSelected = station)} />

    <div class="flex flex-row justify-between">
        <TimePicker onChangedDate={(newDate) => (dateSelected = newDate)}/>

        <div class="flex flex-row">
            <button class="flex flex-row items-center bg-primary rounded-3xl px-2">
                <Clock height="25px" width="25px" />
                <span class="text-xl">Now</span>
            </button>

            <button
                    class="{stationSelected && dateSelected
			? 'bg-accent text-black'
			: 'bg-primary text-text'} flex items-center justify-center rounded-3xl px-4 text-base font-bold text-background md:text-2xl"
                    onclick={gotoRequest}
            >
                Search
            </button>
        </div>
    </div>

</div>
