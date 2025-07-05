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

	{#each displayedStops as viaStop, index}
		<span>{writeStop(viaStop)}</span>{#if index < displayedStops.length - 1}
			<span class="px-2 text-xl font-bold tracking-widest">&minus;</span>
		{/if}
	{/each}

	{#if viaStops.length > 3}
		<ShowMore onclick={() => (viaStopsExtended = !viaStopsExtended)} />
	{/if}
{/snippet}

{#snippet renderSingleTimetableEntry(singleTimetableEntry: SingleTimetableEntry)}
	<div
		class={[
			{ "bg-text text-background": singleTimetableEntry.cancelled },
			{ "bg-background text-text": !singleTimetableEntry.cancelled }
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
					{!isPlatformChanged(singleTimetableEntry.timeInformation)
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
	</div>
{/snippet}

<div class="flex flex-col">
	{#each timetableEntry.entries as singleEntry}
		{@render renderSingleTimetableEntry(singleEntry)}
	{/each}
</div>
