<script lang="ts" module>
	type ScheduleType = "both" | "departures" | "arrivals";
	type DelayMode = "sum" | "avg";

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

	export { type ScheduleType, type DelayMode, type TransportFilter, availableTransportTypeFilters };
</script>

<script lang="ts">
	import { TransportType } from "@lib/api";
	import type { ClassValue } from "svelte/elements";
	import Dialog from "@lib/components/interactable/dialog/Dialog.svelte";
	import DialogItem from "@lib/components/interactable/dialog/DialogItem.svelte";
	import Checkbox from "@lib/components/interactable/Checkbox.svelte";
	import type { Component } from "svelte";
	import LongDistance from "@lib/components/icons/transport-types/LongDistance.svelte";
	import Regional from "@lib/components/icons/transport-types/Regional.svelte";
	import Suburban from "@lib/components/icons/transport-types/Suburban.svelte";
	import Tram from "@lib/components/icons/transport-types/Tram.svelte";
	import Bus from "@lib/components/icons/transport-types/Bus.svelte";
	import Input from "@lib/components/interactable/Input.svelte";
	import ToggleStateGroup, { type ToggleStateOption } from "@lib/components/interactable/togglestate/ToggleStateGroup.svelte";
	import ToggleStateButton from "@lib/components/interactable/togglestate/ToggleStateButton.svelte";

	type Props = {
		isVisible: boolean;
		availableTransportTypes: TransportType[];
		selectedTransportTypes: TransportType[];
		showTotalSeries: boolean;
		scheduleType: ScheduleType;
		delayMode: DelayMode;
		handleCancelledAsDelayed: boolean;
		delayThreshold: number;
		class?: ClassValue;
	};
	let {
		isVisible = $bindable(false),
		availableTransportTypes,
		selectedTransportTypes = $bindable([]),
		showTotalSeries = $bindable(true),
		scheduleType = $bindable("both"),
		delayMode = $bindable("avg"),
		handleCancelledAsDelayed = $bindable(false),
		delayThreshold = $bindable(359),
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
		<h3 class="text-muted-foreground text-xl font-semibold">Delay Analysis Options</h3>

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

			<!-- Delay Mode -->
			<DialogItem title="Delay Mode" class="col-span-2 sm:col-span-1">
				{@const delayModeOptions = [
					{ id: "sum", value: "Sum" },
					{ id: "avg", value: "Average" }
				]}
				{@const delayModeOption = delayModeOptions.find((option: ToggleStateOption) => option.id === delayMode)!}

				<ToggleStateGroup
					options={delayModeOptions}
					selected={delayModeOption}
					onselect={(option: ToggleStateOption) => (delayMode = option.id as DelayMode)}
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

			<!-- Cancellation Behaviour -->
			<DialogItem title="Cancellation Behaviour" class="col-span-2 sm:max-w-2xl">
				<p class="text-muted-foreground text-xs leading-relaxed italic">
					By default, completely cancelled stops are not included in the delay statistics because they have no actual arrival or
					departure time. Activating this option will automatically penalize each cancelled stopover with a fixed delay time
					(the threshold) in seconds.
				</p>

				<label class="flex w-fit cursor-pointer items-center gap-3">
					<span class="text-muted-foreground text-sm font-medium">Handle cancelled stops as delayed</span>
					<Checkbox bind:checked={handleCancelledAsDelayed} />
				</label>

				<div class="flex items-center gap-x-2">
					<Input
						type="number"
						value={delayThreshold}
						disabled={!handleCancelledAsDelayed}
						min={0}
						onchange={(value) => typeof value === "number" && (delayThreshold = value)}
						class={[
							"w-24",
							handleCancelledAsDelayed && "border-emerald-500/20 bg-emerald-500/10 text-emerald-500 focus:border-emerald-500/40"
						]}
					/>
					<span class={["text-sm", handleCancelledAsDelayed ? "text-muted-foreground" : "text-muted-foreground/50"]}>
						seconds penalty per cancellation
					</span>
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
