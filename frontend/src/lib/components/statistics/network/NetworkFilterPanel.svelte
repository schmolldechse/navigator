<script lang="ts">
	import { ScheduleType, type TransportType } from "@lib/api";
	import * as Accordion from "@lib/components/ui/accordion";
	import Button from "@lib/components/ui/Button.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import Input from "@lib/components/ui/Input.svelte";
	import TimePickerDialog from "@lib/components/ui/timepicker/TimePickerDialog.svelte";
	import type { TimePickerRange, TimePickerValue } from "@lib/components/ui/timepicker/TimePicker.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import Check from "@lucide/svelte/icons/check";
	import RotateCcw from "@lucide/svelte/icons/rotate-ccw";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import { DateTime } from "luxon";
	import { getStatisticsDashboardContext } from "../shared/statistics-context.svelte";
	import {
		asLocalDate,
		createRangeLabel,
		networkTransportTypeOptions,
		scheduleTypeOptions,
		type ScheduleTypeOption,
		type StatisticsFilterDraft
	} from "../shared/statistics-dashboard";

	const statistics = getStatisticsDashboardContext();
	const filterPanelValue = "filters";

	let expandedValue = $state<string | undefined>(undefined);
	let datePickerOpen = $state(false);
	let rangeAnchor: HTMLDivElement | undefined = $state(undefined);
	let timePickerStyle = $state("");
	let draft = $state<StatisticsFilterDraft>(statistics.filterDraft);

	const isExpanded = $derived(expandedValue === filterPanelValue);

	const networkTransportTypes = $derived(networkTransportTypeOptions.map((option) => option.value));

	const assignDraft = (next: StatisticsFilterDraft) => {
		draft.from = next.from;
		draft.to = next.to;
		draft.scheduleType = next.scheduleType;
		draft.transportTypes = next.transportTypes.filter((type) => networkTransportTypes.includes(type));
		draft.includeReplacement = next.includeReplacement;
		draft.minVolume = next.minVolume;
	};

	const selectedRange = $derived.by<TimePickerRange | undefined>(() => {
		const start = DateTime.fromISO(draft.from, { setZone: true });
		const exclusiveEnd = DateTime.fromISO(draft.to, { setZone: true });
		if (!start.isValid || !exclusiveEnd.isValid) return undefined;

		return {
			start,
			end: exclusiveEnd.minus({ days: 1 })
		};
	});

	const appliedSummaryItems = $derived.by(() => {
		const parts = [createRangeLabel(statistics.global)];
		if (statistics.global.transportTypes.length > 0) parts.push(`${statistics.global.transportTypes.length} transport filters`);
		if (statistics.event.scheduleType)
			parts.push(statistics.event.scheduleType === ScheduleType.ARRIVAL ? "Arrivals" : "Departures");
		if (!statistics.global.includeReplacement) parts.push("No replacement services");

		return parts;
	});

	const updateTimePickerAnchor = () => {
		if (!rangeAnchor) return;

		const rect = rangeAnchor.getBoundingClientRect();
		timePickerStyle = [
			`--time-picker-left: ${Math.max(16, rect.left)}px`,
			`--time-picker-top: ${rect.bottom + 8}px`,
			`--time-picker-min-width: ${Math.max(320, rect.width)}px`
		].join(";");
	};

	const openDatePicker = () => {
		updateTimePickerAnchor();
		datePickerOpen = true;
	};

	const updateRange = (value: TimePickerValue) => {
		if (!value || !("start" in value)) return;

		draft.from = asLocalDate(value.start);
		draft.to = asLocalDate(value.end.plus({ days: 1 }));
	};

	const toggleTransportType = (transportType: TransportType, checked: boolean) => {
		draft.transportTypes = checked
			? [...new Set([...draft.transportTypes, transportType])]
			: draft.transportTypes.filter((item) => item !== transportType);
	};

	const apply = () => {
		statistics.applyFilters({
			...draft,
			transportTypes: [...draft.transportTypes],
			minVolume: Math.max(0, Number(draft.minVolume) || 0)
		});
		expandedValue = undefined;
	};

	const reset = () => {
		statistics.resetFilters();
		assignDraft(statistics.filterDraft);
		expandedValue = undefined;
	};

	const toggleExpanded = () => {
		if (!isExpanded) assignDraft(statistics.filterDraft);
		expandedValue = isExpanded ? undefined : filterPanelValue;
	};

	$effect(() => {
		if (datePickerOpen) updateTimePickerAnchor();
	});
</script>

<svelte:window onresize={updateTimePickerAnchor} onscroll={updateTimePickerAnchor} />

<section class="border-border bg-secondary/10 rounded-xl border-2">
	<div class="flex flex-col gap-3 p-4 lg:flex-row lg:items-center lg:justify-between">
		<div class="min-w-0">
			<div class="flex items-center gap-2">
				<SlidersHorizontal size={18} class="text-accent shrink-0" />
				<h2 class="text-foreground text-base font-semibold">Filters</h2>
			</div>
			<div class="mt-2 flex flex-wrap gap-2">
				{#each appliedSummaryItems as item (item)}
					<span class="border-border bg-background rounded-lg border px-2.5 py-1 text-xs font-semibold">
						{item}
					</span>
				{/each}
			</div>
		</div>

		<div class="flex flex-wrap gap-2">
			<Button mode="secondary" onclick={reset} class="inline-flex items-center gap-2 text-sm">
				<RotateCcw size={16} />
				Reset
			</Button>
			<Button mode="secondary" onclick={toggleExpanded} class="inline-flex items-center gap-2 text-sm">
				<SlidersHorizontal size={16} />
				Edit
			</Button>
			<Button mode="primary" onclick={apply} class="inline-flex items-center gap-2 text-sm">
				<Check size={16} />
				Apply
			</Button>
		</div>
	</div>

	<Accordion.Root type="single" value={expandedValue} onchange={(value) => (expandedValue = value)}>
		<Accordion.Item value={filterPanelValue} class="border-border/70 border-t">
			<Accordion.Content class="px-4 pt-4 pb-0">
				<div class="grid gap-5">
					<div class="grid gap-4 lg:grid-cols-[minmax(16rem,20rem)_minmax(0,1fr)] lg:items-end">
						<div class="grid gap-1.5 text-sm font-semibold">
							<span class="text-foreground/60">Range</span>
							<div bind:this={rangeAnchor}>
								<Button
									mode="secondary"
									onclick={openDatePicker}
									aria-label="Open date range picker"
									class="inline-flex w-full items-center justify-between gap-3"
								>
									<span>{createRangeLabel(draft)}</span>
									<CalendarDays size={16} class="shrink-0" />
								</Button>
							</div>
							<TimePickerDialog
								bind:isVisible={datePickerOpen}
								isRange
								value={selectedRange}
								onchange={updateRange}
								closeOnSelect
								style={timePickerStyle}
								class={[
									"fixed! inset-x-0! top-auto! bottom-0! m-0! w-[100dvw]! max-w-none! rounded-t-xl! rounded-b-none! border-x-0! border-b-0! p-4! shadow-2xl!",
									"max-h-[75dvh]! overflow-y-auto!",
									"sm:inset-auto! sm:top-[var(--time-picker-top)]! sm:bottom-auto! sm:left-[var(--time-picker-left)]! sm:max-h-[calc(100dvh-var(--time-picker-top)-1rem)]! sm:w-max! sm:min-w-[var(--time-picker-min-width)]! sm:rounded-xl! sm:border-2!"
								]}
							/>
						</div>

						<div class="flex flex-col gap-1">
							<span class="text-foreground/60 text-sm font-semibold">Schedule</span>
							<ToggleGroup
								mode="single"
								allowEmpty={false}
								selected={scheduleTypeOptions.find((option) => option.value === draft.scheduleType) ?? scheduleTypeOptions[0]}
								keyFn={(option: ScheduleTypeOption) => option.value ?? "ALL"}
								onselect={(option: ScheduleTypeOption | undefined) => (draft.scheduleType = option?.value ?? null)}
								class="gap-1"
							>
								{#each scheduleTypeOptions as option (option.value ?? "ALL")}
									<ToggleGroupItem
										item={option}
										class="data-active:border-accent data-active:bg-accent data-active:text-accent-foreground px-2.5 py-1.5 text-xs font-semibold"
									>
										{option.label}
									</ToggleGroupItem>
								{/each}
							</ToggleGroup>
						</div>
					</div>

					<div class="grid gap-4 xl:grid-cols-[minmax(0,1fr)_auto_auto] xl:items-start">
						<div class="grid gap-2 sm:grid-cols-2 xl:grid-cols-5">
							{#each networkTransportTypeOptions as option (option.value)}
								<label
									class={[
										"border-border bg-background flex min-h-20 items-start gap-2 rounded-lg border p-3 text-sm transition-colors",
										draft.transportTypes.includes(option.value) && "border-accent/60 bg-accent/10 text-accent"
									]}
								>
									<Checkbox
										checked={draft.transportTypes.includes(option.value)}
										onchecked={(checked) => toggleTransportType(option.value, checked)}
									/>
									<span class="grid gap-0.5">
										<span class="font-semibold">{option.label}</span>
										<span class="text-foreground/55 text-xs leading-4">{option.description ?? option.shortLabel}</span>
									</span>
								</label>
							{/each}
						</div>

						<label
							class="border-border bg-background inline-flex items-center gap-2 rounded-lg border px-3 py-2 text-sm font-semibold"
						>
							<Checkbox checked={draft.includeReplacement} onchecked={(checked) => (draft.includeReplacement = checked)} />
							<span>Replacement services</span>
						</label>

						<label class="flex items-center gap-2 text-sm font-semibold">
							<span class="text-foreground/60">Ranking min volume</span>
							<Input
								type="number"
								value={draft.minVolume}
								min={0}
								debounceTime={0}
								onchange={(value) => (draft.minVolume = Number(value) || 0)}
								class="bg-background w-24"
							/>
						</label>
					</div>
				</div>
			</Accordion.Content>
		</Accordion.Item>
	</Accordion.Root>
</section>
