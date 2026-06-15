<script lang="ts">
	import { goto } from "$app/navigation";
	import { page } from "$app/state";
	import { ScheduleType, StatisticsBucket, type TransportType } from "@lib/api";
	import Button from "@lib/components/ui/Button.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import Input from "@lib/components/ui/Input.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import Filter from "@lucide/svelte/icons/filter";
	import RotateCcw from "@lucide/svelte/icons/rotate-ccw";
	import {
		createSearchParamsFromStatisticsFilters,
		defaultFilters,
		scheduleTypeOptions,
		statisticsBucketOptions,
		transportTypeOptions,
		type ScheduleTypeOption,
		type StatisticsBucketOption,
		type StatisticsFilterState
	} from "./statistics-dashboard";

	type Props = {
		filters: StatisticsFilterState;
		showBucket?: boolean;
	};

	let { filters, showBucket = true }: Props = $props();

	const navigateWithFilters = async (next: StatisticsFilterState) => {
		const searchParams = createSearchParamsFromStatisticsFilters(next);
		await goto(`${page.url.pathname}?${searchParams.toString()}`, {
			keepFocus: true,
			noScroll: true
		});
	};

	const apply = async (partial: Partial<StatisticsFilterState>) => {
		const next: StatisticsFilterState = {
			...filters,
			...partial,
			transportTypes: partial.transportTypes ? [...partial.transportTypes] : [...filters.transportTypes]
		};
		await navigateWithFilters(next);
	};

	const toggleTransportType = async (transportType: TransportType, checked: boolean) => {
		const nextTransportTypes = checked
			? [...new Set([...filters.transportTypes, transportType])]
			: filters.transportTypes.filter((item) => item !== transportType);

		await apply({ transportTypes: nextTransportTypes });
	};

	const reset = async () => {
		await navigateWithFilters(defaultFilters());
	};
</script>

<section class="border-border bg-secondary/10 flex flex-col gap-4 rounded-xl border-2 p-4">
	<div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
		<div class="grid gap-3 sm:grid-cols-2 lg:grid-cols-[10rem_10rem_auto] lg:items-end">
			<label class="flex flex-col gap-1 text-sm font-semibold">
				<span class="text-foreground/60">From</span>
				<Input
					type="text"
					value={filters.from}
					placeholder="YYYY-MM-DD"
					debounceTime={350}
					onchange={(value) => apply({ from: String(value) })}
					class="bg-background w-full"
				/>
			</label>

			<label class="flex flex-col gap-1 text-sm font-semibold">
				<span class="text-foreground/60">To</span>
				<Input
					type="text"
					value={filters.to}
					placeholder="YYYY-MM-DD"
					debounceTime={350}
					onchange={(value) => apply({ to: String(value) })}
					class="bg-background w-full"
				/>
			</label>

			<div class="flex flex-col gap-1">
				<span class="text-foreground/60 text-sm font-semibold">Schedule</span>
				<ToggleGroup
					mode="single"
					allowEmpty={false}
					selected={scheduleTypeOptions.find((option) => option.value === filters.scheduleType) ?? scheduleTypeOptions[0]}
					keyFn={(option: ScheduleTypeOption) => option.value ?? "ALL"}
					onselect={(option: ScheduleTypeOption | undefined) => apply({ scheduleType: option?.value ?? null })}
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

		<div class="flex flex-wrap items-center gap-2">
			{#if showBucket}
				<ToggleGroup
					mode="single"
					allowEmpty={false}
					selected={statisticsBucketOptions.find((option) => option.value === filters.bucket) ?? statisticsBucketOptions[0]}
					keyFn={(option: StatisticsBucketOption) => option.value}
					onselect={(option: StatisticsBucketOption | undefined) => apply({ bucket: option?.value ?? StatisticsBucket.DAY })}
					class="gap-1"
				>
					{#each statisticsBucketOptions as option (option.value)}
						<ToggleGroupItem
							item={option}
							class="data-active:border-accent data-active:bg-accent data-active:text-accent-foreground px-2.5 py-1.5 text-xs font-semibold"
						>
							{option.label}
						</ToggleGroupItem>
					{/each}
				</ToggleGroup>
			{/if}

			<Button mode="secondary" onclick={reset} class="inline-flex items-center gap-2 text-sm">
				<RotateCcw size={16} />
				Reset
			</Button>
		</div>
	</div>

	<div class="grid gap-4 lg:grid-cols-[minmax(0,1fr)_auto_auto] lg:items-center">
		<div class="flex flex-wrap gap-2">
			{#each transportTypeOptions as option (option.value)}
				<label
					class={[
						"border-border bg-background inline-flex items-center gap-2 rounded-lg border px-2.5 py-1.5 text-xs font-semibold",
						filters.transportTypes.includes(option.value) && "border-accent/60 bg-accent/10 text-accent"
					]}
				>
					<Checkbox
						checked={filters.transportTypes.includes(option.value)}
						onchecked={(checked) => toggleTransportType(option.value, checked)}
					/>
					<span>{option.shortLabel}</span>
				</label>
			{/each}
		</div>

		<label class="border-border bg-background inline-flex items-center gap-2 rounded-lg border px-3 py-2 text-sm font-semibold">
			<Checkbox checked={filters.includeReplacement} onchecked={(checked) => apply({ includeReplacement: checked })} />
			<span>Replacement</span>
		</label>

		<label class="flex items-center gap-2 text-sm font-semibold">
			<Filter size={16} class="text-foreground/60" />
			<span class="text-foreground/60">Min volume</span>
			<Input
				type="number"
				value={filters.minVolume}
				min={0}
				debounceTime={350}
				onchange={(value) => apply({ minVolume: Number(value) || 0 })}
				class="bg-background w-24"
			/>
		</label>
	</div>

	{#if filters.scheduleType === ScheduleType.ARRIVAL || filters.scheduleType === ScheduleType.DEPARTURE}
		<p class="text-foreground/55 text-xs">
			{filters.scheduleType === ScheduleType.ARRIVAL ? "Arrival" : "Departure"} filters apply to stop-event metrics.
		</p>
	{/if}
</section>
