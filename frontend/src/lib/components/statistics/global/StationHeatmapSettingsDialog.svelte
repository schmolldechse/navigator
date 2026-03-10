<script module lang="ts">
	import LongDistance from "@lib/components/icons/transport-types/LongDistance.svelte";
	import Tram from "@lib/components/icons/transport-types/Tram.svelte";
	import Regional from "@lib/components/icons/transport-types/Regional.svelte";
	import Suburban from "@lib/components/icons/transport-types/Suburban.svelte";
	import Bus from "@lib/components/icons/transport-types/Bus.svelte";
	import { TransportType } from "@lib/api";
	import type { TransportFilterOption } from "@lib/components/interactable/filters/TransportFilter.svelte";

	const availableTransportFilters: TransportFilterOption[] = [
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
		{ id: "tram", label: "Tram", icon: Tram, transportTypes: [TransportType.TRAM] },
		{ id: "bus", label: "Bus", icon: Bus, transportTypes: [TransportType.BUS] }
	];

	type StationHeatmapScheduleType = "arrivals" | "departures";
	type StationHeatmapPlotType = "count" | "cancellations" | "delay_avg";

	export { type StationHeatmapScheduleType, type StationHeatmapPlotType };
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

	type Props = {
		isVisible: boolean;
		initialDates: {
			start: DateTime;
			end: DateTime;
		};
		initialScheduleType: StationHeatmapScheduleType;
		initialPlotType: StationHeatmapPlotType;
		initialTransportTypes: TransportType[];
		onapply: (params: {
			start: DateTime;
			end: DateTime;
			scheduleType: StationHeatmapScheduleType;
			plotType: StationHeatmapPlotType;
			transportTypes: TransportType[];
		}) => void;
		class?: ClassValue;
	};
	let {
		isVisible = $bindable(false),
		initialDates,
		initialScheduleType,
		initialPlotType,
		initialTransportTypes,
		onapply,
		class: classNames
	}: Props = $props();

	// svelte_ignore state_referenced_locally
	let localTimerange: { start: DateTime; end: DateTime } = $state(initialDates);
	// svelte_ignore state_referenced_locally
	let localTransportFilter: TransportType[] = $state(initialTransportTypes);
	// svelte_ignore state_referenced_locally
	let localScheduleType: StationHeatmapScheduleType = $state(initialScheduleType);
	// svelte_ignore state_referenced_locally
	let localPlotType: StationHeatmapPlotType = $state(initialPlotType);

	const handleClose = () => {
		if (!localTimerange.start.isValid || !localTimerange.end?.isValid) return;

		onapply({
			start: localTimerange.start,
			end: localTimerange.end!,
			scheduleType: localScheduleType,
			plotType: localPlotType,
			transportTypes: localTransportFilter
		});
	};
</script>

<Dialog bind:isVisible class={["max-w-[650px]", classNames]} title="Settings" onclose={handleClose}>
	<div class="grid grid-cols-2 gap-4">
		<!-- Timerange Section -->
		<DialogItem title="Timerange" class="col-span-2 sm:col-span-1">
			<TimePicker
				multiSelect
				dates={localTimerange}
				onchange={({ start, end }) => {
					if (!end) return;
					localTimerange = { start, end };
				}}
			/>
		</DialogItem>

		<div class="flex flex-row space-x-4 sm:flex-col sm:space-y-4 sm:space-x-0">
			<!-- Schedule Type -->
			<DialogItem title="Schedule Type" class="sm:col-span-1">
				{@const scheduleTypeOptions = [
					{ id: "arrivals", value: "Arrivals" },
					{ id: "departures", value: "Departures" }
				]}
				{@const scheduleTypeOption = scheduleTypeOptions.find((option: ToggleStateOption) => option.id === localScheduleType)!}

				<ToggleStateGroup
					options={scheduleTypeOptions}
					selected={scheduleTypeOption}
					onselect={(option: ToggleStateOption) => (localScheduleType = option.id as StationHeatmapScheduleType)}
				/>
			</DialogItem>

			<!-- Snapshot -->
			<DialogItem title="Snapshot" class="col-span-2">
				{@const snapshotOptions = [
					{ id: "count", value: "Count" },
					{ id: "cancellations", value: "Cancellations" },
					{ id: "delay_avg", value: "Delay (avg)" }
				]}
				{@const snapshotOption = snapshotOptions.find((option: ToggleStateOption) => option.id === localPlotType)!}

				<ToggleStateGroup
					options={snapshotOptions}
					selected={snapshotOption}
					onselect={(option: ToggleStateOption) => (localPlotType = option.id as StationHeatmapPlotType)}
				/>
			</DialogItem>
		</div>

		<DialogItem title="Transport Types" class="col-span-2">
			<TransportFilter
				options={availableTransportFilters}
				includeTotalFilter
				onchange={(transportTypes: TransportType[]) => (localTransportFilter = transportTypes)}
			/>
		</DialogItem>
	</div>
</Dialog>
