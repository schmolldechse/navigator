<script lang="ts">
	import type { Connection } from "$models/connection";
	import { getContext } from "svelte";
	import TimeComponent from "$components/timetable/info/TimeComponent.svelte";
	import { writeStop } from "$lib";
	import Platform from "$components/timetable/info/Platform.svelte";
	import ViaStops from "$components/timetable/info/ViaStops.svelte";
	import Messages from "$components/timetable/messages/Messages.svelte";

	let { connection }: { connection: Connection } = $props();
	const isDeparture = getContext<boolean>("isDeparture");
</script>

<div class:bg-text={connection?.cancelled ?? false}
	 class:text-background={connection?.cancelled ?? false}
	 class="font-medium"
>
	<!-- layout for smaller screens (under md) -->
	<div class="p-2 md:hidden gap-y-2">
		<!-- 1st row; Messages -->
		<Messages connection={connection} />

		<!-- 2nd row; Line Name -->
		<span
			class="text-lg font-bold"
		>
			{connection?.lineInformation?.lineName} {connection?.lineInformation?.additionalLineName ? " / " + connection?.lineInformation?.additionalLineName : ""}
		</span>

		<!-- 3rd row; Time & Platform Information -->
		<div class="flex flex-row items-center justify-between text-2xl font-semibold">
			<TimeComponent time={isDeparture ? connection?.departure : connection?.arrival} />
			<Platform time={isDeparture ? connection?.departure : connection?.arrival} />
		</div>

		<!-- 4th row; viaStops -->
		<ViaStops viaStops={connection?.viaStops} />

		<!-- 5th row; destination/ origin -->
		<div class="text-2xl">
			{isDeparture
				? writeStop(connection?.actualDestination || connection?.destination, connection?.direction)
				: writeStop(connection?.origin, connection?.provenance)
			}
		</div>
	</div>

	<!-- layout for larger screens (above md) -->
	<div class="mx-auto hidden md:flex flex-col justify-between">
		<!-- 1st row -->
		<div class="flex flex-row">
			<span class="flex-[1] mr-8"></span>
			<span class="flex-[4] mr-4">
				<Messages connection={connection} />
			</span>
			<span class="flex-[1]"></span>
		</div>

		<!-- 2nd row -->
		<div class="flex flex-row">
			<!-- Line Name -->
			<span
				class="flex-[1] mr-8 flex justify-end text-xl font-semibold"
			>
				{connection?.lineInformation?.lineName} {connection?.lineInformation?.additionalLineName ? " / " + connection?.lineInformation?.additionalLineName : ""}
			</span>

			<!-- viaStops -->
			<div class="flex-[4] mr-4 text-lg">
				<ViaStops viaStops={connection?.viaStops} />
			</div>

			<!-- empty -->
			<span class="flex-[1]"></span>
		</div>

		<!-- 3rd row -->
		<div class="flex flex-row items-center text-3xl">
			<!-- Time Information -->
			<div class="flex-[1] mr-8">
				<TimeComponent time={isDeparture ? connection?.departure : connection?.arrival} />
			</div>

			<div class="flex-[4] mr-4">
				{isDeparture
					? writeStop(connection?.actualDestination || connection?.destination, connection?.direction)
					: writeStop(connection?.origin, connection?.provenance)
				}
			</div>

			<span class="flex-[1] flex text-right">
				<Platform time={isDeparture ? connection?.departure : connection?.arrival} />
			</span>
		</div>
	</div>
</div>