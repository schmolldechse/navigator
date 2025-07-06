<script lang="ts">
	import Walking from "$components/ui/icons/Walking.svelte";
	import type { NormalSection, RouteEntry, Time } from "$models/models";
	import ChevronDown from "lucide-svelte/icons/chevron-down";
	import ChevronUp from "lucide-svelte/icons/chevron-up";
	import Ban from "lucide-svelte/icons/ban";
	import { DateTime } from "luxon";

	interface Props {
		route: RouteEntry;
	}

	let { route }: Props = $props();
	let detailsExpanded = $state<boolean>(false);

	const isCancelled: boolean = $derived(route.sections?.some((section) => section.cancelled));

	const isChangeoverPossible: boolean = $derived.by(() => {
		if (route.sections.length === 0) return true;

		// skip if any section is cancelled, or no changeovers are available
		if (route.sections.some((section) => section.cancelled)) return true;

		return route.sections.every((section, index) => {
			const startWalking = route.sections[index - 1]?.destination.arrival;
			const stopWalking = route.sections[index + 1]?.origin.departure;

			if (!startWalking || !stopWalking) return true;
			return (
				DateTime.fromISO(stopWalking.actualTime).diff(DateTime.fromISO(startWalking.actualTime), "minutes").minutes >= 0
			);
		});
	});

	const formatDuration = (end: Time, start: Time): string => {
		const duration = DateTime.fromISO(end.actualTime).diff(DateTime.fromISO(start.actualTime), ["hours", "minutes"]);

		if (duration.hours >= 1) return duration.toFormat("h'h' m'm'");
		else return duration.toFormat("m'm'");
	};

	const getWidthRatio = (duration: number, maxDuration: number): string => {
		if (maxDuration <= 0) return "0%";
		return `${(duration / maxDuration) * 100}%`;
	};

	const durationOfSection = (section: NormalSection): number => {
		const departureTime = DateTime.fromISO(section.origin.departure!.actualTime);
		const arrivalTime = DateTime.fromISO(section.destination.arrival!.actualTime);
		return arrivalTime.diff(departureTime, ["minutes"]).minutes;
	};

	const durationWithoutWalking = $derived.by(() => {
		// remove sections with "isWalking" property
		const sections = route.sections.filter((section) => !section.isWalking);
		if (sections.length === 0) return 0;

		return sections.map((section) => durationOfSection(section as NormalSection)).reduce((acc, el) => acc + el, 0);
	});
</script>

{#snippet renderTimeInformation(time: Time)}
	{@const delay = DateTime.fromISO(time?.actualTime).diff(DateTime.fromISO(time?.plannedTime), ["seconds"]).seconds}
	{@const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE)}

	<div class="flex flex-col items-center gap-x-2">
		<span class="text-base font-medium md:text-xl">{displayTime(time.plannedTime)}</span>
		{#if delay < -60 || delay > 60}
			<span class="text-background bg-white px-1.5 text-[10pt] font-bold md:px-2 md:text-[12pt]"
				>{displayTime(time.actualTime)}</span
			>
		{/if}
	</div>
{/snippet}

<div class="border-primary/45 container mx-auto overflow-hidden rounded-lg border-2">
	<!-- Header -->
	{#if isCancelled || !isChangeoverPossible}
		<div class="bg-secondary/50 flex flex-row items-center gap-x-2 px-3 py-1">
			<Ban size="20" />
			<span class="text-sm font-semibold md:text-lg">Route is not possible</span>
		</div>
	{/if}

	<!-- General Route Details -->
	<div class="space-y-2 px-4 py-2" class:opacity-65={isCancelled}>
		<!-- TimeInformation -->
		<div class="flex flex-row items-baseline gap-x-2">
			<div class="flex flex-row gap-x-2">
				{@render renderTimeInformation(route.sections[0].origin.departure!)}
				<span class="-mt-[0.1rem] text-xl font-bold tracking-tighter md:mt-[0.1rem]">&minus;</span>
				{@render renderTimeInformation(route.sections[route.sections.length - 1].destination.arrival!)}
			</div>

			<span class="text-text/50 font-bold">&vert;</span>
			<span class="text-sm"
				>{formatDuration(
					route.sections[route.sections.length - 1].destination.arrival!,
					route.sections[0].origin.departure!
				)}</span
			>

			{#if route.sections.length - 1 > 0}
				{@const walkingSections = route.sections.length - 1}

				<span class="text-text/50 font-bold">&vert;</span>
				<div class="flex items-baseline text-sm">
					<span>{walkingSections}</span>

					<div class="flex items-center">
						<span>x</span>
						<Walking height="25px" width="25px" class="stroke-accent" />
					</div>
				</div>
			{/if}
		</div>

		<!-- Sections -->
		<div class="flex w-full flex-row gap-x-2 overflow-x-auto">
			{#each route.sections.filter((section) => !section.isWalking) as _sec}
				{@const section = _sec as NormalSection}
				<span
					class="bg-primary/25 min-w-fit truncate rounded-lg px-2 py-1 text-center text-base font-medium md:w-auto md:text-lg"
					style:width={getWidthRatio(durationOfSection(section), durationWithoutWalking)}
				>
					{section.lineInformation.journeyName}
				</span>
			{/each}
		</div>

		<!-- Details Button -->
		<div class="flex justify-center">
			<button
				class="flex cursor-pointer flex-row items-center gap-x-2"
				onclick={() => (detailsExpanded = !detailsExpanded)}
			>
				<span class="text-sm md:text-lg">Details</span>
				{#if !detailsExpanded}
					<ChevronDown color="#ffda0a" />
				{:else}
					<ChevronUp color="#ffda0a" />
				{/if}
			</button>
		</div>
	</div>

	<!-- Details -->
</div>
