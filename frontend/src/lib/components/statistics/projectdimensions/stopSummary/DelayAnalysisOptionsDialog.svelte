<script lang="ts" module>
	export type ScheduleType = "both" | "departures" | "arrivals";
	export type DelayMode = "sum" | "avg";

	export const DelayAnalysisSeriesType = {
		TOTAL_ARRIVAL: "TOTAL_ARRIVAL",
		TOTAL_DEPARTURE: "TOTAL_DEPARTURE",
		HIGH_SPEED_TRAIN_ARRIVAL: "HIGH_SPEED_TRAIN_ARRIVAL",
		HIGH_SPEED_TRAIN_DEPARTURE: "HIGH_SPEED_TRAIN_DEPARTURE",
		INTERCITY_TRAIN_ARRIVAL: "INTERCITY_TRAIN_ARRIVAL",
		INTERCITY_TRAIN_DEPARTURE: "INTERCITY_TRAIN_DEPARTURE",
		INTER_REGIONAL_TRAIN_ARRIVAL: "INTER_REGIONAL_TRAIN_ARRIVAL",
		INTER_REGIONAL_TRAIN_DEPARTURE: "INTER_REGIONAL_TRAIN_DEPARTURE",
		REGIONAL_TRAIN_ARRIVAL: "REGIONAL_TRAIN_ARRIVAL",
		REGIONAL_TRAIN_DEPARTURE: "REGIONAL_TRAIN_DEPARTURE",
		CITY_TRAIN_ARRIVAL: "CITY_TRAIN_ARRIVAL",
		CITY_TRAIN_DEPARTURE: "CITY_TRAIN_DEPARTURE",
		SUBWAY_ARRIVAL: "SUBWAY_ARRIVAL",
		SUBWAY_DEPARTURE: "SUBWAY_DEPARTURE",
		TRAM_ARRIVAL: "TRAM_ARRIVAL",
		TRAM_DEPARTURE: "TRAM_DEPARTURE",
		BUS_ARRIVAL: "BUS_ARRIVAL",
		BUS_DEPARTURE: "BUS_DEPARTURE",
		UNKNOWN_ARRIVAL: "UNKNOWN_ARRIVAL",
		UNKNOWN_DEPARTURE: "UNKNOWN_DEPARTURE"
	} as const;
	export type DelayAnalysisSeriesType = (typeof DelayAnalysisSeriesType)[keyof typeof DelayAnalysisSeriesType];

	export const delayAnalysisKeyTitles: Record<DelayAnalysisSeriesType, string> = {
		[DelayAnalysisSeriesType.TOTAL_ARRIVAL]: "Total (Arrival)",
		[DelayAnalysisSeriesType.TOTAL_DEPARTURE]: "Total (Departure)",
		[DelayAnalysisSeriesType.HIGH_SPEED_TRAIN_ARRIVAL]: "High Speed (Arrival)",
		[DelayAnalysisSeriesType.HIGH_SPEED_TRAIN_DEPARTURE]: "High Speed (Departure)",
		[DelayAnalysisSeriesType.INTERCITY_TRAIN_ARRIVAL]: "Intercity (Arrival)",
		[DelayAnalysisSeriesType.INTERCITY_TRAIN_DEPARTURE]: "Intercity (Departure)",
		[DelayAnalysisSeriesType.INTER_REGIONAL_TRAIN_ARRIVAL]: "Inter Regional (Arrival)",
		[DelayAnalysisSeriesType.INTER_REGIONAL_TRAIN_DEPARTURE]: "Inter Regional (Departure)",
		[DelayAnalysisSeriesType.REGIONAL_TRAIN_ARRIVAL]: "Regional (Arrival)",
		[DelayAnalysisSeriesType.REGIONAL_TRAIN_DEPARTURE]: "Regional (Departure)",
		[DelayAnalysisSeriesType.CITY_TRAIN_ARRIVAL]: "Suburban (Arrival)",
		[DelayAnalysisSeriesType.CITY_TRAIN_DEPARTURE]: "Suburban (Departure)",
		[DelayAnalysisSeriesType.SUBWAY_ARRIVAL]: "Subway (Arrival)",
		[DelayAnalysisSeriesType.SUBWAY_DEPARTURE]: "Subway (Departure)",
		[DelayAnalysisSeriesType.TRAM_ARRIVAL]: "Tram (Arrival)",
		[DelayAnalysisSeriesType.TRAM_DEPARTURE]: "Tram (Departure)",
		[DelayAnalysisSeriesType.BUS_ARRIVAL]: "Bus (Arrival)",
		[DelayAnalysisSeriesType.BUS_DEPARTURE]: "Bus (Departure)",
		[DelayAnalysisSeriesType.UNKNOWN_ARRIVAL]: "Unknown (Arrival)",
		[DelayAnalysisSeriesType.UNKNOWN_DEPARTURE]: "Unknown (Departure)"
	};

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
</script>

<script lang="ts">
	import { TransportType } from "@lib/api";
	import type { ClassValue } from "svelte/elements";
	import Dialog from "@lib/components/interactable/dialog/Dialog.svelte";
	import DialogItem from "@lib/components/interactable/dialog/DialogItem.svelte";
	import Checkbox from "@lib/components/interactable/Checkbox.svelte";
	import ToggleStateButton from "@lib/components/interactable/ToggleStateButton.svelte";
	import type { Component } from "svelte";
	import LongDistance from "@lib/components/icons/transport-types/LongDistance.svelte";
	import Regional from "@lib/components/icons/transport-types/Regional.svelte";
	import Suburban from "@lib/components/icons/transport-types/Suburban.svelte";
	import Tram from "@lib/components/icons/transport-types/Tram.svelte";
	import Bus from "@lib/components/icons/transport-types/Bus.svelte";

	interface Props {
		isVisible: boolean;
		availableTransportTypes: TransportType[];
		scheduleType: ScheduleType;
		delayMode: DelayMode;
		selectedSeries: DelayAnalysisSeriesType[];
		handleCancelledAsDelayed: boolean;
		delayThreshold: number;
		class?: ClassValue;
	}

	let {
		isVisible = $bindable(false),
		availableTransportTypes,
		scheduleType = $bindable("both"),
		delayMode = $bindable("avg"),
		selectedSeries = $bindable([]),
		handleCancelledAsDelayed = $bindable(false),
		delayThreshold = $bindable(359),
		class: classNames
	}: Props = $props();

	const getGroupPair = (transportType: TransportType | "TOTAL"): [DelayAnalysisSeriesType, DelayAnalysisSeriesType] => {
		if (transportType === "TOTAL") return [DelayAnalysisSeriesType.TOTAL_ARRIVAL, DelayAnalysisSeriesType.TOTAL_DEPARTURE];
		return [`${transportType}_ARRIVAL` as DelayAnalysisSeriesType, `${transportType}_DEPARTURE` as DelayAnalysisSeriesType];
	};

	const isGroupActive = (transportType: TransportType | "TOTAL"): boolean => {
		const [arrival, departure] = getGroupPair(transportType);
		return selectedSeries.includes(arrival) || selectedSeries.includes(departure);
	};

	const toggleGroup = (transportType: TransportType | "TOTAL") => {
		const [arrival, departure] = getGroupPair(transportType);
		if (isGroupActive(transportType))
			return (selectedSeries = selectedSeries.filter(
				(seriesType: DelayAnalysisSeriesType) => seriesType !== arrival && seriesType !== departure
			));
		return (selectedSeries = [...selectedSeries, arrival, departure]);
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
				<div class="flex flex-wrap gap-2">
					{#each [{ value: "both", label: "Both" }, { value: "arrivals", label: "Arrivals" }, { value: "departures", label: "Departures" }] as option (option.value)}
						<ToggleStateButton
							state={scheduleType === option.value}
							ontoggle={() => (scheduleType = option.value as ScheduleType)}
						>
							{option.label}
						</ToggleStateButton>
					{/each}
				</div>
			</DialogItem>

			<!-- Delay Mode -->
			<DialogItem title="Delay Mode" class="col-span-2 sm:col-span-1">
				<div class="flex flex-wrap gap-2">
					{#each [{ value: "sum", label: "Sum" }, { value: "avg", label: "Average" }] as option (option.value)}
						<ToggleStateButton state={delayMode === option.value} ontoggle={() => (delayMode = option.value as DelayMode)}>
							{option.label}
						</ToggleStateButton>
					{/each}
				</div>
			</DialogItem>

			<!-- Series -->
			<DialogItem title="Series" class="col-span-2">
				<div class="flex flex-wrap gap-2">
					{#if availableTransportTypes.length > 1}
						<ToggleStateButton state={isGroupActive("TOTAL")} ontoggle={() => toggleGroup("TOTAL")}>Total</ToggleStateButton>
					{/if}
					{#each availableTransportTypes as transportType (transportType)}
						{@const transportTypeFilter = availableTransportTypeFilters.find(
							(filter: TransportFilter) => filter.transportType === transportType
						)}
						{@const Icon = transportTypeFilter?.icon}
						<ToggleStateButton
							state={isGroupActive(transportType)}
							ontoggle={() => toggleGroup(transportType)}
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
					<input
						type="number"
						bind:value={delayThreshold}
						disabled={!handleCancelledAsDelayed}
						min={0}
						class={[
							"w-24 rounded-lg border px-3 py-1.5 text-sm font-semibold transition-all outline-none",
							handleCancelledAsDelayed
								? "border-emerald-500/20 bg-emerald-500/10 text-emerald-500 focus:border-emerald-500/40"
								: "border-muted-foreground/10 bg-muted/10 text-muted-foreground/40 cursor-not-allowed opacity-75"
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
