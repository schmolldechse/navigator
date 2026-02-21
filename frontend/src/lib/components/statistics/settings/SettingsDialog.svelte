<script module lang="ts">
	import LongDistance from "../../icons/transport-types/LongDistance.svelte";
	import Tram from "../../icons/transport-types/Tram.svelte";
	import Regional from "../../icons/transport-types/Regional.svelte";
	import Suburban from "../../icons/transport-types/Suburban.svelte";
	import Bus from "../../icons/transport-types/Bus.svelte";
	import { TransportType } from "../../../api";
	import type { Component } from "svelte";

	export type TransportFilter = {
		id: string;
		label: string;
		icon: Component<{ class?: string }>;
		transportTypes: TransportType[];
	};
	const availableTransportTypeFilters: TransportFilter[] = [
		{
			id: "long_distance",
			label: "Long Distance",
			icon: LongDistance,
			transportTypes: [TransportType.HIGH_SPEED_TRAIN, TransportType.INTERCITY_TRAIN]
		},
		{
			id: "regional",
			label: "Regional",
			icon: Regional,
			transportTypes: [TransportType.INTER_REGIONAL_TRAIN, TransportType.REGIONAL_TRAIN]
		},
		{ id: "suburban", label: "Suburban", icon: Suburban, transportTypes: [TransportType.CITY_TRAIN] },
		{ id: "tram", label: "Tram", icon: Tram, transportTypes: [TransportType.TRAM] },
		{ id: "bus", label: "Bus", icon: Bus, transportTypes: [TransportType.BUS] }
	];
</script>

<script lang="ts">
	import { DateTime } from "luxon";
	import TimePicker from "../../interactable/TimePicker.svelte";
	import type { ClassValue } from "svelte/elements";
	import { onMount } from "svelte";
	import { page } from "$app/state";
	import SettingsItem from "./SettingsItem.svelte";

	type Props = {
		isVisible: boolean;
		dates: {
			start: DateTime;
			end: DateTime;
		};
		onapply: (params: { start: DateTime; end: DateTime; filter: TransportType[] }) => void;
		class?: ClassValue;
	};

	let { isVisible = $bindable(false), dates, onapply, class: classNames }: Props = $props();

	let dialog: HTMLDialogElement | undefined = $state(undefined);

	// svelte-ignore state_referenced_locally
	let localTimerange: { start: DateTime; end: DateTime } = $state(dates);
	let localFilter: TransportType[] = $state([]);

	onMount(
		() =>
			(localFilter = page.url.searchParams
				.getAll("transportTypes")
				.filter((transportType: string): transportType is TransportType =>
					Object.values(TransportType).includes(transportType as TransportType)
				))
	);

	$effect(() => {
		if (!dialog) return;
		isVisible ? dialog.show() : dialog.close();
	});

	const handleClose = () => {
		if (!isVisible) return;
		isVisible = false;

		onapply({ start: localTimerange.start, end: localTimerange.end, filter: localFilter });
	};

	const handleWindowClick = (event: MouseEvent) => {
		if (!dialog || !isVisible) return;
		if (!(event.target instanceof Node) || dialog.contains(event.target as Node)) return;
		handleClose();
	};

	const handleDialogClick = (event: MouseEvent) => {
		if (event.target === dialog) handleClose();
	};

	const toggleFilterItem = (filter: TransportFilter) => {
		const isAllActive = localFilter.length === 0;
		const isFilterActive =
			!isAllActive && filter.transportTypes.every((transportType: TransportType) => localFilter.includes(transportType));

		if (isAllActive) {
			// "Include All" is active → select only this filter
			localFilter = [...filter.transportTypes];
		} else if (isFilterActive) {
			// deselect this filter
			const remaining = localFilter.filter((transportType: TransportType) => !filter.transportTypes.includes(transportType));
			// fall back to "Include All" if no filter is active
			localFilter = remaining.length > 0 ? remaining : [];
		} else localFilter = [...localFilter, ...filter.transportTypes]; // add this filter
	};
</script>

<svelte:window onclick={handleWindowClick} />

<dialog
	bind:this={dialog}
	onclose={handleClose}
	onclick={handleDialogClick}
	class={[
		"bg-background border-muted-foreground/20 absolute right-0 left-auto z-100 w-[650px] max-w-[calc(100vw-2rem)] rounded-lg border-2",
		classNames
	]}
>
	<div class="m-3 flex flex-col space-y-3">
		<h3 class="text-muted-foreground text-xl font-semibold">Settings</h3>

		<div class="divide-muted-foreground/20 flex flex-col divide-y sm:flex-row sm:divide-x sm:divide-y-0">
			<!-- Timerange Section -->
			<SettingsItem title="Timerange" class="shrink-0 pb-4 sm:pr-4 sm:pb-0">
				{#snippet content()}
					<TimePicker
						multiSelect
						dates={localTimerange}
						onchange={({ start, end }) => {
							if (!end) return;

							localTimerange = { start, end };
						}}
						class="sm:w-fit"
					/>
				{/snippet}
			</SettingsItem>

			<!-- Transport Types Section -->
			<SettingsItem title="Transport Types" class="pt-4 sm:pt-0 sm:pl-4">
				{#snippet content()}
					<div class="flex flex-wrap gap-2">
						<button
							class={[
								"flex cursor-pointer items-center gap-x-2 rounded-lg border px-3 py-1.5 text-sm font-semibold transition-all",
								{
									"border-emerald-500/20 bg-emerald-500/10 text-emerald-500 hover:bg-emerald-500/20": localFilter.length === 0
								},
								{
									"border-muted-foreground/10 bg-muted/10 text-muted-foreground hover:bg-muted-foreground/10 opacity-75":
										localFilter.length > 0
								}
							]}
							onclick={() => (localFilter = [])}
						>
							Include All
						</button>

						{#each availableTransportTypeFilters as transportFilter}
							{@const isActive =
								localFilter.length > 0 &&
								transportFilter.transportTypes.every((transportType: TransportType) => localFilter.includes(transportType))}
							{@const Icon = transportFilter.icon}
							<button
								class={[
									"flex cursor-pointer items-center gap-x-2 rounded-lg border px-3 py-1.5 text-sm font-semibold transition-all",
									isActive && "border-emerald-500/20 bg-emerald-500/10 text-emerald-500 hover:bg-emerald-500/20",
									!isActive &&
										"border-muted-foreground/10 bg-muted/10 text-muted-foreground hover:bg-muted-foreground/10 opacity-75"
								]}
								onclick={() => toggleFilterItem(transportFilter)}
							>
								<Icon class="h-4 w-4" />
								{transportFilter.label}
							</button>
						{/each}
					</div>
				{/snippet}
			</SettingsItem>
		</div>

		<!-- Actions -->
		<div class="flex justify-end">
			<button
				class="bg-accent text-background hover:bg-accent/90 cursor-pointer rounded-lg px-4 py-1.5 font-semibold transition-colors"
				onclick={handleClose}
			>
				Done
			</button>
		</div>
	</div>
</dialog>

<style>
	dialog {
		--dialog-enter-opacity: 0;
		--dialog-enter-scale: 0.95;
		--dialog-animation-duration: 0.15s;

		opacity: var(--dialog-enter-opacity, 1);
		transform: translate3d(0, 0, 0)
			scale3d(var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1));

		transition:
			opacity var(--dialog-animation-duration) ease-out,
			transform var(--dialog-animation-duration) ease-out,
			display var(--dialog-animation-duration) ease-out allow-discrete,
			overlay var(--dialog-animation-duration) ease-out allow-discrete;
	}

	dialog[open] {
		opacity: 1;
		transform: translate3d(0, 0, 0) scale3d(1, 1, 1);
	}

	@starting-style {
		dialog[open] {
			opacity: var(--dialog-enter-opacity, 1);
			transform: translate3d(0, 0, 0)
				scale3d(var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1));
		}
	}

	dialog::backdrop {
		background-color: rgba(0, 0, 0, 0);
		transition:
			display var(--dialog-animation-duration, 1) allow-discrete,
			overlay var(--dialog-animation-duration, 1) allow-discrete,
			background-color var(--dialog-animation-duration, 1);
	}

	dialog[open]::backdrop {
		background-color: rgba(0, 0, 0, 0.2);
	}
</style>
