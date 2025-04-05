<script lang="ts">
	import type { Route } from "$models/route";
	import Minus from "lucide-svelte/icons/minus";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import { calculateDuration, durationOfConnection, formatDuration } from "$lib";
	import { DateTime } from "luxon";
	import type { LineColor } from "$models/connection";
	import { env } from "$env/dynamic/public";
	import ChevronDown from "lucide-svelte/icons/chevron-down";
	import ChevronUp from "lucide-svelte/icons/chevron-up";
	import LegInfo from "$components/route-planner/LegInfo.svelte";
	import { onMount } from "svelte";
	import Changeover from "$components/route-planner/Changeover.svelte";
	import Walking from "$components/ui/icons/Walking.svelte";

	let { route }: { route: Route } = $props();
	let detailsOpen = $state<boolean>(false);

	const durationWithoutWalking = $derived(() => {
		// remove legs with "walking" property
		const legs = (route?.legs || []).filter((leg) => !leg?.walking);
		if (legs.length === 0) return 0;

		return legs
			.flatMap((leg) =>
				calculateDuration(
					DateTime.fromISO(leg?.arrival?.actualTime ?? leg?.arrival?.plannedTime ?? ""),
					DateTime.fromISO(leg?.departure?.actualTime ?? leg?.departure?.plannedTime ?? ""),
					["minutes"]
				).as("minutes")
			)
			.reduce((acc, el) => acc + el);
	});

	const getWidthRatio = (duration: number, maxDuration: number) => {
		if (maxDuration <= 0) return "0%";
		return `${(duration / maxDuration) * 100}%`;
	};

	let legColors = $state<LineColor[]>([]);
	onMount(() => {
		const fetchLineColors = async () => {
			const params = new URLSearchParams({
				line: route?.legs
					.filter((leg) => !leg?.walking)
					.map((leg) => leg?.lineInformation?.lineName)
					.join(";"),
				hafasOperatorCode: route?.legs
					.filter((leg) => !leg?.walking)
					.map((leg) => leg?.lineInformation?.operator?.id)
					.join(";")
			});

			const request = await fetch(`${env.PUBLIC_BACKEND_BASE_URL}/api/v1/journey/color?${params}`);
			if (!request.ok) return;

			legColors = (await request.json()) as LineColor[];
		};

		fetchLineColors();
	});

	const normalize = (value: string): string => value.toLowerCase().replace(/[^a-zA-Z0-9]+/g, "");
</script>

<div class="border-primary-dark/75 space-y-2 rounded-lg border-2 px-4 py-2 text-2xl font-medium">
	<!-- Time Info -->
	<div class="flex flex-row items-baseline gap-x-2">
		<div class="flex flex-row">
			<TimeInformation time={route?.legs[0]?.departure} direction="col" class="text-xl" delayClass="text-sm md:text-lg" />
			<Minus class="mx-2 mt-[0.15rem]" />
			<TimeInformation
				time={route?.legs[route?.legs?.length - 1]?.arrival}
				direction="col"
				class="text-xl"
				delayClass="text-sm md:text-lg"
			/>
		</div>

		<span class="text-primary/90">|</span>
		<span class="mt-[0.35rem] text-sm md:text-lg">
			{formatDuration(route?.legs[route?.legs?.length - 1]?.arrival, route?.legs[0]?.departure)}
		</span>

		{#if route?.legs?.filter((leg) => leg?.walking).length > 0}
			<span class="text-primary/90">|</span>
			<div class="mt-[0.35rem] flex items-baseline text-sm md:text-lg">
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
				class="bg-primary-darker line-clamp-1 min-w-fit truncate rounded-lg px-2 py-1 text-center text-base md:line-clamp-none md:text-lg"
				style:width={getWidthRatio(durationOfConnection(leg), durationWithoutWalking())}
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

	{#if detailsOpen}
		<div class="border-primary-dark/75 flex flex-col border-t">
			<span class="text-lg font-semibold">Route Details</span>

			{#each route?.legs as leg, i}
				{#if leg?.walking}
					<!--
					firstIsWalking : i === 0
					If true, it swaps the arrival & departure times.
					-> Arrival is then the time at which you arrive at the stop[i + 1]
					-> Departure is then the time at which you set off from the stop[i]
					 -->
					{@const firstIsWalking = i === 0}
					<Changeover
						arrival={firstIsWalking ? route?.legs[i + 1]?.departure : route?.legs[i - 1]?.arrival}
						departure={firstIsWalking ? leg?.departure : route?.legs[i + 1]?.departure}
						{firstIsWalking}
						origin={firstIsWalking ? leg?.origin : undefined}
					/>
				{:else}
					<LegInfo
						{leg}
						lineColor={legColors.find(
							(color) => normalize(color.lineName) === normalize(leg?.lineInformation?.lineName ?? "")
						)}
					/>
				{/if}
			{/each}
		</div>
	{/if}
</div>
