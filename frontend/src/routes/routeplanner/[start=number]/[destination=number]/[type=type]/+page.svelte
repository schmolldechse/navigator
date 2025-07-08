<script lang="ts">
	import RoutePlannerHeader from "$components/route-planner/RoutePlannerHeader.svelte";
	import type { RouteDetails, RouteEntry as SingleRouteEntry, Station } from "$models/models";
	import { MetaTags } from "svelte-meta-tags";
	import type { PageProps } from "./$types";
	import SpinningCircle from "$components/ui/icons/SpinningCircle.svelte";
	import ArrowDown from "@lucide/svelte/icons/arrow-down";
	import ArrowUp from "@lucide/svelte/icons/arrow-up";
	import { env } from "$env/dynamic/public";
	import { page } from "$app/state";
	import { DateTime } from "luxon";
	import RouteEntrySkeleton from "$components/route-planner/RouteEntrySkeleton.svelte";
	import RouteEntriesEmptyWarning from "$components/route-planner/RouteEntriesEmptyWarning.svelte";
	import RouteEntry from "$components/route-planner/RouteEntry.svelte";

	let { data }: PageProps = $props();

	let plannedStations = $state<{ start?: Station; destination?: Station }>({
		start: undefined,
		destination: undefined
	});
	data.stations.then(({ start, destination }: { start: Station; destination: Station }) => {
		plannedStations.start = start;
		plannedStations.destination = destination;
	});

	let plannedRoute = $state<RouteDetails | undefined>(undefined);
	data.route.then((route: RouteDetails) => {
		plannedRoute = route;
		// fetchLineColors();
	});

	let loadingEarlier = $state<boolean>(false);
	let loadingLater = $state<boolean>(false);

	let isDeparture = page.params.type === "departures";
	const requestRoutes = async (earlier: boolean) => {
		if (!plannedStations?.start || !plannedStations?.destination) return;
		if (!plannedRoute?.earlierRef || !plannedRoute?.laterRef) return;

		let reference = earlier ? plannedRoute.earlierRef : plannedRoute.laterRef;
		if (earlier) loadingEarlier = true;
		else loadingLater = true;

		const request = await fetch(`${env.PUBLIC_BACKEND_URL}/api/route`, {
			method: "POST",

			headers: {
				"Content-Type": "application/json"
			},
			body: JSON.stringify({
				from: plannedStations.start.evaNumber,
				to: plannedStations.destination.evaNumber,
				type: data.filter.type === "departures" ? "departure" : "arrival",
				when: data.filter.date,
				filter: {
					disabledProducts: data.disabledProducts ?? [],
					changeover: {
						maxAmount: -1,
						minDuration: -1
					},
					onlyFastRoutes: false
				},
				reference: reference
			})
		});
		if (!request.ok) {
			if (earlier) loadingEarlier = false;
			else loadingLater = false;
			return;
		}
		const result = (await request.json()) as RouteDetails;

		if (earlier) plannedRoute.earlierRef = result.earlierRef;
		else plannedRoute.laterRef = result.laterRef;

		// update journeys by taking a snapshot of the current journeys and appending the new ones
		const seenTokens = new Set<string>();
		plannedRoute.entries = $state
			.snapshot(plannedRoute.entries)
			.concat(result.entries)
			.filter((routeEntry: SingleRouteEntry) => {
				if (!routeEntry.refreshToken) return false;
				if (seenTokens.has(routeEntry.refreshToken)) return false;
				seenTokens.add(routeEntry.refreshToken);
				return true;
			})
			.sort((a: SingleRouteEntry, b: SingleRouteEntry) => {
				const aDeparture = DateTime.fromISO(a.sections[0].origin.departure!.actualTime);
				const bDeparture = DateTime.fromISO(b.sections[0].origin.departure!.actualTime);
				return aDeparture.valueOf() - bDeparture.valueOf();
			});

		if (earlier) loadingEarlier = false;
		else loadingLater = false;

		// await fetchLineColors();
	};

	const isDayDifferent = (index: number): boolean => {
		if (index <= 0) return false;

		const prevJourney = plannedRoute?.entries[index - 1];
		const currentJourney = plannedRoute?.entries[index];

		if (!prevJourney || !currentJourney) return false;

		const prevDeparture = DateTime.fromISO(
			(prevJourney.sections[0].origin.arrival ?? prevJourney.sections[0].origin.departure)?.actualTime ?? ""
		);
		const departure = DateTime.fromISO(
			(currentJourney.sections[0].origin.arrival ?? currentJourney.sections[0].origin.departure)?.actualTime ?? ""
		);

		if (!prevDeparture || !departure) return false;
		return prevDeparture.day !== departure.day;
	};

	/**
     * 
	let lineColors = $state<LineColor[]>([]);
	const fetchLineColors = async () => {
		const legs = (plannedRoute?.journeys ?? []).filter((route: RouteInfo) =>
			route?.legs?.some((leg: Connection) => !leg?.walking)
		);
		if (legs.length === 0) return [];

		const params = new URLSearchParams({
			line: legs.map((leg: Connection) => leg?.lineInformation?.lineName).join(";"),
			hafasOperatorCode: legs.map((leg: Connection) => leg?.lineInformation?.operator?.id).join(";")
		});

		const request = await fetch(`${env.PUBLIC_BACKEND_BASE_URL}/api/v1/journey/color?${params}`);
		if (!request.ok) return [];

		lineColors = (await request.json()) as LineColor[];
	};
    */
</script>

<MetaTags
	title={`Route Planner`}
	description={`Plan your journey`}
	openGraph={{
		url: "https://navigator.voldechse.wtf/",
		title: "Navigator",
		siteName: "Navigator",
		description: "The Navigator for your train journeys.",
		images: [
			{
				url: "https://navigator.voldechse.wtf/logo.png",
				width: 1024,
				height: 1024,
				alt: "Navigator Logo"
			}
		]
	}}
/>

<div class="flex min-h-screen flex-col gap-y-2 px-2 md:px-0">
	<RoutePlannerHeader stations={data.stations} />

	<button
		class="container mx-auto flex cursor-pointer flex-row items-center gap-x-2"
		disabled={loadingEarlier}
		onclick={async () => await requestRoutes(true)}
	>
		<span class="text-base md:text-lg">Earlier connections</span>
		{#if loadingEarlier}<SpinningCircle height="20px" width="20px" />
		{:else}<ArrowUp color="#ffda0a" />{/if}
	</button>

	{#await data.route}
		{#each Array(5) as _}
			<RouteEntrySkeleton />
		{/each}
	{:then _}
		{#each plannedRoute?.entries ?? [] as route, index}
			{#if isDayDifferent(index)}
				{@const date = DateTime.fromISO(route.sections[0].origin.departure?.plannedTime ?? "")}

				<div class="container my-2 self-center px-4 md:px-8">
					<span class="border-text/50 text-text block w-full border-b text-lg font-medium md:text-xl">
						{date.toLocaleString({ weekday: "short", day: "numeric", month: "short", year: "numeric" })}
					</span>
				</div>
			{/if}
			<RouteEntry {route} />
		{:else}<RouteEntriesEmptyWarning />
		{/each}
	{/await}

	<button
		class="container mx-auto flex cursor-pointer flex-row items-center gap-x-2"
		disabled={loadingLater}
		onclick={async () => await requestRoutes(false)}
	>
		<span class="text-base md:text-lg">Later connections</span>
		{#if loadingLater}<SpinningCircle height="20px" width="20px" />
		{:else}<ArrowDown color="#ffda0a" />{/if}
	</button>
</div>
