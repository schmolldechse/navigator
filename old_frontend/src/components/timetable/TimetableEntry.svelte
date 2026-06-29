<script lang="ts">
	import { page } from "$app/state";
	import type { GroupedTimetableEntry, SingleTimetableEntry, Time, TimetableStop } from "$models/models";
	import { DateTime } from "luxon";
	import ShowMore from "./info/ShowMore.svelte";
	import Messages from "./messages/Messages.svelte";

	interface Props {
		timetableEntry: GroupedTimetableEntry;
	}

	let { timetableEntry }: Props = $props();

	let isDeparture = page.params.type === "departures";

	let viaStopsExtended = $state<boolean>(false);

	const writeStop = (stop: TimetableStop): string => {
		if (!stop) return "???";
		if (!stop.nameParts || stop.nameParts.length === 0) return stop.name;
		return stop.nameParts
			.map((part) => part.value)
			.join("")
			.trim();
	};

	const isDelayed = (timeInformation: Time): boolean => {
		const delay =
			DateTime.fromISO(timeInformation.actualTime).toSeconds() -
			DateTime.fromISO(timeInformation.plannedTime).toSeconds();
		return delay < -1 || delay > 1;
	};

	const isPlatformChanged = (timeInformation: Time): boolean => {
		return timeInformation.plannedPlatform !== timeInformation.actualPlatform;
	};

	const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE);
</script>

{#snippet renderViaStops(viaStops: TimetableStop[])}
	{@const displayedStops = viaStopsExtended ? viaStops : viaStops.slice(0, 3)}

	<div class="flex flex-wrap items-center">
		{#each displayedStops as viaStop, index}
			<span class="text-base md:text-xl">{writeStop(viaStop)}</span>{#if index < displayedStops.length - 1}
				<span class="px-2 text-xl font-bold tracking-widest md:text-2xl">&minus;</span>
			{/if}
		{/each}

		{#if viaStops.length > 3}
			<ShowMore onclick={() => (viaStopsExtended = !viaStopsExtended)} />
		{/if}
	</div>
{/snippet}

{#snippet renderSingleTimetableEntry(singleTimetableEntry: SingleTimetableEntry)}
	<div
		class={[
			{ "bg-text text-background": singleTimetableEntry.cancelled },
			{ "bg-background text-text": !singleTimetableEntry.cancelled },
			"py-2"
		]}
	>
		<!-- Layout for smaller screens -->
		<div class="flex flex-col gap-y-0.5 px-2 md:hidden">
			<!-- 1st row: Messages -->
			<Messages timetableEntry={singleTimetableEntry} />

			<!-- 2nd row: LineInformation -->
			<div class="flex flex-row text-lg font-bold">
				<span>{singleTimetableEntry.lineInformation.journeyName}</span>
				{#if singleTimetableEntry.lineInformation?.additionalJourneyName}
					<span>/ {singleTimetableEntry.lineInformation?.additionalJourneyName}</span>
				{/if}
			</div>

			<!-- 3rd row: TimeInformation -->
			<div class="flex items-center justify-between">
				<!-- Time -->
				<div class="flex gap-x-2">
					<span class="text-xl font-medium">{displayTime(singleTimetableEntry.timeInformation.plannedTime)}</span>
					{#if isDelayed(singleTimetableEntry.timeInformation)}
						<span
							class="bg-text text-background flex w-fit items-center justify-center px-2 py-0.5 text-[12pt] font-bold"
						>
							{displayTime(singleTimetableEntry.timeInformation.actualTime)}
						</span>
					{/if}
				</div>

				<!-- Platform -->
				<span
					class={[
						{
							"bg-text text-background px-2 py-0.5 text-[12pt] font-bold md:px-0": isPlatformChanged(
								singleTimetableEntry.timeInformation
							)
						},
						{ "text-xl font-medium": !isPlatformChanged(singleTimetableEntry.timeInformation) }
					]}
				>
					{isPlatformChanged(singleTimetableEntry.timeInformation)
						? singleTimetableEntry.timeInformation.actualPlatform
						: singleTimetableEntry.timeInformation.plannedPlatform}
				</span>
			</div>

			<!-- 4th row: ViaStops -->
			<div>
				{@render renderViaStops(singleTimetableEntry.viaStops)}
			</div>

			<!-- 5th row: Destination/ Origin -->
			<span class="text-xl font-semibold tracking-tight">
				{isDeparture ? writeStop(singleTimetableEntry.destination) : writeStop(singleTimetableEntry.origin)}
			</span>
		</div>

		<!-- Layout for larger screens -->
		<div class="container mx-auto hidden flex-col gap-y-0.5 md:flex">
			<!-- 1st row: Messages -->
			<div class="flex flex-row gap-x-8">
				<!-- Spacer -->
				<div class="flex-1"></div>

				<!-- Messages -->
				<div class="flex-4">
					<Messages timetableEntry={singleTimetableEntry} />
				</div>

				<!-- Spacer -->
				<div class="flex-1"></div>
			</div>

			<!-- 2nd row: LineInformation, ViaStops -->
			<div class="flex flex-row items-center gap-x-8">
				<!-- LineInformation -->
				<div class="flex flex-1 justify-end text-xl font-semibold">
					<span>{singleTimetableEntry.lineInformation.journeyName}</span>
					{#if singleTimetableEntry.lineInformation?.additionalJourneyName}
						<span>/ {singleTimetableEntry.lineInformation?.additionalJourneyName}</span>
					{/if}
				</div>

				<!-- ViaStops -->
				<div class="flex-4">
					{@render renderViaStops(singleTimetableEntry.viaStops)}
				</div>

				<!-- Spacer -->
				<div class="flex-1"></div>
			</div>

			<!-- 3rd row: Time, Destination/ Origin, Platform -->
			<div class="flex flex-row items-center gap-x-8">
				<!-- Time Information -->
				<div class="flex w-full flex-1 justify-end gap-x-2">
					<span class="text-3xl font-medium">{displayTime(singleTimetableEntry.timeInformation.plannedTime)}</span>
					{#if isDelayed(singleTimetableEntry.timeInformation)}
						<span
							class="bg-text text-background flex w-fit items-center justify-center px-2 py-0.5 text-[16pt] font-bold"
						>
							{displayTime(singleTimetableEntry.timeInformation.actualTime)}
						</span>
					{/if}
				</div>

				<!-- Destination/ Origin -->
				<span class="flex-4 text-3xl font-medium tracking-tight">
					{isDeparture ? writeStop(singleTimetableEntry.destination) : writeStop(singleTimetableEntry.origin)}
				</span>

				<!-- Platform -->
				<span
					class={[
						{
							"bg-text text-background px-2 py-0.5 text-[16pt] font-bold md:px-0": isPlatformChanged(
								singleTimetableEntry.timeInformation
							)
						},
						{ "text-3xl font-medium": !isPlatformChanged(singleTimetableEntry.timeInformation) },
						"w-full flex-1 text-right"
					]}
				>
					{isPlatformChanged(singleTimetableEntry.timeInformation)
						? singleTimetableEntry.timeInformation.actualPlatform
						: singleTimetableEntry.timeInformation.plannedPlatform}
				</span>
			</div>
		</div>
	</div>
{/snippet}

<div class="flex flex-col">
	{#each timetableEntry.entries as singleEntry}
		{@render renderSingleTimetableEntry(singleEntry)}
	{/each}
</div>
