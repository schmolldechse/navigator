<script lang="ts">
	import type {Connection} from "$models/connection";
	import {DateTime} from "luxon";
	import calculateDuration from "$lib/time";
	import Platform from "$components/timetable/info/Platform.svelte";
	import {getContext} from "svelte";

	let {connection}: { connection: Connection } = $props();
	const isDeparture = getContext<boolean>("isDeparture");

	let expanded = $state<boolean>(false);

	const isDelayed = () => {
		const platform = isDeparture ? connection?.departure : connection?.arrival;

		const diff = calculateDuration(
			DateTime.fromISO(platform?.actualTime!),
			DateTime.fromISO(platform?.plannedTime!),
            "minutes"
        );
		return diff >= 1;
    }
</script>

<span>
    {connection?.lineInformation?.lineName}
    <Platform time={isDeparture ? connection?.departure : connection?.arrival} />
</span>