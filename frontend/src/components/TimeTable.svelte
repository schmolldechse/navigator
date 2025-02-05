<script lang="ts">
	import { DateTime } from "luxon";

	let typeSelected: "departures" | "arrivals" = $state("departures");

	let stationSelected = $state(242359);
	let dateSelected = $state(DateTime.now().set({ second: 0, millisecond: 0 }));
</script>

<div class="ml-auto items-center space-x-4">

	<button type="button"
			class="cursor-pointer {typeSelected === 'departures' ? 'text-primary font-bold' : 'text-text font-bold'}"
			onclick={() => typeSelected = "departures"}>Departures
	</button>
	<span class="text-xl nd-fg-white">|</span>
	<button type="button"
			class="cursor-pointer {typeSelected === 'arrivals' ? 'text-primary font-bold' : 'text-text font-bold'}"
			onclick={() => typeSelected = 'arrivals'}>Arrivals
	</button>
</div>

<!--
<StationSearch onSelectStation={(station) => setStationSelected(station)} />
-->

<div class="flex flex-col">
	<span>Pick a time:</span>
	<!-- Date picker should be here -->
</div>

<a class="w-full bg-primary p-2 rounded text-black font-bold text-base md:text-2xl text-center flex justify-center items-center"
   href="/{stationSelected}/{typeSelected}?startDate=${encodeURIComponent(dateSelected.toISO())}">
	Request
</a>