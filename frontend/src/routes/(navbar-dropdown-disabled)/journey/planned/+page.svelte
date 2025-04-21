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

	let { data }: PageProps = $props();

	let plannedStations = $state<{ from?: Station; to?: Station }>({ from: undefined, to: undefined });
	data.stations.then((stations) => plannedStations = stations);

	let plannedRoute = $state<RouteData | undefined>(undefined);
	data.route.then((route) => plannedRoute = route);

	const requestRoutes = async (earlier: boolean) => {
		if (!plannedStations?.from || !plannedStations?.to) return;
		if (!plannedRoute?.earlierRef || !plannedRoute?.laterRef) return;

		// loading = true;
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
			// loading = false;
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
		// loading = false;
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

<div class="mx-auto flex min-h-full w-full flex-1 flex-col gap-y-4 px-2 my-4 md:max-w-[65%] md:px-0">
	<!-- Route Request Info -->
	<RouteRequest stations={data?.stations} />

	{#await data.route}
		{#each Array(5) as _, i}
			<Skeleton />
		{/each}
	{:then route}
		<button
			class="flex cursor-pointer flex-row items-center gap-x-2"
			disabled={false}
			onclick={async () => await requestRoutes(true)}
		>
			<span class="text-lg">Earlier connections</span>
			<ArrowUp color="#ffda0a" />
		</button>

		{#if (plannedRoute?.journeys?.length ?? 0) === 0}
			<WarningNoRoutes />
		{:else}
			<div class="space-y-2">
				{#each plannedRoute?.journeys ?? [] as route}
					<Route {route} />
				{/each}
			</div>
		{/if}

		<button
			class="flex cursor-pointer flex-row items-center gap-x-2"
			disabled={false}
			onclick={async () => await requestRoutes(false)}
		>
			<span class="text-lg">Later connections</span>
			<ArrowDown color="#ffda0a" />
		</button>
	{/await}
</div>
