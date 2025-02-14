<script lang="ts">
	import NavbarButton from "$components/NavbarButton.svelte";
	import { gotoTimetable } from "$lib";
	import { page } from "$app/state";
	import { DateTime } from "luxon";

	let type: "departures" | "arrivals" = $state(page.params.type as "departures" | "arrivals");
</script>

<span class="flex items-center space-x-4 text-base md:text-xl">
	<NavbarButton
		isSelected={type === "departures"}
		onclick={async () => {
			type = "departures";
			await gotoTimetable(
				page.params.evaNumber,
				type,
				page.url.searchParams.get("startDate") ?? DateTime.now().set({ second: 0, millisecond: 0 }).toISO()
			);
		}}
	>
		Departures
	</NavbarButton>
	<NavbarButton
		isSelected={type === "arrivals"}
		onclick={async () => {
			type = "arrivals";
			await gotoTimetable(
				page.params.evaNumber,
				type,
				page.url.searchParams.get("startDate") ?? DateTime.now().set({ second: 0, millisecond: 0 }).toISO()
			);
		}}
	>
		Arrivals
	</NavbarButton>
</span>
