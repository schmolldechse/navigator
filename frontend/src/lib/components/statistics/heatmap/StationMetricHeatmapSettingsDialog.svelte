<script module lang="ts">
	import { ScheduleType, TransportType } from "@lib/api";
	import type { Component } from "svelte";
	import LongDistance from "@lib/components/icons/transport-types/LongDistance.svelte";
	import Regional from "@lib/components/icons/transport-types/Regional.svelte";
	import Suburban from "@lib/components/icons/transport-types/Suburban.svelte";

	type TransportOption = {
		id: string;
		label: string;
		icon?: Component<{ class?: string }>;
		transportTypes: TransportType[];
	};

	const TRANSPORT_OPTIONS: TransportOption[] = [
		{
			id: "long-distance",
			label: "Long Distance",
			icon: LongDistance,
			transportTypes: [TransportType.HIGH_SPEED_TRAIN, TransportType.INTERCITY_TRAIN]
		},
		{
			id: "regional",
			label: "Regional",
			icon: Regional,
			transportTypes: [TransportType.REGIONAL_TRAIN, TransportType.INTER_REGIONAL_TRAIN]
		},
		{
			id: "suburban",
			label: "S-Bahn",
			icon: Suburban,
			transportTypes: [TransportType.CITY_TRAIN]
		}
	];

	type ScheduleOption = {
		id: ScheduleType;
		label: string;
	};

	const SCHEDULE_OPTIONS: ScheduleOption[] = [
		{
			id: ScheduleType.ARRIVAL,
			label: "Arrivals"
		},
		{
			id: ScheduleType.DEPARTURE,
			label: "Departures"
		}
	];

	export { SCHEDULE_OPTIONS, TRANSPORT_OPTIONS };
</script>

<script lang="ts">
	import Button from "@lib/components/ui/Button.svelte";
	import Dialog from "@lib/components/ui/dialog/Dialog.svelte";
	import TimePicker, { type TimePickerRange, type TimePickerValue } from "@lib/components/ui/timepicker/TimePicker.svelte";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import Route from "@lucide/svelte/icons/route";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import { HEATMAP_METRIC_OPTIONS, type HeatmapMetricOption, type HeatmapSettings } from "./heatmap";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";

	type Props = {
		isVisible: boolean;
		settings: HeatmapSettings;
		onsave: (settings: HeatmapSettings) => void;
		oncancel?: () => void;
	};
	let { isVisible = $bindable(true), settings, onsave, oncancel }: Props = $props();

	const cloneSettings = (source: HeatmapSettings): HeatmapSettings => ({
		dates: {
			start: source.dates.start,
			end: source.dates.end
		},
		seriesType: source.seriesType,
		scheduleType: source.scheduleType,
		transportTypes: [...source.transportTypes]
	});

	// svelte-ignore state_referenced_locally
	let localSettings: HeatmapSettings = $state(cloneSettings(settings));
	let selectedMetricOptions: HeatmapMetricOption[] = $derived(
		HEATMAP_METRIC_OPTIONS.filter((option: HeatmapMetricOption) => option.seriesType === localSettings.seriesType)
	);
	let selectedScheduleOptions: ScheduleOption[] = $derived(
		SCHEDULE_OPTIONS.filter((option: ScheduleOption) => option.id === localSettings.scheduleType)
	);

	const isTransportActive = (option: TransportOption) =>
		localSettings.transportTypes.length > 0 &&
		option.transportTypes.every((transportType: TransportType) => localSettings.transportTypes.includes(transportType));

	const toggleTransport = (option: TransportOption) => {
		if (isTransportActive(option)) {
			localSettings.transportTypes = localSettings.transportTypes.filter(
				(transportType: TransportType) => !option.transportTypes.includes(transportType)
			);
			return;
		}

		localSettings.transportTypes = [...new Set([...localSettings.transportTypes, ...option.transportTypes])];
	};

	const save = () => {
		if (!localSettings.dates.start.isValid || !localSettings.dates.end.isValid) return;

		const nextSettings = cloneSettings(localSettings);
		isVisible = false;
		onsave(nextSettings);
	};

	const cancel = () => {
		localSettings = cloneSettings(settings);
		oncancel?.();
		isVisible = false;
	};

	$effect(() => {
		if (isVisible) localSettings = cloneSettings(settings);
	});
</script>

<Dialog
	bind:isVisible
	title="Heatmap Settings"
	showActions
	onclose={cancel}
	isModal={false}
	class={[
		"z-50 max-md:overflow-hidden",
		"max-md:fixed max-md:inset-x-0 max-md:bottom-0 max-md:max-h-[min(88dvh,42rem)] max-md:w-screen max-md:rounded-b-none max-md:border-x-0 max-md:border-b-0", // mobile view
		"md:absolute md:top-full md:left-auto md:mt-2 md:max-h-[50vh] md:max-w-[50rem]" // desktop view
	]}
>
	<div
		class={[
			"grid grid-cols-1 gap-3 md:grid-cols-[minmax(14rem,20rem)_1fr]",
			"max-md:max-h-[calc(min(88dvh,42rem)-8.75rem)] max-md:overflow-y-auto max-md:overscroll-contain max-md:pb-2 max-md:[-webkit-overflow-scrolling:touch]"
		]}
	>
		<section class="border-border bg-secondary/30 grid gap-2 rounded-lg border p-2.5">
			<div class="flex items-center gap-x-2 px-1">
				<CalendarDays size={16} class="text-accent" />
				<h4 class="text-xs font-semibold tracking-wide uppercase">Time range</h4>
			</div>

			<TimePicker
				isRange
				value={localSettings.dates}
				onchange={(value: TimePickerValue) => {
					if (!value || !("start" in value)) return;
					localSettings.dates = value as TimePickerRange;
				}}
			/>
		</section>

		<div class="grid content-start gap-4">
			<section class="space-y-2">
				<div class="flex items-center gap-2">
					<SlidersHorizontal size={16} class="text-accent" />
					<h4 class="text-xs font-semibold tracking-wide uppercase">Metric</h4>
				</div>

				<ToggleGroup
					mode="single"
					selected={selectedMetricOptions}
					keyFn={(option: HeatmapMetricOption) => option.seriesType}
					onselect={(selected: HeatmapMetricOption[]) => {
						const [option] = selected;
						if (!option) return;

						localSettings.seriesType = option.seriesType;
					}}
					class="grid! grid-cols-2 gap-1.5 sm:grid-cols-3"
				>
					{#each HEATMAP_METRIC_OPTIONS as option}
						<ToggleGroupItem
							item={option}
							title={option.description}
							aria-label={`${option.label}: ${option.description}`}
							class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground rounded-lg px-2.5 py-2 text-xs font-semibold transition-colors"
						>
							{#snippet children()}
								{option.label}
							{/snippet}
						</ToggleGroupItem>
					{/each}
				</ToggleGroup>
			</section>

			<div class="grid gap-3 sm:grid-cols-[minmax(10rem,0.8fr)_1fr]">
				<section class="space-y-2">
					<div class="flex items-center gap-2">
						<Route size={16} class="text-accent" />
						<h4 class="text-xs font-semibold tracking-wide uppercase">Schedule</h4>
					</div>

					<ToggleGroup
						mode="single"
						selected={selectedScheduleOptions}
						keyFn={(option: ScheduleOption) => option.id}
						onselect={(selected: ScheduleOption[]) => {
							const [option] = selected;
							if (!option) return;

							localSettings.scheduleType = option.id;
						}}
						class="grid! grid-cols-2 gap-1.5 sm:grid-cols-1"
					>
						{#each SCHEDULE_OPTIONS as option}
							<ToggleGroupItem
								item={option}
								class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground rounded-lg px-2.5 py-2 text-xs font-semibold transition-colors"
							>
								{#snippet children()}
									{option.label}
								{/snippet}
							</ToggleGroupItem>
						{/each}
					</ToggleGroup>
				</section>

				<section class="space-y-2">
					<h4 class="text-xs font-semibold tracking-wide uppercase">Transport types</h4>
					<div class="flex flex-wrap gap-1.5">
						<button
							type="button"
							data-active={localSettings.transportTypes.length === 0 ? true : undefined}
							class={[
								"border-border enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:hover:bg-accent data-active:hover:text-accent-foreground rounded-lg border px-2.5 py-1.5 text-xs font-semibold transition-colors",
								localSettings.transportTypes.length === 0 && "border-accent bg-accent text-accent-foreground"
							]}
							onclick={() => (localSettings.transportTypes = [])}
						>
							All
						</button>

						{#each TRANSPORT_OPTIONS as option}
							{@const active = isTransportActive(option)}
							{@const Icon = option.icon}
							<button
								type="button"
								data-active={active ? true : undefined}
								class={[
									"border-border enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:hover:bg-accent data-active:hover:text-accent-foreground flex items-center gap-x-1.5 rounded-lg border px-2.5 py-1.5 text-xs font-semibold transition-colors",
									active && "border-accent bg-accent text-accent-foreground"
								]}
								onclick={() => toggleTransport(option)}
							>
								{#if Icon}
									<Icon class="h-3.5 w-3.5" />
								{/if}
								{option.label}
							</button>
						{/each}
					</div>
				</section>
			</div>
		</div>
	</div>

	{#snippet actions()}
		<div
			class={[
				"mt-2 flex justify-end gap-2",
				"max-md:border-border max-md:bg-background/95 max-md:sticky max-md:bottom-0 max-md:-mx-4 max-md:-mb-4 max-md:border-t max-md:px-4 max-md:py-3 max-md:backdrop-blur-md"
			]}
		>
			<Button mode="tertiary" onclick={cancel}>Cancel</Button>
			<Button mode="primary" onclick={save}>Save</Button>
		</div>
	{/snippet}
</Dialog>
