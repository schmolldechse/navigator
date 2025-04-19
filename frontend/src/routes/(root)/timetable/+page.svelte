<script lang="ts">
	import { DateTime } from "luxon";
	import StationSearch from "$components/ui/controls/StationSearch.svelte";
	import type { Station } from "$models/station";
	import TimePicker from "$components/ui/controls/TimePicker.svelte";
	import { goto } from "$app/navigation";
	import Search from "lucide-svelte/icons/search";
	import TravelMode from "$components/ui/controls/TravelMode.svelte";
	import Button from "$components/ui/interactive/Button.svelte";
	import type { Snapshot } from "../$types";

	let type: "departures" | "arrivals" = $state<"departures" | "arrivals">("departures");
	let station: Station | undefined = $state(undefined);
	let date = $state(DateTime.now().set({ second: 0, millisecond: 0 }));

	interface SnapshotData {
		station: Station | undefined;
		type: "departures" | "arrivals";
	}

	export const snapshot: Snapshot<SnapshotData> = {
		capture: () => ({ station, type }),
		restore: (value) => {
			station = value.station;
			type = value.type;
		}
	};
</script>

<div class="mx-4 flex w-full flex-col gap-y-4 text-base md:w-[45%]">
	<!-- Titlebar -->
	<header class="bg-titlebar-background flex items-center justify-between rounded-t-2xl p-4">
		<span class="text-accent text-base font-bold md:text-3xl">Timetable</span>
		<TravelMode bind:type class="text-xs md:text-base" />
	</header>

	<!-- Station -->
	<StationSearch bind:station placeholder="Search your station...">
		<Search size={44} />
	</StationSearch>

	<TimePicker bind:date />

	<Button
		class="rounded-md px-5 py-2 font-bold text-black md:w-fit md:self-end"
		onclick={async () => {
			if (!station) return;
			if (!date) return;

			await goto(`/${station?.evaNumber}/${type}?startDate=${encodeURIComponent(date.toISO())}`);
		}}
	>
		Search
	</Button>
</div>
