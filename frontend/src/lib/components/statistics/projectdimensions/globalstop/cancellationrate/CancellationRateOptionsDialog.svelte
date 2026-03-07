<script module lang="ts">
	type ScheduleType = "both" | "departures" | "arrivals";
	type RateMode = "absolute" | "relative";

	type TransportFilter = {
		id: string;
		label: string;
		icon?: Component<{ class?: string }>;
		transportType: TransportType;
	};
	const availableTransportTypeFilters: TransportFilter[] = [
		{
			id: "long_distance",
			label: "Long Distance",
			icon: LongDistance,
			transportType: TransportType.HIGH_SPEED_TRAIN
		},
		{
			id: "inter_city_train",
			label: "Intercity",
			icon: LongDistance,
			transportType: TransportType.INTERCITY_TRAIN
		},
		{
			id: "regional",
			label: "Regional",
			icon: Regional,
			transportType: TransportType.REGIONAL_TRAIN
		},
		{
			id: "inter_regional",
			label: "Inter Regional",
			icon: Regional,
			transportType: TransportType.INTER_REGIONAL_TRAIN
		},
		{ id: "suburban", label: "Suburban", icon: Suburban, transportType: TransportType.CITY_TRAIN },
		{ id: "tram", label: "Tram", icon: Tram, transportType: TransportType.TRAM },
		{ id: "bus", label: "Bus", icon: Bus, transportType: TransportType.BUS },
		{ id: "unknown", label: "Unknown", transportType: TransportType.UNKNOWN }
	];

	export { type ScheduleType, type RateMode, type TransportFilter, availableTransportTypeFilters };
</script>

<script lang="ts">
	import { TransportType } from "@lib/api";
	import Bus from "@lib/components/icons/transport-types/Bus.svelte";
	import LongDistance from "@lib/components/icons/transport-types/LongDistance.svelte";
	import Regional from "@lib/components/icons/transport-types/Regional.svelte";
	import Suburban from "@lib/components/icons/transport-types/Suburban.svelte";
	import Tram from "@lib/components/icons/transport-types/Tram.svelte";
	import Dialog from "@lib/components/interactable/dialog/Dialog.svelte";
	import DialogItem from "@lib/components/interactable/dialog/DialogItem.svelte";
	import ToggleStateButton from "@lib/components/interactable/togglestate/ToggleStateButton.svelte";
	import ToggleStateGroup, { type ToggleStateOption } from "@lib/components/interactable/togglestate/ToggleStateGroup.svelte";
	import type { Component } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		isVisible: boolean;
		availableTransportTypes: TransportType[];
		selectedTransportTypes: TransportType[];
		showTotalSeries: boolean;
		scheduleType: ScheduleType;
		rateMode: RateMode;
		class?: ClassValue;
	};
	let {
		isVisible = $bindable(false),
		availableTransportTypes,
		selectedTransportTypes = $bindable([]),
		showTotalSeries = $bindable(true),
		scheduleType = $bindable("both" as ScheduleType),
		rateMode = $bindable("relative" as RateMode),
		class: classNames
	}: Props = $props();

	const toggleFilterItem = (selectedTransportType: TransportType) => {
		if (selectedTransportTypes.includes(selectedTransportType))
			selectedTransportTypes = selectedTransportTypes.filter(
				(transportType: TransportType) => transportType !== selectedTransportType
			);
		else selectedTransportTypes = [...selectedTransportTypes, selectedTransportType];
	};
</script>

<Dialog
	bind:isVisible
	isModal
	class={[
		"mx-0 mt-auto mb-0 max-h-[75vh] w-full max-w-full rounded-t-2xl rounded-b-none border-b-0", // mobile: bottom sheet
		"sm:m-auto sm:max-h-fit sm:max-w-2xl sm:rounded-lg sm:border-b-2", // desktop: centered dialog
		classNames
	]}
>
	<div class="m-3 flex flex-col space-y-3">
		<h3 class="text-muted-foreground text-xl font-semibold">Cancellation Rate Options</h3>

		<div class="grid grid-cols-2 gap-4">
			<!-- Schedule Type -->
			<DialogItem title="Scheduling" class="col-span-2 sm:col-span-1">
				{@const scheduleTypeOptions = [
					{ id: "both", value: "Both" },
					{ id: "arrivals", value: "Arrivals" },
					{ id: "departures", value: "Departures" }
				]}
				{@const scheduleTypeOption = scheduleTypeOptions.find((option: ToggleStateOption) => option.id === scheduleType)!}

				<ToggleStateGroup
					options={scheduleTypeOptions}
					selected={scheduleTypeOption}
					onselect={(option: ToggleStateOption) => (scheduleType = option.id as ScheduleType)}
				/>
			</DialogItem>

			<!-- Rate Mode -->
			<DialogItem title="Rate Mode" class="col-span-2 sm:col-span-1">
				{@const rateModeOptions = [
					{ id: "relative", value: "Relative" },
					{ id: "absolute", value: "Absolute" }
				]}
				{@const rateModeOption = rateModeOptions.find((option: ToggleStateOption) => option.id === rateMode)!}

				<ToggleStateGroup
					options={rateModeOptions}
					selected={rateModeOption}
					onselect={(option: ToggleStateOption) => (rateMode = option.id as RateMode)}
				/>
			</DialogItem>

			<!-- Series -->
			<DialogItem title="Series" class="col-span-2">
				<div class="flex flex-wrap gap-2">
					{#if availableTransportTypes.length > 1}
						<ToggleStateButton bind:state={showTotalSeries}>Total</ToggleStateButton>
					{/if}

					{#each availableTransportTypes as transportType (transportType)}
						{@const transportTypeFilter = availableTransportTypeFilters.find(
							(filter: TransportFilter) => filter.transportType === transportType
						)}
						{@const Icon = transportTypeFilter?.icon}
						<ToggleStateButton
							state={selectedTransportTypes.includes(transportType)}
							ontoggle={() => toggleFilterItem(transportType)}
							class="flex items-center gap-x-2"
						>
							{#if Icon}
								<Icon class="h-4 w-4" />
							{/if}
							{transportTypeFilter?.label ?? transportType}
						</ToggleStateButton>
					{/each}
				</div>
			</DialogItem>
		</div>

		<!-- Actions -->
		<div class="flex justify-end">
			<button
				class="bg-accent text-background hover:bg-accent/90 cursor-pointer rounded-lg px-4 py-1.5 font-semibold transition-colors"
				onclick={() => (isVisible = false)}
			>
				Done
			</button>
		</div>
	</div>
</Dialog>
