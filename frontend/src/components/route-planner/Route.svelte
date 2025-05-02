<script lang="ts">
	import type { Route } from "$models/route";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import { calculateDuration, formatDuration } from "$lib";
	import { DateTime } from "luxon";
	import type { Connection, LineColor } from "$models/connection";
	import ChevronDown from "lucide-svelte/icons/chevron-down";
	import ChevronUp from "lucide-svelte/icons/chevron-up";
	import Ban from "lucide-svelte/icons/ban";
	import Minus from "lucide-svelte/icons/minus";
	import LegInfo from "$components/route-planner/details/LegInfo.svelte";
	import Changeover from "$components/route-planner/details/Changeover.svelte";
	import Walking from "$components/ui/icons/Walking.svelte";

	let { route, lineColors }: { route: Route; lineColors: LineColor[] } = $props();
	let detailsOpen = $state<boolean>(false);

	const durationOfConnection = (connection: Connection): number =>
		calculateDuration(
			DateTime.fromISO(connection?.arrival?.actualTime ?? connection?.arrival?.plannedTime ?? ""),
			DateTime.fromISO(connection?.departure?.actualTime ?? connection?.departure?.plannedTime ?? ""),
			["minutes"]
		).minutes;

	const durationWithoutWalking = $derived.by(() => {
		// remove legs with "walking" property
		const legs = (route?.legs || []).filter((leg) => !leg?.walking);
		if (legs.length === 0) return 0;

		return legs.flatMap((leg) => durationOfConnection(leg)).reduce((acc, el) => acc + el);
	});

	const isCancelled: boolean = $derived(route?.legs?.some((leg: Connection) => leg?.cancelled));

	const getWidthRatio = (duration: number, maxDuration: number) => {
		if (maxDuration <= 0) return "0%";
		return `${(duration / maxDuration) * 100}%`;
	};

	const normalize = (value: string): string => value.toLowerCase().replace(/[^a-zA-Z0-9]+/g, "");
</script>

<div class="border-primary/45 overflow-hidden rounded-lg border-2">
	<!-- Cancelled Header -->
	{#if isCancelled}
		<div class="bg-secondary/50 flex flex-row items-center gap-x-2 px-3 py-1">
			<Ban size="20" />
			<span class="text-sm font-semibold md:text-lg">Route not possible</span>
		</div>
	{/if}

	<div class="space-y-2 px-4 py-2" class:opacity-65={isCancelled}>
		<!-- Time Info -->
		<div class="flex flex-row items-baseline gap-x-2 tracking-tight">
			<div class="flex flex-row gap-x-1">
				<TimeInformation
					time={route?.legs[0]?.departure}
					direction="col"
					class="text-base font-semibold md:text-xl"
					delayClass="text-sm md:text-base"
				/>
				<Minus class="mt-[0.05rem] md:mt-[0.15rem]" />
				<TimeInformation
					time={route?.legs[route?.legs?.length - 1]?.arrival}
					direction="col"
					class="text-base font-semibold md:text-xl"
					delayClass="text-sm md:text-base"
				/>
			</div>

			<span class="text-text/50 font-medium">|</span>
			<span class="text-sm">
				{formatDuration(route?.legs[route?.legs?.length - 1]?.arrival, route?.legs[0]?.departure)}
			</span>

			{#if route?.legs?.filter((leg) => leg?.walking).length > 0}
				<span class="text-text/50 font-medium">|</span>
				<div class="flex items-baseline text-sm">
					<span>{route?.legs?.filter((leg) => leg?.walking).length}</span>

					<div class="flex items-center">
						<span>x</span>
						<Walking height="25px" width="25px" class="stroke-accent" />
					</div>
				</div>
			{/if}
		</div>

		<!-- Legs -->
		<div class="accent-scrollbar flex w-full flex-row gap-x-2 overflow-x-auto">
			{#each route?.legs.filter((leg) => !leg?.walking) as leg}
				<span
					class="bg-primary/25 min-w-fit truncate rounded-lg px-2 py-1 text-center text-base font-medium md:text-lg"
					style:width={getWidthRatio(durationOfConnection(leg), durationWithoutWalking)}
				>
					{leg?.lineInformation?.lineName}
				</span>
			{/each}
		</div>

		<div class="flex justify-center">
			<button class="flex cursor-pointer flex-row items-center gap-x-2" onclick={() => (detailsOpen = !detailsOpen)}>
				<span class="text-sm md:text-lg">Details</span>
				{#if !detailsOpen}
					<ChevronDown color="#ffda0a" />
				{:else}
					<ChevronUp color="#ffda0a" />
				{/if}
			</button>
		</div>
	</div>

	{#if detailsOpen}
		<div class="border-primary/85 flex flex-col border-t px-1 py-4">
			<span class="px-4 pb-4 text-lg font-semibold tracking-tighter md:pb-8">Route Details</span>

			{#each route?.legs as leg, i}
				{#if leg?.walking}
					{@const firstIsWalking = i === 0}
					{@const lastIsWalking = i === route?.legs.length - 1}

					{#if firstIsWalking}
						<!--
						startWalking is the time, at which you set off from the stop (leg?.departure)
						stopWalking is the time, at which the next leg starts (route?.legs[i + 1]?.departure)
						-->
						<Changeover
							startWalking={leg?.departure}
							stopWalking={route?.legs[i + 1]?.departure}
							firstIsWalking={leg?.origin}
						/>
					{:else if lastIsWalking}
						<!--
						startWalking is the time, at which you set off from the stop (leg?.departure)
						stopWalking is the time, at which you arrive at the destination (leg?.arrival)
						-->
						<Changeover startWalking={leg?.departure} stopWalking={leg?.arrival} lastIsWalking={leg?.destination} />
					{:else}
						<!--
						startWalking is the time at which you arrive at the last stop (route?.legs[i - 1]?.arrival)
						stopWalking is the time at which you depart at the next stop (route?.legs[i + 1]?.departure)
						-->
						<Changeover startWalking={route?.legs[i - 1]?.arrival} stopWalking={route?.legs[i + 1]?.departure} />
					{/if}
				{:else}
					<LegInfo
						{leg}
						lineColor={lineColors.find(
							(color: LineColor) => normalize(color.lineName) === normalize(leg?.lineInformation?.lineName ?? "")
						)}
					/>
				{/if}
			{/each}
		</div>
	{/if}
</div>
