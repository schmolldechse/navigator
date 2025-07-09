<script lang="ts">
	import { DateTime } from "luxon";
	import StationSearch from "$components/ui/controls/StationSearch.svelte";
	import type { Station } from "$models/models";
	import TimePicker from "$components/ui/controls/TimePicker.svelte";
	import Search from "@lucide/svelte/icons/search";
	import TravelMode from "$components/ui/controls/TravelMode.svelte";
	import type { Valid } from "luxon/src/_util";

	interface Props {
		onSearch?: (station: Station, date: DateTime<Valid>, type: "departure" | "arrival") => void;
	}

	let { onSearch }: Props = $props();

	// settings
	let type = $state<"departure" | "arrival">("departure");
	let station = $state<Station | undefined>(undefined);
	let date = $state<DateTime>(DateTime.now().set({ second: 0, millisecond: 0 }));
</script>

<div class="mx-4 flex w-full flex-col gap-y-4 text-base md:w-[720px]">
	<!-- Titlebar -->
	<header class="bg-primary/35 flex items-center justify-between rounded-t-2xl p-4">
		<span class="text-accent text-base font-bold md:text-3xl">Timetable</span>
		<TravelMode bind:type class="text-xs md:text-base" />
	</header>

	<!-- Station -->
	<StationSearch bind:station placeholder="Search your station...">
		<Search size={30} color="#ffda0a" class="shrink-0" />
	</StationSearch>

	<TimePicker bind:date />

	<button
		class="bg-accent hover:bg-accent/90 disabled:bg-accent/50 cursor-pointer rounded-md px-5 py-2 font-bold text-black transition-colors md:w-fit md:self-end disabled:cursor-not-allowed"
		disabled={!station || !date}
		onclick={() => {
			if (!onSearch) return;

			if (!station || !date) return;

			onSearch(station, date, type);
		}}>Search</button
	>
</div>
