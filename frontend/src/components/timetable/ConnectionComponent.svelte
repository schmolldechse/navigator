<script lang="ts">
	import type { Connection } from "$models/connection";
	import { getContext } from "svelte";
	import TimeComponent from "$components/timetable/info/TimeComponent.svelte";
	import { writeStop } from "$lib";
	import Platform from "$components/timetable/info/Platform.svelte";
	import ViaStops from "$components/timetable/info/ViaStops.svelte";
	import Messages from "$components/timetable/messages/Messages.svelte";
	import type { Station } from "$models/station";
	import { DateTime } from "luxon";

	let {
		connection,
		renderBorder,
		renderInformation
	}: {
		connection: Connection;
		renderBorder: boolean;
		renderInformation: boolean;
	} = $props();
	const isDeparture = getContext<boolean>("isDeparture");
	const station = getContext<Station>("station");
</script>

<div
	class:bg-text={connection?.cancelled ?? false}
	class:text-background={connection?.cancelled ?? false}
	class="py-1 font-medium"
>
	<!-- layout for smaller screens (under md) -->
	<div class="gap-y-2 p-2 md:hidden">
		<!-- 1st row; Messages -->
		<Messages {connection} />

		<!-- 2nd row; Line Name -->
		<div class="flex flex-row text-lg font-bold">
			<a
				href={`/journey/coach-sequence?lineDetails=${connection?.lineInformation?.product}_${connection?.lineInformation?.fahrtNr}&evaNumber=${station?.evaNumber}&date=${DateTime.local().toFormat("yyyyMMdd")}`}
				target="_blank"
			>
				{connection?.lineInformation?.lineName}
			</a>
			{#if connection?.lineInformation?.additionalLineName}
				<span>/ {connection?.lineInformation?.additionalLineName}</span>
			{/if}
		</div>

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
				: writeStop(connection?.origin, connection?.provenance)}
		</div>
	</div>

	<!-- layout for larger screens (above md) -->
	<div class="mx-auto hidden flex-col justify-between md:flex">
		<!-- 1st row -->
		<div class="flex flex-row">
			<span class="mr-8 flex-1" class:border-t={renderBorder} class:border-text={renderBorder}></span>
			<span class="mr-4 flex-4" class:border-t={renderBorder} class:border-text={renderBorder}>
				<Messages {connection} />
			</span>
			<span class="flex-1" class:border-t={renderBorder} class:border-text={renderBorder}></span>
		</div>

		<!-- 2nd row -->
		<div class="flex flex-row">
			<!-- Line Name -->
			<div class="mr-8 flex flex-1 flex-row justify-end text-right text-xl font-semibold">
				<a
					href={`/journey/coach-sequence?lineDetails=${connection?.lineInformation?.product}_${connection?.lineInformation?.fahrtNr}&evaNumber=${station?.evaNumber}&date=${DateTime.local().toFormat("yyyyMMdd")}`}
					target="_blank"
				>
					{connection?.lineInformation?.lineName}
				</a>
				{#if connection?.lineInformation?.additionalLineName}
					<span>/ {connection?.lineInformation?.additionalLineName}</span>
				{/if}
			</div>

			<!-- viaStops -->
			<div class="mr-4 flex-4 text-lg">
				<ViaStops viaStops={connection?.viaStops} />
			</div>

			<!-- empty -->
			<span class="flex-1"></span>
		</div>

		<!-- 3rd row -->
		<div class="flex flex-row items-center text-3xl">
			<!-- Time Information -->
			<div class="mr-8 flex-1">
				{#if renderInformation}
					<TimeComponent time={isDeparture ? connection?.departure : connection?.arrival} />
				{/if}
			</div>

			<div class="mr-4 flex-4">
				{isDeparture
					? writeStop(connection?.actualDestination || connection?.destination, connection?.direction)
					: writeStop(connection?.origin, connection?.provenance)}
			</div>

			<span class="flex flex-1 text-right">
				<Platform time={isDeparture ? connection?.departure : connection?.arrival} />
			</span>
		</div>
	</div>
</div>
