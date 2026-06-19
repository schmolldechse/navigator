<script lang="ts">
	import { ScheduleType, type TransportType } from "@lib/api";
	import Button from "@lib/components/ui/Button.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import Input from "@lib/components/ui/Input.svelte";
	import TimePickerDialog from "@lib/components/ui/timepicker/TimePickerDialog.svelte";
	import type { TimePickerRange, TimePickerValue } from "@lib/components/ui/timepicker/TimePicker.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import Check from "@lucide/svelte/icons/check";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import RotateCcw from "@lucide/svelte/icons/rotate-ccw";
	import { DateTime } from "luxon";
	import { getStationStatisticsContext, type StationFilterDraft } from "./station-context.svelte";
	import {
		asLocalDate,
		createRangeLabel,
		scheduleTypeOptions,
		transportTypeOptions,
		type ScheduleTypeOption
	} from "../shared/statistics-dashboard";

	const statistics = getStationStatisticsContext();

	let expanded = $state(false);
	let datePickerOpen = $state(false);
	let draft = $state<StationFilterDraft>(statistics.filterDraft);

	const assignDraft = (next: StationFilterDraft) => {
		draft.from = next.from;
		draft.to = next.to;
		draft.scheduleType = next.scheduleType;
		draft.transportTypes = [...next.transportTypes];
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

	const appliedSummary = $derived.by(() => {
		const parts = [createRangeLabel(statistics.global)];
		if (statistics.global.transportTypes.length > 0) parts.push(`${statistics.global.transportTypes.length} transport filters`);
		if (statistics.event.scheduleType)
			parts.push(statistics.event.scheduleType === ScheduleType.ARRIVAL ? "Arrivals" : "Departures");
		if (!statistics.global.includeReplacement) parts.push("No replacement");

		return parts.join(" | ");
	});

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
		expanded = false;
	};

	const reset = () => {
		statistics.resetFilters();
		assignDraft(statistics.filterDraft);
		expanded = false;
	};
</script>

<section class="border-border bg-secondary/10 flex flex-col gap-4 rounded-xl border-2 p-4">
	<div class="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
		<div class="min-w-0">
			<div class="flex items-center gap-2">
				<SlidersHorizontal size={18} class="text-accent shrink-0" />
				<h2 class="text-foreground text-base font-semibold">Filters</h2>
			</div>
			<p class="text-foreground/60 mt-1 truncate text-sm font-semibold">{appliedSummary}</p>
		</div>

		<div class="flex flex-wrap gap-2">
			<Button mode="secondary" onclick={reset} class="inline-flex items-center gap-2 text-sm">
				<RotateCcw size={16} />
				Reset
			</Button>
			<Button
				mode={expanded ? "primary" : "secondary"}
				onclick={() => {
					if (!expanded) assignDraft(statistics.filterDraft);
					expanded = !expanded;
				}}
				class="inline-flex items-center gap-2 text-sm"
			>
				<SlidersHorizontal size={16} />
				Edit
			</Button>
			<Button mode="primary" onclick={apply} class="inline-flex items-center gap-2 text-sm">
				<Check size={16} />
				Apply
			</Button>
		</div>
	</div>

	{#if expanded}
		<div class="border-border/70 grid gap-5 border-t pt-4">
			<div class="grid gap-4 lg:grid-cols-[minmax(16rem,20rem)_minmax(0,1fr)] lg:items-end">
				<div class="grid gap-1.5 text-sm font-semibold">
					<span class="text-foreground/60">Range</span>
					<Button
						mode="secondary"
						onclick={() => (datePickerOpen = true)}
						class="inline-flex w-full items-center justify-between gap-3"
					>
						<span>{createRangeLabel(draft)}</span>
						<CalendarDays size={16} class="shrink-0" />
					</Button>
					<TimePickerDialog
						bind:isVisible={datePickerOpen}
						isRange
						value={selectedRange}
						onchange={updateRange}
						closeOnSelect
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

			<div class="grid gap-4 lg:grid-cols-[minmax(0,1fr)_auto_auto] lg:items-center">
				<div class="flex flex-wrap gap-2">
					{#each transportTypeOptions as option (option.value)}
						<label
							class={[
								"border-border bg-background inline-flex items-center gap-2 rounded-lg border px-2.5 py-1.5 text-xs font-semibold",
								draft.transportTypes.includes(option.value) && "border-accent/60 bg-accent/10 text-accent"
							]}
						>
							<Checkbox
								checked={draft.transportTypes.includes(option.value)}
								onchecked={(checked) => toggleTransportType(option.value, checked)}
							/>
							<span>{option.shortLabel}</span>
						</label>
					{/each}
				</div>

				<label
					class="border-border bg-background inline-flex items-center gap-2 rounded-lg border px-3 py-2 text-sm font-semibold"
				>
					<Checkbox checked={draft.includeReplacement} onchecked={(checked) => (draft.includeReplacement = checked)} />
					<span>Replacement</span>
				</label>

				<label class="flex items-center gap-2 text-sm font-semibold">
					<span class="text-foreground/60">Min volume</span>
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
	{/if}
</section>
