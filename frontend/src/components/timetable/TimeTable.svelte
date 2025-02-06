<script lang="ts">
	import {DateTime} from "luxon";
	import StationSearch from "$components/StationSearch.svelte";
    import type {Station} from "$models/station";
    import TimePicker from "$components/timetable/TimePicker.svelte";

	let typeSelected: "departures" | "arrivals" = $state("departures");

    let stationSelected: Station | undefined = $state(undefined);
	let dateSelected = $state(DateTime.now().set({second: 0, millisecond: 0}));

    const gotoRequest = () => {
        if (stationSelected !== undefined) {
            console.log(stationSelected);
            window.location.href = `/${stationSelected?.evaNr}/${typeSelected}?startDate=${encodeURIComponent(dateSelected.toISO())}`;
        }
    }
</script>

<div class="md:w-[40%] flex flex-col">

    <div class="flex flex-row items-center justify-end gap-x-2">
        <button
                type="button"
                class="cursor-pointer font-bold"
                class:underline={typeSelected === 'departures'}
                class:underline-offset-2={typeSelected === 'departures'}
                class:decoration-2={typeSelected === 'departures'}
                class:text-accent={typeSelected === 'departures'}
                class:text-text={typeSelected !== 'departures'}
                onclick={() => typeSelected = "departures"}
        >
            Departures
        </button>
        <span class="text-xl nd-fg-white">|</span>
        <button
                type="button"
                class="cursor-pointer font-bold"
                class:underline={typeSelected === 'arrivals'}
                class:underline-offset-2={typeSelected === 'arrivals'}
                class:decoration-2={typeSelected === 'arrivals'}
                class:text-accent={typeSelected === 'arrivals'}
                class:text-text={typeSelected !== 'arrivals'}
                onclick={() => typeSelected = 'arrivals'}
        >
            Arrivals
        </button>
    </div>

    <StationSearch onStationSelect={(station) => stationSelected = station} />

    <div class="flex flex-col">
        <span>Pick a time:</span>
        <!-- Date picker should be here -->
    </div>

    <button class="{stationSelected && dateSelected ? 'bg-accent text-black' : 'bg-primary text-text'} p-2 rounded-md text-background font-bold text-base md:text-2xl flex justify-center items-center"
       onclick={gotoRequest}
    >
        Request
    </button>
</div>