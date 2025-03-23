<script lang="ts">
	import type { Route } from "$models/route";
	import Minus from "lucide-svelte/icons/minus";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import calculateDuration from "$lib/time";
	import { DateTime } from "luxon";
	import type { Connection, LineColor } from "$models/connection";
	import { env } from "$env/dynamic/public";
	import ChevronDown from "lucide-svelte/icons/chevron-down";
	import ChevronUp from "lucide-svelte/icons/chevron-up";

	let { route }: { route: Route } = $props();
	let detailsOpen = $state<boolean>(false);

	const formatDuration = (): string => {
		const end = route?.legs[route?.legs?.length - 1];
		const start = route?.legs[0];

		const duration = calculateDuration(
			DateTime.fromISO(end?.arrival?.actualTime ?? end?.arrival?.plannedTime ?? ""),
			DateTime.fromISO(start?.departure?.actualTime ?? start?.departure?.plannedTime ?? ""),
			["minutes"]
		).as("minutes");

		if (duration > 60) {
			const hours = Math.floor(duration / 60);
			const minutes = duration % 60;
			return `${hours} h ${minutes} min`;
		} else return `${Math.floor(duration)} min`;
	};

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

	const durationByConnection = (connection: Connection) =>
		calculateDuration(
			DateTime.fromISO(connection?.arrival?.actualTime ?? connection?.arrival?.plannedTime ?? ""),
			DateTime.fromISO(connection?.departure?.actualTime ?? connection?.departure?.plannedTime ?? ""),
			["minutes"]
		).as("minutes");

	const getWidthRatio = (duration: number, maxDuration: number) => {
		if (maxDuration <= 0) return "0%";
		return `${(duration / maxDuration) * 100}%;`;
	};

	let legColors = $state<LineColor[]>([]);
	$effect(() => {
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

	const getLineColor = (lineName?: string): LineColor | undefined => legColors.find((color) => color.lineName === lineName);
</script>

<div class="border-primary-dark/75 space-y-2 rounded-lg border-2 px-4 py-2 text-2xl font-medium">
	<!-- Time Info -->
	<div class="flex flex-row gap-x-2">
		<div class="flex flex-row">
			<TimeInformation time={route?.legs[0]?.departure} direction="col" />
			<Minus class="mx-2 mt-[0.25rem]" />
			<TimeInformation time={route?.legs[route?.legs?.length - 1]?.arrival} direction="col" />
		</div>

		<span class="text-primary/90">|</span>
		<span class="mt-[0.35rem] text-lg">{formatDuration()}</span>
	</div>

	<!-- Legs -->
	<div class="flex w-full flex-row gap-x-2">
		{#each route?.legs.filter((leg) => !leg?.walking) as leg}
			<span
				class="bg-primary-darker line-clamp-1 truncate rounded-lg px-2 py-1 text-center text-base md:line-clamp-none md:text-lg"
				style={`width: ${getWidthRatio(durationByConnection(leg), durationWithoutWalking())}; min-width: 5em`}
			>
				{leg?.lineInformation?.lineName}
			</span>
		{/each}
	</div>

	<!-- TODO: Details -->
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
		<div class="border-primary-dark/75 border-t">
			<span class="text-lg font-semibold">Route Details</span>
		</div>
	{/if}
</div>
