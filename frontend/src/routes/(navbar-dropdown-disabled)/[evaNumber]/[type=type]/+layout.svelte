<script lang="ts">
	import "$src/app.css";
	import type { LayoutProps } from "./$types";
	import { MetaTags } from "svelte-meta-tags";
	import Clock from "$components/timetable/Clock.svelte";
	import CornerDownRight from "lucide-svelte/icons/corner-down-right";
	import Star from "lucide-svelte/icons/star";
	import { authClient } from "$lib/auth-client";
	import { page } from "$app/state";
	import { getContext, onMount, setContext } from "svelte";
	import type { Station } from "$models/station";
	import { env } from "$env/dynamic/public";

	let { children, data }: LayoutProps = $props();
	setContext<Station>("station", data.station);
	let station = getContext<Station>("station");

	setContext<boolean>("isDeparture", page.params.type === "departures");
	let isDeparture = getContext<boolean>("isDeparture");

	const session = authClient.useSession();

	let favor = $state<boolean>(false);
	onMount(() => {
		if (!$session?.data) return;

		const checkFavored = async () => {
			const request = await fetch(`${env.PUBLIC_BACKEND_BASE_URL}/api/v1/user/station/favored/${station?.evaNumber}`, {
				method: "GET",
				credentials: "include"
			});
			if (!request.ok) return;
			favor = (await request.json()).favored;
		};

		checkFavored();
	});

	const toggleFavour = async () => {
		if (!$session?.data) return;

		const request = await fetch(`${env.PUBLIC_BACKEND_BASE_URL}/api/v1/user/station/favor/${station?.evaNumber}`, {
			method: "POST",
			credentials: "include"
		});
		if (!request.ok) return;
		favor = (await request.json()).favored;
	};

	const navigate = async () => {
		const type = isDeparture ? "arrivals" : "departures";
		const startDate = page.url.searchParams.get("startDate") ?? new Date().toISOString();

		// TODO: inspect why "goto" does not reload the page, even using invalidateAll
		window.location.href = `/${station?.evaNumber}/${type}?startDate=${encodeURIComponent(startDate)}`;
	};
</script>

<MetaTags
	title={station.name}
	description="The Navigator for your train journeys."
	openGraph={{
		url: "https://navigator.voldechse.wtf",
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

<div class="bg-background sticky top-20 z-10 container mx-auto w-full flex-col px-6 pt-4">
	<div class="flex items-start justify-between">
		<div class="flex flex-row items-baseline gap-x-2 break-all">
			{#if $session.data}
				<Star
					class="shrink-0 cursor-pointer transition-all duration-300 {favor
						? 'fill-accent stroke-yellow-400'
						: 'hover:stroke-accent fill-transparent'}"
					onclick={toggleFavour}
				/>
			{/if}
			<span class="text-xl font-semibold break-normal whitespace-normal md:text-4xl">{station.name}</span>
		</div>
		<Clock />
	</div>

	<button class="group relative flex cursor-pointer gap-x-2 text-base md:text-xl" onclick={navigate}>
		<CornerDownRight />
		Show {isDeparture ? "Arrivals" : "Departures"}
		<span
			class="bg-accent absolute bottom-0 left-0 h-0.5 w-full scale-x-0 transform transition-transform duration-300 group-hover:scale-x-100"
		></span>
	</button>
</div>

{@render children()}
