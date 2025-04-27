<script lang="ts">
	import type { PageProps } from "./$types";
	import { MetaTags } from "svelte-meta-tags";
	import type { Station } from "$models/station";
	import type { Route as RouteInfo, RouteData } from "$models/route";
	import { env } from "$env/dynamic/public";
	import RouteRequest from "$components/route-planner/RouteRequest.svelte";
	import Skeleton from "$components/route-planner/Skeleton.svelte";
	import ArrowDown from "lucide-svelte/icons/arrow-down";
	import ArrowUp from "lucide-svelte/icons/arrow-up";
	import WarningNoRoutes from "$components/route-planner/WarningNoRoutes.svelte";
	import Route from "$components/route-planner/Route.svelte";
	import { DateTime } from "luxon";
	import SpinningCircle from "$components/ui/icons/SpinningCircle.svelte";
	import DateHeader from "$components/ui/info/DateHeader.svelte";
	import type { Connection, LineColor } from "$models/connection";

	let { data }: PageProps = $props();

	let plannedStations = $state<{ from?: Station; to?: Station }>({ from: undefined, to: undefined });
	data.stations.then((stations) => (plannedStations = stations));

	let plannedRoute = $state<RouteData | undefined>(undefined);
	data.route.then((route) => {
		plannedRoute = route;
		fetchLineColors();
	});

	let loadingEarlier = $state<boolean>(false);
	let loadingLater = $state<boolean>(false);

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

	const requestRoutes = async (earlier: boolean) => {
		if (!plannedStations?.from || !plannedStations?.to) return;
		if (!plannedRoute?.earlierRef || !plannedRoute?.laterRef) return;

		if (earlier) loadingEarlier = true;
		else loadingLater = true;
		const urlParams = new URLSearchParams(window.location.search);

		const params = new URLSearchParams({
			results: "5",
			from: String(plannedStations.from.evaNumber),
			to: String(plannedStations.to.evaNumber),
			...(urlParams.has("disabledProducts") && { disabledProducts: urlParams.get("disabledProducts")! }),
			...(earlier && { earlierThan: plannedRoute.earlierRef }),
			...(!earlier && { laterThan: plannedRoute.laterRef })
		});

		const request = await fetch(`${env.PUBLIC_BACKEND_BASE_URL}/api/v1/journey/route-planner?${params.toString()}`, {
			method: "GET"
		});
		if (!request.ok) {
			if (earlier) loadingEarlier = false;
			else loadingLater = false;
			return;
		}
		const result = (await request.json()) as RouteData;

		if (earlier) plannedRoute.earlierRef = result.earlierRef;
		else plannedRoute.laterRef = result.laterRef;

		// update journeys by taking a snapshot of the current journeys and appending the new ones
		const seenTokens = new Set<string>();
		plannedRoute.journeys = $state
			.snapshot(plannedRoute.journeys)
			.concat(result.journeys)
			.filter((routeInfo: RouteInfo) => {
				if (!routeInfo.refreshToken) return false;
				if (seenTokens.has(routeInfo.refreshToken)) return false;
				seenTokens.add(routeInfo.refreshToken);
				return true;
			})
			.sort((a: RouteInfo, b: RouteInfo) => {
				const aDeparture = DateTime.fromISO(a.legs[0].departure!.actualTime);
				const bDeparture = DateTime.fromISO(b.legs[0].departure!.actualTime);
				return aDeparture.valueOf() - bDeparture.valueOf();
			});

		if (earlier) loadingEarlier = false;
		else loadingLater = false;

		await fetchLineColors();
	};

	const isDayDifferent = (index: number): boolean => {
		if (index <= 0) return false;

		const prevJourney = plannedRoute?.journeys[index - 1];
		const currentJourney = plannedRoute?.journeys[index];

		if (!prevJourney || !currentJourney) return false;

		const prevDeparture = DateTime.fromISO(
			prevJourney.legs[0].departure?.actualTime ?? prevJourney.legs[0].departure?.plannedTime ?? ""
		);
		const departure = DateTime.fromISO(
			currentJourney.legs[0].departure?.actualTime ?? currentJourney.legs[0].departure?.plannedTime ?? ""
		);

		if (!prevDeparture || !departure) return false;
		return prevDeparture.day !== departure.day;
	};
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

<div class="mx-auto mb-2 flex flex-1 flex-col gap-y-4 w-full px-2 md:px-4 md:max-w-[960px]">
	<!-- Route Request Info -->
	<RouteRequest stations={data?.stations} />

	{#await data.route}
		{#each Array(5) as _}
			<Skeleton />
		{/each}
	{:then _}
		<button
			class="flex cursor-pointer flex-row items-center gap-x-2"
			disabled={loadingEarlier}
			onclick={async () => await requestRoutes(true)}
		>
			<span class="text-base md:text-lg">Earlier connections</span>
			{#if loadingEarlier}<SpinningCircle height="20px" width="20px" />
			{:else}<ArrowUp color="#ffda0a" />{/if}
		</button>

		<div class="gap-y-2">
			{#each plannedRoute?.journeys ?? [] as route, index}
				{#if isDayDifferent(index)}
					{@const date = DateTime.fromISO(
						route.legs[0].departure?.actualTime ?? route.legs[0].departure?.plannedTime ?? ""
					)}
					<DateHeader {date} />
				{/if}
				<Route {route} {lineColors} />
			{:else}<WarningNoRoutes />
			{/each}
		</div>

		<button
			class="flex cursor-pointer flex-row items-center gap-x-2"
			disabled={loadingLater}
			onclick={async () => await requestRoutes(false)}
		>
			<span class="text-base md:text-lg">Later connections</span>
			{#if loadingLater}<SpinningCircle height="20px" width="20px" />
			{:else}<ArrowDown color="#ffda0a" />{/if}
		</button>
	{/await}
</div>
