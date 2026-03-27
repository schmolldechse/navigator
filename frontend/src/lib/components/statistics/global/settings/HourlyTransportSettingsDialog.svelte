<script module lang="ts">
	import LongDistance from "@lib/components/icons/transport-types/LongDistance.svelte";
	import Tram from "@lib/components/icons/transport-types/Tram.svelte";
	import Regional from "@lib/components/icons/transport-types/Regional.svelte";
	import Suburban from "@lib/components/icons/transport-types/Suburban.svelte";
	import Bus from "@lib/components/icons/transport-types/Bus.svelte";
	import { TransportType } from "@lib/api";

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

	type HourlyTransportSettings = {
		dates: {
			start: DateTime;
			end: DateTime;
		};
		transportTypes: TransportType[];
	};

	export { type HourlyTransportSettings };
</script>

<script lang="ts">
	import { DateTime } from "luxon";
	import type { ClassValue } from "svelte/elements";
	import DialogItem from "@lib/components/interactable/dialog/DialogItem.svelte";
	import TimePicker from "@lib/components/interactable/timepicker/TimePicker.svelte";
	import type { TransportFilterOption } from "@lib/components/interactable/filters/TransportFilter.svelte";
	import Dialog from "@lib/components/interactable/dialog/Dialog.svelte";
	import TransportFilter from "@lib/components/interactable/filters/TransportFilter.svelte";

	type Props = {
		isVisible: boolean;
		initialSettings: HourlyTransportSettings;
		onapply: (settings: HourlyTransportSettings) => void;
		class?: ClassValue;
	};
	let { isVisible = $bindable(false), initialSettings, onapply, class: classNames }: Props = $props();

	// deep clone to prevent modifying the original settings object before applying
	// svelte-ignore state_referenced_locally
	let localSettings: HourlyTransportSettings = $state({
		dates: { start: initialSettings.dates.start, end: initialSettings.dates.end },
		transportTypes: [...initialSettings.transportTypes]
	});

	const handleClose = () => {
		if (!localSettings.dates.start.isValid || !localSettings.dates.end?.isValid) return;

		onapply({
			dates: {
				start: localSettings.dates.start,
				end: localSettings.dates.end
			},
			transportTypes: localSettings.transportTypes
		});
	};
</script>

<Dialog bind:isVisible class={["max-w-[750px]", classNames]} title="Hourly Transport Settings" onclose={handleClose}>
	<div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
		<!-- Timerange Section -->
		<DialogItem title="Timerange" class="col-span-1">
			<TimePicker multiSelect bind:dates={localSettings.dates} />
		</DialogItem>

		<DialogItem title="Transport Types">
			<TransportFilter
				options={availableTransportFilters}
				includeTotalFilter
				onchange={(transportTypes: TransportType[]) => (localSettings.transportTypes = transportTypes)}
			/>
		</DialogItem>
	</div>
</Dialog>
