<script lang="ts">
	import Activity from "@lucide/svelte/icons/activity";
	import X from "@lucide/svelte/icons/x";
	import { getStationGatheringInfo } from "../../remote/geostation.remote";
	import { DateTime } from "luxon";
	import { onMount, type Component } from "svelte";
	import LongDistance from "../icons/transport-types/LongDistance.svelte";
	import Regional from "../icons/transport-types/Regional.svelte";
	import Suburban from "../icons/transport-types/Suburban.svelte";
	import Bus from "../icons/transport-types/Bus.svelte";
	import Subway from "../icons/transport-types/Subway.svelte";
	import Tram from "../icons/transport-types/Tram.svelte";
	import Taxi from "../icons/transport-types/Taxi.svelte";
	import { type BaseStation, type StationGatheringInfo, TransportType } from "@lib/api";

	interface Props {
		isVisible: boolean;
		station: BaseStation;
		onclose: () => void;
		class?: string;
	}

	let { isVisible = $bindable(), station, onclose, class: classes = "" }: Props = $props();

	let dialog: HTMLDialogElement | undefined = $state(undefined);

	$effect(() => {
		if (!dialog) return;
		isVisible ? dialog.showModal() : dialog.close();
	});

	const handleClose = () => {
		isVisible = false;
		onclose?.();
	};

	let stationGatheringInfo: StationGatheringInfo | null = $state(null);

	onMount(async () => {
		if (!station) return;
		stationGatheringInfo = await getStationGatheringInfo({ evaNumber: Number(station.evaNumber) });
	});

	interface TransportVisualizationGroup {
		label: string;
		component?: Component;
		values: TransportType[];
	}

	const transportGroups: TransportVisualizationGroup[] = [
		{ label: "High-Speed", component: LongDistance, values: [TransportType.HIGH_SPEED_TRAIN, TransportType.INTERCITY_TRAIN] },
		{ label: "Regional", component: Regional, values: [TransportType.REGIONAL_TRAIN, TransportType.INTER_REGIONAL_TRAIN] },
		{ label: "Suburban", component: Suburban, values: [TransportType.CITY_TRAIN] },
		{ label: "Bus", component: Bus, values: [TransportType.BUS] },
		{ label: "Subway", component: Subway, values: [TransportType.SUBWAY] },
		{ label: "Tram", component: Tram, values: [TransportType.TRAM] },
		{ label: "Taxi / Shuttle", component: Taxi, values: [TransportType.TAXI, TransportType.SHUTTLE] }
	];

	let activeTransports: TransportVisualizationGroup[] = $derived(
		transportGroups.filter((group) => group.values.some((type) => (stationGatheringInfo?.active ?? []).includes(type)))
	);
	let inactiveTransports: TransportVisualizationGroup[] = $derived(
		transportGroups.filter((group) => group.values.some((type) => (stationGatheringInfo?.disabled ?? []).includes(type)))
	);
</script>

<dialog
	bind:this={dialog}
	onclose={handleClose}
	onclick={(event) => event.target === dialog && handleClose()}
	class={[
		"m-0 h-fit w-full max-w-none overflow-visible bg-transparent p-0 outline-none",
		"fixed inset-x-0 top-auto bottom-0", // mobile: bottom sheet
		"md:top-1/2 md:right-0 md:bottom-auto md:left-auto md:w-115 md:-translate-y-1/2", // desktop: right sidebar, y-centered
		classes
	]}
>
	<div
		class="bg-background border-muted-foreground/20 flex h-full max-h-[90vh] w-full flex-col space-y-6 rounded-t-2xl border-t-2 p-8 shadow-2xl md:rounded-t-none md:rounded-l-3xl md:border-y-2 md:border-t-0 md:border-l-2"
	>
		<!-- Header -->
		<div class="flex items-center justify-between">
			<div class="flex flex-col">
				<p class="text-accent mb-1 text-xs font-bold tracking-widest uppercase">Station Details</p>
				<h2 class="text-text text-2xl leading-tight font-bold">{station.name}</h2>
			</div>

			<button
				onclick={handleClose}
				class="hover:bg-muted-foreground/10 group -mr-2 cursor-pointer rounded-full p-2 transition-colors"
			>
				<X class="stroke-muted-foreground group-hover:stroke-accent h-6 w-6" />
			</button>
		</div>

		<!-- Content -->
		<div class="flex flex-col space-y-6 overflow-y-auto pr-2">
			<!-- Gathering Status -->
			<div class="border-muted-foreground/10 bg-muted/5 flex items-center justify-between gap-x-4 rounded-xl border p-4">
				<div class="flex items-center gap-x-3">
					<div class="bg-accent/10 text-accent rounded-lg p-2">
						<Activity class="h-5 w-5" />
					</div>
					<div>
						<p class="text-muted-foreground text-[10px] font-black tracking-wider uppercase">Gathering Status</p>

						<div class="flex items-center gap-x-2">
							<div
								class={[
									"flex items-center gap-x-2 rounded-full border px-2 py-1 text-xs font-black tracking-tighter uppercase",
									{ "border-emerald-500/20 bg-emerald-500/10 text-emerald-500": stationGatheringInfo?.queryingEnabled },
									{ "border-rose-500/20 bg-rose-500/10 text-rose-500": !stationGatheringInfo?.queryingEnabled }
								]}
							>
								<div
									class={[
										"h-2 w-2 animate-pulse rounded-full",
										{ "bg-emerald-500": stationGatheringInfo?.queryingEnabled },
										{ "bg-rose-500": !stationGatheringInfo?.queryingEnabled }
									]}
								></div>
							</div>
							<p class="text-text text-sm font-bold">
								{stationGatheringInfo?.queryingEnabled ? "Actively Monitoring" : "Inactive"}
							</p>
						</div>
					</div>
				</div>
			</div>

			{#if stationGatheringInfo?.queryingEnabled}
				<div class="flex flex-col gap-y-1 px-1">
					<p class="text-muted-foreground text-[10px] font-black tracking-wider uppercase">Last queried</p>
					{#if stationGatheringInfo?.lastQueried}
						<p class="text-text text-sm font-medium">
							{DateTime.fromISO(stationGatheringInfo?.lastQueried).toLocaleString(DateTime.DATETIME_MED_WITH_SECONDS)}
						</p>
					{:else}
						<p class="text-text text-sm font-medium italic">Never queried</p>
					{/if}
				</div>
			{/if}

			{#if activeTransports.length > 0}
				<div>
					<p class="text-muted-foreground mb-3 px-1 text-[10px] font-black tracking-wider uppercase">Monitoring Transports</p>
					<div class="flex flex-wrap gap-2">
						{#each activeTransports as transport}
							{@const Component = transport.component}

							<div
								class="flex items-center gap-x-2 rounded-lg border border-emerald-500/20 bg-emerald-500/10 px-3 py-2 text-emerald-500"
							>
								<Component class="h-5 w-5" />
								<span class="text-xs font-bold">{transport.label}</span>
							</div>
						{/each}
					</div>
				</div>
			{/if}

			{#if inactiveTransports.length > 0}
				<div>
					<p class="text-muted-foreground mb-3 px-1 text-[10px] font-black tracking-wider uppercase">Inactive Transports</p>
					<div class="flex flex-wrap gap-2">
						{#each inactiveTransports as transport}
							{@const Component = transport.component}

							<div
								class="bg-muted/10 border-muted-foreground/10 text-muted-foreground flex items-center gap-x-2 rounded-lg border px-3 py-2 opacity-60"
							>
								<Component class="h-5 w-5" />
								<span class="text-xs font-bold">{transport.label}</span>
							</div>
						{/each}
					</div>
				</div>
			{/if}
		</div>
	</div>
</dialog>

<style>
	dialog::backdrop {
		background: rgba(0, 0, 0, 0.7);
		backdrop-filter: blur(8px);
	}

	@media (max-width: 767px) {
		dialog[open] {
			animation: slide-up 0.4s cubic-bezier(0.16, 1, 0.3, 1);
		}
	}

	@media (min-width: 768px) {
		dialog[open] {
			animation: slide-left 0.4s cubic-bezier(0.16, 1, 0.3, 1);
		}
	}

	@keyframes slide-up {
		from {
			transform: translateY(100%);
		}
		to {
			transform: translateY(0);
		}
	}

	@keyframes slide-left {
		from {
			transform: translateX(100%) translateY(-50%);
		}
		to {
			transform: translateX(0) translateY(-50%);
		}
	}
</style>
