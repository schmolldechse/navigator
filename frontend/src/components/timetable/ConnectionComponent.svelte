<script lang="ts">
	import type {Connection} from "$models/connection";
	import {DateTime} from "luxon";
	import calculateDuration from "$lib/time";

	let {connection, isDeparture}: { connection: Connection, isDeparture: boolean } = $props();

	let expanded = $state<boolean>(false);

	const isDelayed = () => {
		const diff = calculateDuration(
			DateTime.fromISO(isDeparture ? connection?.departure?.actualTime! : connection?.arrival?.actualTime!),
			DateTime.fromISO(isDeparture ? connection?.departure?.plannedTime! : connection?.arrival?.plannedTime!),
            "minutes"
        );
		return diff >= 1;
    }
</script>

<div>
    <span>{JSON.stringify(connection?.departure)}</span>
    <span>{connection?.lineInformation?.lineName}</span>
</div>