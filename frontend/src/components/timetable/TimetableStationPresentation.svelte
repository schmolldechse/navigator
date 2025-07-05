<script lang="ts">
	import type { Station } from "$models/models";
	import { MetaTags } from "svelte-meta-tags";
	import Clock from "./Clock.svelte";
	import { client } from "$src/lib/auth/client";
	import Star from "lucide-svelte/icons/star";
	import { env } from "$env/dynamic/public";
	import CornerDownRight from "lucide-svelte/icons/corner-down-right";
	import { page } from "$app/state";
	import { goto } from "$app/navigation";

	interface Props {
		station: Promise<Station>;
	}

	let { station }: Props = $props();
	let isFavored = $state<boolean>(false);

	// better-auth
	const session = client.useSession();
	session.subscribe((listener) => {
		if (listener.isPending) return;
		if (!listener.data) return;

		(async () => {
			const request = await fetch(`${env.PUBLIC_BACKEND_URL}/api/user/favored/${(await station).evaNumber}`, {
				method: "GET",
				credentials: "include"
			});
			if (!request.ok) return;
			isFavored = (await request.json()).favored;
		})();
	});

	const toggleFavour = async () => {
		const request = await fetch(`${env.PUBLIC_BACKEND_URL}/api/user/favored/${(await station).evaNumber}`, {
			method: "POST",
			credentials: "include"
		});
		if (!request.ok) return;
		isFavored = (await request.json()).favored;
	};

	let isDeparture = page.params.type === "departures";
	const navigate = async () => {
		const type = isDeparture ? "arrivals" : "departures";
		const startDate = page.url.searchParams.get("when") ?? new Date().toISOString();

		await goto(`/timetable/${(await station).evaNumber}/${type}?when=${encodeURIComponent(startDate)}`, {
			invalidateAll: true
		});
	};
</script>

{#await station}
	<MetaTags
		title="Timetable loading..."
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

	<div class="container mx-auto flex items-center justify-between">
		<span class="h-[35px] w-[25%] animate-pulse rounded-2xl bg-stone-200/45 px-6 py-2 text-2xl font-bold"></span>
		<Clock />
	</div>
{:then station}
	<MetaTags
		title={`Timetable for ${station.name}`}
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

	<div class="container mx-auto w-full">
		<div class="flex items-start justify-between">
			<div class="flex flex-row gap-x-2 break-all">
				{#if $session.data}
					<Star
						class="shrink-0 cursor-pointer transition-all duration-300 {isFavored
							? 'fill-accent stroke-yellow-400'
							: 'hover:stroke-accent fill-transparent'}"
						onclick={toggleFavour}
					/>
				{/if}
				<span class="whitespace-normal break-normal text-xl font-semibold md:text-4xl">{station.name}</span>
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
{/await}
