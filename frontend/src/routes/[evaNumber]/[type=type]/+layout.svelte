<script lang="ts">
	import "$src/app.css";
	import Navbar from "$components/Navbar.svelte";
	import type { LayoutProps } from "./$types";
	import { MetaTags } from "svelte-meta-tags";
	import Clock from "$components/timetable/Clock.svelte";
	import { CornerDownRight, Star } from "lucide-svelte";
	import { authClient } from "$lib/auth-client";
	import { page } from "$app/state";
	import { getContext, setContext } from "svelte";
	import type { Station } from "$models/station";

	let { children, data }: LayoutProps = $props();
	setContext<Station>("station", data.station);
	let station = getContext<Station>("station");

	setContext<boolean>("isDeparture", page.params.type === "departures");
	let isDeparture = getContext<boolean>("isDeparture");

	const session = authClient.useSession();

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

<div class="flex min-h-screen flex-col">
	<Navbar />

	<div class="bg-background sticky top-20 z-10 container mx-auto w-full flex-col px-6 pt-4">
		<div class="flex justify-between">
			{#snippet favoriteStation()}
				<div class="flex flex-row break-all items-baseline gap-x-2">
					{#if $session.data}
						<Star class="shrink-0" />
					{/if}
					<span class="text-xl font-semibold break-words whitespace-normal md:text-4xl">{station.name}</span>
				</div>
			{/snippet}

			{@render favoriteStation()}
			<Clock />
		</div>

		<button class="flex gap-x-2 group relative cursor-pointer text-base md:text-xl" onclick={navigate}>
			<CornerDownRight />
			Show {isDeparture ? "Arrivals" : "Departures"}
			<span
				class="bg-accent absolute bottom-0 left-0 h-0.5 w-full scale-x-0 transform transition-transform duration-300 group-hover:scale-x-100"
			></span>
		</button>
	</div>

	{@render children()}
</div>
