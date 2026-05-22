<script lang="ts">
	import Dialog from "@lib/components/ui/dialog/Dialog.svelte";
	import {
		ADMINISTRATION_RANKING_METRIC_OPTIONS,
		type AdministrationRankingMetricOption,
		type AdministrationRankingSettings
	} from "./administration-ranking";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import TimePicker, { type TimePickerValue, type TimePickerRange } from "@lib/components/ui/timepicker/TimePicker.svelte";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import Button from "@lib/components/ui/Button.svelte";

	type Props = {
		isVisible: boolean;
		settings: AdministrationRankingSettings;
		onsave: (settings: AdministrationRankingSettings) => void;
	};
	let { isVisible = $bindable(true), settings, onsave }: Props = $props();

	const cloneSettings = (source: AdministrationRankingSettings): AdministrationRankingSettings => ({
		dates: {
			start: source.dates.start,
			end: source.dates.end
		},
		seriesType: source.seriesType,
		limit: source.limit,
		offset: source.offset
	});

	// svelte-ignore state_referenced_locally
	let localSettings: AdministrationRankingSettings = $state(cloneSettings(settings));
	let selectedMetricOptions: AdministrationRankingMetricOption[] = $derived(
		ADMINISTRATION_RANKING_METRIC_OPTIONS.filter(
			(option: AdministrationRankingMetricOption) => option.seriesType === localSettings.seriesType
		)
	);

	const save = () => {
		if (!localSettings.dates.start.isValid || !localSettings.dates.end.isValid) return;

		const nextSettings = cloneSettings(localSettings);
		nextSettings.limit = 10;
		nextSettings.offset = 0;
		isVisible = false;
		onsave(nextSettings);
	};

	const cancel = () => {
		localSettings = cloneSettings(settings);
		isVisible = false;
	};

	$effect(() => {
		if (isVisible) localSettings = cloneSettings(settings);
	});
</script>

<Dialog
	bind:isVisible
	title="Administration Ranking Settings"
	showActions
	onclose={cancel}
	isModal={false}
	class={[
		"z-50 max-md:overflow-hidden",
		"max-md:fixed max-md:inset-x-0 max-md:bottom-0 max-md:max-h-[min(88dvh,42rem)] max-md:w-screen max-md:rounded-b-none max-md:border-x-0 max-md:border-b-0", // mobile view
		"md:absolute md:top-full md:left-auto md:mt-2 md:max-h-[70vh] md:w-[min(34rem,calc(100vw-2rem))]" // desktop view
	]}
>
	<div
		class={[
			"grid grid-cols-1 gap-4",
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

		<section class="space-y-2">
			<div class="flex items-center gap-2">
				<SlidersHorizontal size={16} class="text-accent" />
				<h4 class="text-xs font-semibold tracking-wide uppercase">Metric</h4>
			</div>

			<ToggleGroup
				mode="single"
				selected={selectedMetricOptions}
				keyFn={(option: AdministrationRankingMetricOption) => option.seriesType}
				onselect={(selected: AdministrationRankingMetricOption[]) => {
					const [option] = selected;
					if (!option) return;

					localSettings.seriesType = option.seriesType;
				}}
				class="grid! grid-cols-2 gap-1.5 sm:grid-cols-3"
			>
				{#each ADMINISTRATION_RANKING_METRIC_OPTIONS as option}
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
