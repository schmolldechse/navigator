<script lang="ts">
	import { ScheduleType, type TransportType } from "@lib/api";
	import { getTransportTypeIconGroup } from "@lib/components/transport-types/transport-type-icons";
	import * as Accordion from "@lib/components/ui/accordion";
	import Badge from "@lib/components/ui/Badge.svelte";
	import Button from "@lib/components/ui/Button.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import TimePickerDialog from "@lib/components/ui/timepicker/TimePickerDialog.svelte";
	import type { TimePickerRange, TimePickerValue } from "@lib/components/ui/timepicker/TimePicker.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import Check from "@lucide/svelte/icons/check";
	import RotateCcw from "@lucide/svelte/icons/rotate-ccw";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import { DateTime } from "luxon";
	import { getNetworkStatisticsContext, type NetworkFilterDraft } from "./network-context.svelte";
	import {
		asLocalDate,
		createRangeLabel,
		networkTransportTypeOptions,
		scheduleTypeOptions,
		type ScheduleTypeOption,
		type TransportTypeOption
	} from "../shared/statistics-dashboard";

	const statistics = getNetworkStatisticsContext();
	const filterPanelValue = "filters";

	let expandedValue = $state<string | undefined>(undefined);
	let datePickerOpen = $state(false);
	let rangeAnchor: HTMLDivElement | undefined = $state(undefined);
	let timePickerStyle = $state("");
	const createDraft = (): NetworkFilterDraft => ({
		from: statistics.filterDraft.from,
		to: statistics.filterDraft.to,
		scheduleType: statistics.filterDraft.scheduleType,
		transportTypes: [...statistics.filterDraft.transportTypes],
		includeReplacement: statistics.filterDraft.includeReplacement
	});

	let draft = $state<NetworkFilterDraft>(createDraft());

	const isExpanded = $derived(expandedValue === filterPanelValue);

	const networkTransportTypes = $derived(networkTransportTypeOptions.map((option) => option.value));
	const selectedTransportTypeOptions = $derived(
		networkTransportTypeOptions.filter((option) => draft.transportTypes.includes(option.value))
	);
	const transportTypeKey = (transportTypes: TransportType[]): string => [...transportTypes].sort().join("|");
	const hasFilterChanges = $derived.by(() => {
		const applied = statistics.filterDraft;

		return (
			draft.from !== applied.from ||
			draft.to !== applied.to ||
			draft.scheduleType !== applied.scheduleType ||
			draft.includeReplacement !== applied.includeReplacement ||
			transportTypeKey(draft.transportTypes) !== transportTypeKey(applied.transportTypes)
		);
	});

	const assignDraft = (next: NetworkFilterDraft) => {
		draft.from = next.from;
		draft.to = next.to;
		draft.scheduleType = next.scheduleType;
		draft.transportTypes = next.transportTypes.filter((type) => networkTransportTypes.includes(type));
		draft.includeReplacement = next.includeReplacement;
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
		const minWidth = Math.max(320, rect.width);
		const viewportPadding = 16;
		const left = Math.min(
			Math.max(viewportPadding, rect.left),
			Math.max(viewportPadding, window.innerWidth - minWidth - viewportPadding)
		);

		timePickerStyle = [
			`--time-picker-left: ${left}px`,
			`--time-picker-top: ${rect.bottom + 8}px`,
			`--time-picker-min-width: ${minWidth}px`,
			`--time-picker-max-width: calc(100dvw - ${viewportPadding * 2}px)`
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

	const updateTransportTypes = (selectedOptions: TransportTypeOption[]) => {
		draft.transportTypes = selectedOptions.map((option) => option.value).filter((type) => networkTransportTypes.includes(type));
	};

	const apply = () => {
		statistics.applyFilters({
			...draft,
			transportTypes: [...draft.transportTypes]
		});
		expandedValue = undefined;
	};

	const reset = () => {
		statistics.resetFilters();
		assignDraft(createDraft());
		expandedValue = undefined;
	};

	const toggleExpanded = () => {
		if (!isExpanded) assignDraft(createDraft());
		expandedValue = isExpanded ? undefined : filterPanelValue;
	};

	$effect(() => {
		if (datePickerOpen) updateTimePickerAnchor();
	});
</script>

<svelte:window onresize={updateTimePickerAnchor} onscroll={updateTimePickerAnchor} />

<section class="border-border bg-secondary/10 rounded-xl border-2">
	<div class="grid gap-4 p-4 sm:p-5 lg:grid-cols-[minmax(0,1fr)_auto] lg:items-start">
		<div class="min-w-0">
			<div class="flex items-center gap-2">
				<SlidersHorizontal size={18} class="text-accent shrink-0" />
				<h2 class="text-foreground text-base font-semibold">Filters</h2>
			</div>

			<div class="mt-2 flex flex-wrap gap-2">
				{#each appliedSummaryItems as item (item)}
					<Badge>{item}</Badge>
				{/each}
			</div>
		</div>

		<div class="grid grid-cols-3 gap-2 sm:flex sm:flex-wrap sm:justify-end">
			<Button
				mode="secondary"
				onclick={reset}
				class="inline-flex h-9 w-full items-center justify-center gap-2 text-sm sm:w-auto"
			>
				<RotateCcw size={16} />
				Reset
			</Button>

			<Button
				mode="secondary"
				onclick={toggleExpanded}
				aria-expanded={isExpanded}
				class="inline-flex h-9 w-full items-center justify-center gap-2 text-sm sm:w-auto"
			>
				<SlidersHorizontal size={16} />
				Edit
			</Button>

			<Button
				mode="primary"
				onclick={apply}
				disabled={!hasFilterChanges}
				class="inline-flex h-9 w-full items-center justify-center gap-2 text-sm sm:w-auto"
			>
				<Check size={16} />
				Apply
			</Button>
		</div>
	</div>

	<Accordion.Root
		type="single"
		value={expandedValue}
		onchange={(value) => (expandedValue = value)}
		class="[&>[role=separator]]:hidden"
	>
		<Accordion.Item value={filterPanelValue}>
			<Accordion.Content class="border-border/70 border-t px-4 pt-4 pb-4 sm:px-5 sm:pb-5">
				<div class="grid gap-5 lg:grid-cols-12 lg:gap-x-6 xl:gap-x-8">
					<div class="flex min-w-0 flex-col items-start gap-1.5 lg:col-span-3">
						<span class="text-foreground/60 text-[0.65rem] font-bold tracking-wider uppercase">Range</span>

						<div bind:this={rangeAnchor} class="w-full">
							<Button
								mode="secondary"
								onclick={openDatePicker}
								aria-label="Open date range picker"
								class="inline-flex h-9 w-full items-center justify-between gap-3 px-3"
							>
								<span class="truncate">{createRangeLabel(draft)}</span>
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
								"sm:inset-auto! sm:top-[var(--time-picker-top)]! sm:bottom-auto! sm:left-[var(--time-picker-left)]! sm:max-h-[calc(100dvh-var(--time-picker-top)-1rem)]! sm:w-max! sm:max-w-[var(--time-picker-max-width)]! sm:min-w-[var(--time-picker-min-width)]! sm:rounded-xl! sm:border-2!"
							]}
						/>
					</div>

					<div class="flex min-w-0 flex-col items-start gap-1.5 lg:col-span-4">
						<span class="text-foreground/60 text-[0.65rem] font-bold tracking-wider uppercase">Schedule</span>

						<ToggleGroup
							mode="single"
							allowEmpty={false}
							selected={scheduleTypeOptions.find((option) => option.value === draft.scheduleType) ?? scheduleTypeOptions[0]}
							keyFn={(option: ScheduleTypeOption) => option.value ?? "ALL"}
							onselect={(option: ScheduleTypeOption | undefined) => (draft.scheduleType = option?.value ?? null)}
							class="flex flex-wrap gap-2"
						>
							{#each scheduleTypeOptions as option (option.value ?? "ALL")}
								<ToggleGroupItem
									item={option}
									class="data-active:border-accent data-active:bg-accent data-active:text-accent-foreground min-h-9 px-3 py-1.5 text-xs font-semibold"
								>
									{option.label}
								</ToggleGroupItem>
							{/each}
						</ToggleGroup>
					</div>

					<div class="flex min-w-0 flex-col items-start gap-1.5 lg:col-span-5">
						<span class="text-foreground/60 text-[0.65rem] font-bold tracking-wider uppercase">Replacement</span>

						<label
							class="border-border bg-background hover:border-accent/60 hover:bg-secondary/40 inline-flex w-full items-start gap-2 rounded-md border px-3 py-2 text-sm transition-colors"
						>
							<Checkbox
								checked={draft.includeReplacement}
								onchecked={(checked) => (draft.includeReplacement = checked)}
								class="mt-0.5 shrink-0"
							/>
							<span class="grid min-w-0 gap-0.5">
								<span class="text-xs font-semibold">Include replacement</span>
								<span class="text-foreground/60 text-xs leading-4">
									Include services marked as replacement transport in all network metrics. Enabled by default.
								</span>
							</span>
						</label>
					</div>

					<div class="flex min-w-0 flex-col items-start gap-1.5 lg:col-span-12">
						<span class="text-foreground/60 text-[0.65rem] font-bold tracking-wider uppercase">Transport types</span>

						<ToggleGroup
							mode="multiple"
							selected={selectedTransportTypeOptions}
							keyFn={(option: TransportTypeOption) => option.value}
							onselect={updateTransportTypes}
							class="grid w-full grid-cols-2 gap-2 sm:flex sm:flex-wrap"
						>
							{#each networkTransportTypeOptions as option (option.value)}
								{@const iconGroup = getTransportTypeIconGroup(option.value)}
								<ToggleGroupItem
									item={option}
									aria-label={`Toggle ${option.label} transport filter`}
									class="data-active:border-accent data-active:bg-accent data-active:text-accent-foreground min-h-9 justify-center gap-1.5 px-3 py-1.5 text-xs font-semibold sm:justify-start"
								>
									{#if iconGroup}
										{@const Icon = iconGroup.Icon}
										<Icon type="rounded-corners" class="size-4 shrink-0" />
									{/if}
									<span class="truncate">{option.label}</span>
								</ToggleGroupItem>
							{/each}
						</ToggleGroup>
					</div>
				</div>
			</Accordion.Content>
		</Accordion.Item>
	</Accordion.Root>
</section>
