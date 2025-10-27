<script lang="ts">
	import type { StationSummaryDTO } from "$lib/models/StationSummaryDTO";
	import Activity from "@lucide/svelte/icons/activity";
	import X from "@lucide/svelte/icons/x";
	import Clock_4 from "@lucide/svelte/icons/clock-4";
	import { getStationGatheringInfo } from "../../../routes/(navigation)/maps/stations.remote";
	import { DateTime } from "luxon";
	import type { StationGatheringInfoDTO } from "$lib/models/StationGatheringInfoDTO";
	import { TransportType } from "$lib/models/TransportType";
	import type { Component } from "svelte";
	import LongDistance from "../icons/transport-types/LongDistance.svelte";
	import Regional from "../icons/transport-types/Regional.svelte";
	import Suburban from "../icons/transport-types/Suburban.svelte";
	import Bus from "../icons/transport-types/Bus.svelte";
	import Subway from "../icons/transport-types/Subway.svelte";
	import Tram from "../icons/transport-types/Tram.svelte";
	import Taxi from "../icons/transport-types/Taxi.svelte";

	let { station = $bindable() }: { station: StationSummaryDTO | null } = $props();

	let gatheringInfo: StationGatheringInfoDTO | null = $derived(
		await getStationGatheringInfo({ evaNumber: station!.evaNumber })
	);

	interface TransportVisualizationGroup {
		label: string;
		component?: Component;
		values: TransportType[];
	}

	const transportGroups: TransportVisualizationGroup[] = [
		{
			label: "High-Speed",
			component: LongDistance,
			values: [TransportType.HighSpeedTrain, TransportType.IntercityTrain]
		},
		{
			label: "Regional",
			component: Regional,
			values: [TransportType.RegionalTrain, TransportType.InterRegionalTrain]
		},
		{
			label: "Suburban",
			component: Suburban,
			values: [TransportType.CityTrain]
		},
		{
			label: "Bus",
			component: Bus,
			values: [TransportType.Bus]
		},
		{
			label: "Subway",
			component: Subway,
			values: [TransportType.Subway]
		},
		{
			label: "Tram",
			component: Tram,
			values: [TransportType.Tram]
		},
		{
			label: "Taxi / Shuttle",
			component: Taxi,
			values: [TransportType.Taxi, TransportType.Shuttle]
		}
	];

	let activeTransports: TransportVisualizationGroup[] = $derived(
		transportGroups.filter((group) => group.values.some((type) => (gatheringInfo?.active ?? []).includes(type)))
	);
	let inactiveTransports: TransportVisualizationGroup[] = $derived(
		transportGroups.filter((group) => group.values.some((type) => (gatheringInfo?.inactive ?? []).includes(type)))
	);
</script>

<div
	class="z-999 absolute inset-0 bg-black/40 backdrop-blur-sm"
	onclick={() => (station = null)}
	onkeydown={(e) => {
		if (e.key === "Escape") station = null;
	}}
	role="button"
	tabindex={0}
></div>

<div
	class="z-1000 border-accent bg-background md:rounded-x-2xl absolute bottom-0 left-0 right-0 h-[70vh] rounded-t-2xl md:left-auto md:right-12 md:top-1/2 md:w-96 md:-translate-y-1/2 md:rounded-none md:rounded-l-2xl md:border-y-2 md:border-l-2"
>
	<!-- Header -->
	<div class="border-secondary flex items-center justify-between gap-x-6 border-b p-4">
		<h2 class="text-lg font-bold">
			{station!.name}
		</h2>
		<button
			onclick={() => (station = null)}
			class="hover:bg-accent/20 cursor-pointer rounded-lg p-2 transition-colors"
			aria-label="Close popup"
		>
			<X class="h-5 w-5" />
		</button>
	</div>

	<!-- Content -->
	<div class="flex-1 overflow-y-auto p-4">
		<!-- Querying -->
		<div class="space-y-3">
			<!-- Status -->
			<div
				class={[
					"flex items-center gap-3 rounded-lg border p-4 transition-all",
					{ "border-accent bg-accent/10": gatheringInfo?.queryingEnabled },
					{ "border-red-500/50 bg-red-900/40": !gatheringInfo?.queryingEnabled }
				]}
			>
				<div
					class={[
						"flex h-10 w-10 items-center justify-center rounded-full",
						{ "bg-accent/20": gatheringInfo?.queryingEnabled },
						{ "bg-red-500/20": !gatheringInfo?.queryingEnabled }
					]}
				>
					<Activity
						class="h-5 w-5"
						color={gatheringInfo?.queryingEnabled ? "var(--color-accent)" : "var(--color-red-500)"}
					/>
				</div>
				<div class="flex-1">
					<p class="text-text/60 text-xs uppercase tracking-wider">Status</p>
					<p
						class={[
							"text-sm font-bold",
							{ "text-accent": gatheringInfo?.queryingEnabled },
							{ "text-red-500": !gatheringInfo?.queryingEnabled }
						]}
					>
						{gatheringInfo?.queryingEnabled ? "Actively Monitoring" : "Inactive"}
					</p>
				</div>
			</div>

			<!-- Last Queried -->
			{#if gatheringInfo?.lastQueried}
				<div class="flex items-center gap-3 rounded-lg border border-white/10 bg-white/5 p-5">
					<div class="flex h-10 w-10 items-center justify-center rounded-full bg-white/10">
						<Clock_4 class="text-text/60 h-5 w-5" />
					</div>
					<div class="flex-1">
						<p class="text-text/60 text-xs uppercase tracking-wider">Last Queried</p>
						<p class="text-sm text-white">
							{DateTime.fromISO(gatheringInfo.lastQueried).toFormat("dd. MMMM yyyy, HH:mm")}
						</p>
					</div>
				</div>
			{/if}
		</div>

		<!-- Active Products -->
		{#if activeTransports.length > 0}
			<div class="space-y-2 pt-4">
				<h4 class="text-accent text-sm font-bold">Active Transport Products</h4>
				<div class="flex flex-wrap gap-2">
					{#each activeTransports as transport}
						{@const Component = transport.component}

						<div class="flex items-center gap-2 rounded-lg border border-white/10 bg-white/5 px-3 py-2">
							<Component width="24px" height="24px" />
							<span class="text-sm">{transport.label}</span>
						</div>
					{/each}
				</div>
			</div>
		{/if}

		<!-- Inactive Products -->
		{#if inactiveTransports.length > 0}
			<div class="space-y-2 pt-4">
				<h4 class="text-sm font-bold text-red-500">Inactive Transport Products</h4>
				<div class="flex flex-wrap gap-2">
					{#each inactiveTransports as transport}
						{@const Component = transport.component}

						<div class="flex items-center gap-2 rounded-lg border border-white/10 bg-white/5 px-3 py-2">
							<Component width="24px" height="24px" />
							<span class="text-sm">{transport.label}</span>
						</div>
					{/each}
				</div>
			</div>
		{/if}
	</div>
</div>
