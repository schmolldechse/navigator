<script module lang="ts">
	import LongDistance from "@lib/components/icons/transport-types/LongDistance.svelte";
	import Tram from "@lib/components/icons/transport-types/Tram.svelte";
	import Regional from "@lib/components/icons/transport-types/Regional.svelte";
	import Suburban from "@lib/components/icons/transport-types/Suburban.svelte";
	import Bus from "@lib/components/icons/transport-types/Bus.svelte";
	import Ferry from "@lib/components/icons/transport-types/Ferry.svelte";
	import { TransportType } from "@lib/api";

	const availableTransportFilters: TransportFilterOption[] = [
		{
			id: "local_public_transport",
			label: "Local Transit",
			icon: Regional,
			transportTypes: [
				TransportType.REGIONAL_TRAIN,
				TransportType.CITY_TRAIN,
				TransportType.SUBWAY,
				TransportType.TRAM,
				TransportType.BUS,
				TransportType.FERRY
			]
		},
		{
			id: "long_distance",
			label: "Long Distance",
			icon: LongDistance,
			transportTypes: [TransportType.HIGH_SPEED_TRAIN]
		},
		{
			id: "inter_city_train",
			label: "Intercity",
			icon: LongDistance,
			transportTypes: [TransportType.INTERCITY_TRAIN]
		},
		{
			id: "regional",
			label: "Regional",
			icon: Regional,
			transportTypes: [TransportType.REGIONAL_TRAIN]
		},
		{
			id: "inter_regional",
			label: "Inter Regional",
			icon: Regional,
			transportTypes: [TransportType.INTER_REGIONAL_TRAIN]
		},
		{ id: "suburban", label: "Suburban", icon: Suburban, transportTypes: [TransportType.CITY_TRAIN] },
		{ id: "subway", label: "Subway", icon: Suburban, transportTypes: [TransportType.SUBWAY] },
		{ id: "tram", label: "Tram", icon: Tram, transportTypes: [TransportType.TRAM] },
		{ id: "bus", label: "Bus", icon: Bus, transportTypes: [TransportType.BUS] },
		{ id: "ferry", label: "Ferry", icon: Ferry, transportTypes: [TransportType.FERRY] }
	];

	type StationHeatmapSettings = {
		dates: {
			start: DateTime;
			end: DateTime;
		};
		scheduleType: StationHeatmapScheduleType;
		plotType: StationHeatmapPlotType;
		transportTypes: TransportType[];
	};

	type StationHeatmapScheduleType = "arrivals" | "departures";
	type StationHeatmapPlotType = "count" | "cancellations" | "delay_avg";

	export { type StationHeatmapScheduleType, type StationHeatmapPlotType, type StationHeatmapSettings };
</script>

<script lang="ts">
	import Dialog from "@lib/components/interactable/dialog/Dialog.svelte";
	import DialogItem from "@lib/components/interactable/dialog/DialogItem.svelte";
	import TimePicker from "@lib/components/interactable/timepicker/TimePicker.svelte";
	import TransportFilter from "@lib/components/interactable/filters/TransportFilter.svelte";
	import { DateTime } from "luxon";
	import type { ClassValue } from "svelte/elements";
	import type { ToggleStateOption } from "@lib/components/interactable/togglestate/ToggleStateGroup.svelte";
	import ToggleStateGroup from "@lib/components/interactable/togglestate/ToggleStateGroup.svelte";
	import type { TransportFilterOption } from "@lib/components/interactable/filters/TransportFilter.svelte";

	type Props = {
		isVisible: boolean;
		initialSettings: StationHeatmapSettings;
		onapply: (settings: StationHeatmapSettings) => void;
		class?: ClassValue;
	};
	let { isVisible = $bindable(false), initialSettings, onapply, class: classNames }: Props = $props();

	// deep clone to prevent modifying the original settings object before applying
	// svelte-ignore state_referenced_locally
	let localSettings: StationHeatmapSettings = $state({
		dates: { start: initialSettings.dates.start, end: initialSettings.dates.end },
		scheduleType: initialSettings.scheduleType,
		plotType: initialSettings.plotType,
		transportTypes: [...initialSettings.transportTypes]
	});

	const handleClose = () => {
		if (!localSettings.dates.start.isValid || !localSettings.dates.end?.isValid) return;

		onapply({
			dates: {
				start: localSettings.dates.start,
				end: localSettings.dates.end
			},
			scheduleType: localSettings.scheduleType,
			plotType: localSettings.plotType,
			transportTypes: localSettings.transportTypes
		});
	};
</script>

<Dialog bind:isVisible class={["max-w-[750px]", classNames]} title="Heatmap Settings" onclose={handleClose}>
	<div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
		<!-- Timerange Section -->
		<DialogItem title="Timerange" class="col-span-1">
			<TimePicker multiSelect bind:dates={localSettings.dates} />
		</DialogItem>

		<div class="col-span-1 flex flex-col space-y-4 sm:col-span-2 sm:justify-start">
			<!-- Schedule Type -->
			<DialogItem title="Schedule Type">
				{@const scheduleTypeOptions = [
					{ id: "arrivals", value: "Arrivals" },
					{ id: "departures", value: "Departures" }
				]}
				{@const scheduleTypeOption = scheduleTypeOptions.find(
					(option: ToggleStateOption) => option.id === localSettings.scheduleType
				)!}

				<ToggleStateGroup
					options={scheduleTypeOptions}
					selected={scheduleTypeOption}
					onselect={(option: ToggleStateOption) => (localSettings.scheduleType = option.id as StationHeatmapScheduleType)}
				/>
			</DialogItem>

			<!-- Snapshot -->
			<DialogItem title="Snapshot">
				{@const snapshotOptions = [
					{ id: "count", value: "Count" },
					{ id: "cancellations", value: "Cancellations" },
					{ id: "delay_avg", value: "Delay (avg)" }
				]}
				{@const snapshotOption = snapshotOptions.find((option: ToggleStateOption) => option.id === localSettings.plotType)!}

				<ToggleStateGroup
					options={snapshotOptions}
					selected={snapshotOption}
					onselect={(option: ToggleStateOption) => (localSettings.plotType = option.id as StationHeatmapPlotType)}
				/>
			</DialogItem>

			<DialogItem title="Transport Types">
				<TransportFilter
					options={availableTransportFilters}
					includeTotalFilter
					onchange={(transportTypes: TransportType[]) => (localSettings.transportTypes = transportTypes)}
				/>
			</DialogItem>
		</div>
	</div>
</Dialog>
