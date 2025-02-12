<script lang="ts">
	import type {Connection} from "$models/connection";
	import {DateTime} from "luxon";
	import calculateDuration from "$lib/time";
	import Platform from "$components/timetable/info/Platform.svelte";
	import {getContext} from "svelte";

	let {connection}: { connection: Connection } = $props();
	const isDeparture = getContext<boolean>("isDeparture");

	const isDelayed = () => {
		const platform = isDeparture ? connection?.departure : connection?.arrival;

		const diff = calculateDuration(
			DateTime.fromISO(platform?.actualTime!),
			DateTime.fromISO(platform?.plannedTime!),
            "minutes"
        );
		return diff >= 1;
    }

	const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE);
</script>

<div class:bg-text={connection?.cancelled ?? false}
     class:text-background={connection?.cancelled ?? false}
     class="font-medium"
>
    <!-- layout for smaller screens (under md) -->
    <div class="p-2 md:hidden gap-y-2">

    </div>

    <!-- layout for larger screens (above md) -->
    <div class="mx-auto hidden md:flex flex-col justify-between">
        <!-- 1st row -->
        <div class="flex flex-row">
            <span class="flex-[1] mr-8"></span>
            <span class="flex-[4] mr-4">Connection details</span>
            <span class="flex-[1]"></span>
        </div>

        <!-- 2nd row -->
        <div class="flex flex-row">
            <!-- Line Name -->
            <span class="flex-[1] mr-8 flex justify-end text-xl">{connection?.lineInformation?.lineName} {connection?.lineInformation?.additionalLineName ? " / " + connection?.lineInformation?.additionalLineName : ""}</span>

            <!-- viaStops -->
            <div class="flex-[4] mr-4 text-lg">

            </div>

            <!-- empty -->
            <span class="flex-[1]"></span>
        </div>
    </div>
</div>