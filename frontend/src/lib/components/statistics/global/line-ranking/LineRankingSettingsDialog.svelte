<script lang="ts">
	import Dialog from "@lib/components/ui/dialog/Dialog.svelte";
	import { LINE_RANKING_METRIC_OPTIONS, type LineRankingMetricOption, type LineRankingSettings } from "./line-ranking";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import Filter from "@lucide/svelte/icons/filter";
	import TimePicker, { type TimePickerRange, type TimePickerValue } from "@lib/components/ui/timepicker/TimePicker.svelte";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import Button from "@lib/components/ui/Button.svelte";
	import Input from "@lib/components/ui/Input.svelte";

	type Props = {
		isVisible: boolean;
		settings: LineRankingSettings;
		onsave: (settings: LineRankingSettings) => void;
	};
	let { isVisible = $bindable(true), settings, onsave }: Props = $props();

	const cloneSettings = (source: LineRankingSettings): LineRankingSettings => ({
		dates: {
			start: source.dates.start,
			end: source.dates.end
		},
		seriesType: source.seriesType,
		line: source.line,
		number: source.number,
		limit: source.limit,
		offset: source.offset
	});

	// svelte-ignore state_referenced_locally
	let localSettings: LineRankingSettings = $state(cloneSettings(settings));
	let selectedMetricOptions: LineRankingMetricOption[] = $derived(
		LINE_RANKING_METRIC_OPTIONS.filter((option: LineRankingMetricOption) => option.seriesType === localSettings.seriesType)
	);

	const save = () => {
		if (!localSettings.dates.start.isValid || !localSettings.dates.end.isValid) return;

		const nextSettings = cloneSettings(localSettings);
		nextSettings.limit = 10;
		nextSettings.offset = 0;
		nextSettings.line = nextSettings.line.trim();
		nextSettings.number = nextSettings.number.trim();
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
	title="Line Ranking Settings"
	showActions
	onclose={cancel}
	isModal={false}
	class={[
		"z-50 max-md:overflow-hidden",
		"max-md:fixed max-md:inset-x-0 max-md:bottom-0 max-md:max-h-[min(88dvh,42rem)] max-md:w-screen max-md:rounded-b-none max-md:border-x-0 max-md:border-b-0",
		"md:absolute md:top-full md:left-auto md:mt-2 md:max-h-[70vh] md:w-[min(40rem,calc(100vw-2rem))]"
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
				keyFn={(option: LineRankingMetricOption) => option.seriesType}
				onselect={(selected: LineRankingMetricOption[]) => {
					const [option] = selected;
					if (!option) return;

					localSettings.seriesType = option.seriesType;
				}}
				class="grid! grid-cols-2 gap-1.5 sm:grid-cols-3"
			>
				{#each LINE_RANKING_METRIC_OPTIONS as option}
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

		<section class="space-y-2">
			<div class="flex items-center gap-2">
				<Filter size={16} class="text-accent" />
				<h4 class="text-xs font-semibold tracking-wide uppercase">Filters</h4>
			</div>

			<div class="grid gap-2 sm:grid-cols-2">
				<label class="grid gap-1">
					<span class="text-foreground/60 text-xs font-semibold">Line</span>
					<Input type="text" bind:value={localSettings.line} placeholder="RE|S|ICE" maxlength={64} />
				</label>

				<label class="grid gap-1">
					<span class="text-foreground/60 text-xs font-semibold">Number</span>
					<Input type="text" bind:value={localSettings.number} placeholder="1|8|612" maxlength={64} />
				</label>
			</div>
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
