<script lang="ts">
	import { DateTime } from "luxon";
	import Button from "@lib/components/ui/Button.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import TimePicker, { type TimePickerRange, type TimePickerValue } from "@lib/components/ui/timepicker/TimePicker.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import Check from "@lucide/svelte/icons/check";
	import ChevronDown from "@lucide/svelte/icons/chevron-down";
	import LoaderCircle from "@lucide/svelte/icons/loader-circle";
	import RotateCcw from "@lucide/svelte/icons/rotate-ccw";
	import Route from "@lucide/svelte/icons/route";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import TrainFront from "@lucide/svelte/icons/train-front";
	import { TransportType } from "@lib/api";
	import {
		SCHEDULE_OPTIONS,
		TRANSPORT_OPTIONS,
		areScopeSettingsEqual,
		cloneScopeSettings,
		type ScheduleOption,
		type StatisticsScopeSettings,
		type TransportOption
	} from "./statistics-scope";
	import { getStatisticsScopeContext } from "./statistics-scope-context.svelte";

	type Props = {
		isUpdating?: boolean;
	};
	let { isUpdating = false }: Props = $props();

	const scopeContext = getStatisticsScopeContext();

	let expanded: boolean = $state(false);

	let draft: StatisticsScopeSettings = $state(cloneScopeSettings(scopeContext.current));
	let hasChanges = $derived(!areScopeSettingsEqual(draft, scopeContext.current));

	let dateLabel = $derived(
		`${draft.dates.start.toLocaleString(DateTime.DATE_MED)} - ${draft.dates.end.toLocaleString(DateTime.DATE_MED)}`
	);
	let transportLabel = $derived.by(() => {
		if (draft.transportTypes.length === 0) return "All transport";

		const selectedLabels = TRANSPORT_OPTIONS.filter((option: TransportOption) =>
			option.transportTypes.every((transportType: TransportType) => draft.transportTypes.includes(transportType))
		).map((option) => option.label);

		return selectedLabels.length > 0 ? selectedLabels.join(", ") : `${draft.transportTypes.length} transport types`;
	});

	/**
	 * ScheduleOption
	 */
	let selectedScheduleOption: ScheduleOption | undefined = $derived(
		SCHEDULE_OPTIONS.find((option: ScheduleOption) => option.id === draft.scheduleType)
	);

	/**
	 * TransportOption
	 */
	const isTransportActive = (option: TransportOption) =>
		draft.transportTypes.length > 0 &&
		option.transportTypes.every((transportType: TransportType) => draft.transportTypes.includes(transportType));

	let selectedTransportOptions: TransportOption[] = $derived(
		TRANSPORT_OPTIONS.filter((option: TransportOption) => isTransportActive(option))
	);

	const selectTransportOptions = (options: TransportOption[]) => {
		draft.transportTypes = [...new Set(options.flatMap((option: TransportOption) => option.transportTypes))];
	};

	const apply = () => {
		if (!draft.dates.start.isValid || !draft.dates.end.isValid || !hasChanges) return;

		scopeContext.update(draft);
		draft = cloneScopeSettings(scopeContext.current);
	};

	const reset = () => {
		draft = cloneScopeSettings(scopeContext.current);
	};
</script>

<section class="border-border bg-secondary/20 rounded-lg border p-3 sm:p-4">
	<div class="grid gap-3 lg:grid-cols-[minmax(0,1fr)_auto] lg:items-start">
		<div class="flex flex-col gap-y-2">
			<div class="flex flex-wrap items-center gap-x-2">
				<SlidersHorizontal size={18} class="text-accent" />
				<h2 class="text-base font-semibold">Scope</h2>

				{#if isUpdating}
					<span class="text-foreground/60 inline-flex items-center gap-1.5 text-xs font-semibold">
						<LoaderCircle size={13} class="animate-spin" />
						Updating
					</span>
				{/if}
			</div>

			<div class="text-foreground/65 grid gap-1.5 text-xs font-semibold sm:flex sm:flex-wrap">
				<span class="bg-background border-border inline-flex items-center gap-x-1 rounded-md border px-2 py-1">
					<CalendarDays size={13} class="shrink-0" />
					<span class="min-w-0 truncate">{dateLabel}</span>
				</span>
				<span class="bg-background border-border inline-flex items-center gap-x-1 rounded-md border px-2 py-1">
					<Route size={13} class="shrink-0" />
					<span class="min-w-0 truncate">{selectedScheduleOption?.label || "Schedule"}</span>
				</span>
				<span class="bg-background border-border inline-flex items-center gap-x-1 rounded-md border px-2 py-1">
					<TrainFront size={13} class="shrink-0" />
					<span class="min-w-0 truncate">{transportLabel}</span>
				</span>
			</div>
		</div>

		<div class="flex flex-row gap-x-2">
			<Button
				mode="secondary"
				onclick={() => (expanded = !expanded)}
				aria-expanded={expanded}
				class="inline-flex items-center justify-center gap-2 px-3 py-1.5 text-sm"
			>
				<ChevronDown
					size={16}
					class={["transition-transform", expanded && "rotate-180 duration-300", !expanded && "rotate-0 duration-300"]}
				/>
				Edit
			</Button>

			<Button
				mode="tertiary"
				disabled={!hasChanges}
				onclick={reset}
				aria-label="Reset scope changes"
				class="inline-flex items-center justify-center gap-2 px-3 py-1.5 text-sm"
			>
				<RotateCcw size={16} />
				Reset
			</Button>

			<Button
				mode="primary"
				disabled={!hasChanges || !draft.dates.start.isValid || !draft.dates.end.isValid}
				onclick={apply}
				aria-label="Apply statistics scope"
				class="inline-flex items-center justify-center gap-2 px-3 py-1.5 text-sm"
			>
				<Check size={16} />
				Apply
			</Button>
		</div>
	</div>

	{#if expanded}
		<div class="border-border mt-4 grid gap-3 border-t pt-4 lg:grid-cols-[minmax(16rem,22rem)_minmax(0,1fr)]">
			<!-- Time Picker -->
			<section class="grid gap-2">
				<div class="flex items-center gap-2 px-1">
					<CalendarDays size={16} class="text-accent" />
					<h3 class="text-xs font-semibold tracking-wide uppercase">Time range</h3>
				</div>

				<TimePicker
					isRange
					value={draft.dates}
					onchange={(value: TimePickerValue) => {
						if (!value || !("start" in value)) return;
						draft.dates = value as TimePickerRange;
					}}
				/>
			</section>

			<div class="grid auto-rows-max gap-3 md:grid-cols-2">
				<section class="grid gap-2">
					<div class="flex items-center gap-2 px-1">
						<Route size={16} class="text-accent" />
						<h3 class="text-xs font-semibold tracking-wide uppercase">Schedule</h3>
					</div>

					<ToggleGroup
						mode="single"
						allowEmpty={false}
						selected={selectedScheduleOption}
						keyFn={(option: ScheduleOption) => option.id}
						onselect={(option: ScheduleOption | undefined) => {
							if (!option) return;
							draft.scheduleType = option.id;
						}}
					>
						{#each SCHEDULE_OPTIONS as option (option.id)}
							<ToggleGroupItem
								item={option}
								class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground rounded-lg px-2.5 py-2 text-xs font-semibold transition-colors"
							>
								{option.label}
							</ToggleGroupItem>
						{/each}
					</ToggleGroup>
				</section>

				<section class="grid gap-2">
					<div class="flex items-center gap-2 px-1">
						<TrainFront size={16} class="text-accent" />
						<h3 class="text-xs font-semibold tracking-wide uppercase">Transport types</h3>
					</div>

					<div class="flex flex-wrap gap-2">
						<button
							type="button"
							data-active={draft.transportTypes.length === 0 ? true : undefined}
							class={[
								"border-border enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:hover:bg-accent data-active:hover:text-accent-foreground rounded-lg border px-2.5 py-1.5 text-xs font-semibold transition-colors",
								draft.transportTypes.length === 0 && "border-accent bg-accent text-accent-foreground"
							]}
							onclick={() => (draft.transportTypes = [])}
						>
							All
						</button>

						<ToggleGroup
							mode="multiple"
							selected={selectedTransportOptions}
							keyFn={(option: TransportOption) => option.id}
							onselect={selectTransportOptions}
						>
							{#each TRANSPORT_OPTIONS as option (option.id)}
								{@const Icon = option.icon}
								<ToggleGroupItem
									item={option}
									class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground flex items-center gap-x-1.5 rounded-lg px-2.5 py-1.5 text-xs font-semibold transition-colors"
								>
									{#if Icon}
										<Icon class="h-3.5 w-3.5" />
									{/if}
									{option.label}
								</ToggleGroupItem>
							{/each}
						</ToggleGroup>
					</div>
				</section>

				<section class="md:col-span-2">
					<label
						class="bg-background/60 border-border flex items-start gap-3 rounded-lg border p-3"
						for="include-replacement-transport"
					>
						<Checkbox
							bind:checked={draft.includeReplacementTransport}
							id="include-replacement-transport"
							aria-label="Include replacement transport"
						/>
						<span class="grid gap-0.5 text-sm leading-relaxed">
							<span class="font-medium">Include replacement services</span>
							<span class="text-foreground/60">
								When disabled, journeys and stop events marked as replacement transport are excluded.
							</span>
						</span>
					</label>
				</section>
			</div>
		</div>
	{/if}
</section>
