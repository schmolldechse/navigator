<script lang="ts">
	import type { MeasuredTimeframeStatisticDTO } from "$lib/models/MeasuredTimeframeStatisticDTO";
	import { formatBytes, getUnit, isUnitBytes } from "$lib/util/bytes";
	import X from "@lucide/svelte/icons/x";
	import { Chart, Svg, Axis, Spline, Highlight } from "layerchart";
	import { DateTime, Duration } from "luxon";
	import TimePickerDialog from "../interactable/TimePickerDialog.svelte";

	interface Props {
		dialog: HTMLDialogElement;
		title: string;
		measuredStatistic: MeasuredTimeframeStatisticDTO | null;
		onclose?: () => void;
		ontimerangechange?: (timerange: { start: DateTime; end: DateTime }) => void;
	}

	let { dialog = $bindable(), title, measuredStatistic, onclose, ontimerangechange }: Props = $props();

	let timePickerDialog: HTMLDialogElement;

	interface TimerangeOption {
		label: string;
		value?: Duration;
		isCustom?: boolean;
	}

	let selectableTimeranges: TimerangeOption[] = [
		{ label: "24h", value: Duration.fromObject({ hours: 24 }) },
		{ label: "7d", value: Duration.fromObject({ days: 7 }) },
		{ label: "30d", value: Duration.fromObject({ days: 30 }) },
		{ label: "Custom", isCustom: true }
	];
	let selectedOption: TimerangeOption = $state(selectableTimeranges[0]);
	let lastSelectedOption: TimerangeOption = $state(selectableTimeranges[0]);

	let selectedTimerange: { start: DateTime; end: DateTime } | null = $state({
		start: DateTime.now().minus(selectedOption.value!),
		end: DateTime.now()
	});
</script>

<dialog
	bind:this={dialog}
	onclick={(event) => {
		if (event.target !== dialog) return;
		onclose?.();
	}}
	class="fixed top-auto left-1/2 -translate-x-1/2 bg-transparent backdrop:bg-background/80 backdrop:backdrop-blur-sm md:inset-auto md:top-1/2 md:bottom-auto md:left-1/2 md:w-full md:max-w-xl md:-translate-x-1/2 md:-translate-y-1/2"
>
	<div class="space-y-6 rounded-xl border-2 border-muted-foreground/20 bg-background p-4 shadow-lg">
		<!-- Header -->
		<div class="flex items-center justify-between">
			<h2 class="text-2xl text-text">{title}</h2>
			<button>
				<X
					class="h-6 w-6 cursor-pointer rounded-md stroke-muted-foreground hover:bg-muted-foreground/10 hover:stroke-accent"
					onclick={() => onclose?.()}
				/>
			</button>
		</div>

		<!-- Timerange selection -->
		<div class="flex gap-2">
			{#each selectableTimeranges as timerange}
				{@const isSelected = selectedOption.label === timerange.label}

				<div class="flex gap-2">
					<button
						class={[
							"cursor-pointer rounded-xl border-2 border-muted-foreground/70 px-4 py-1 text-xs",
							{ "border-accent/90 bg-accent/90 font-bold text-background": isSelected },
							{ "text-text hover:border-accent/75 hover:bg-accent/10": !isSelected }
						]}
						onclick={() => {
							lastSelectedOption = selectedOption;
							selectedOption = timerange;

							if (timerange.isCustom) timePickerDialog.showModal();
							else {
								selectedTimerange = { start: DateTime.now().minus(timerange.value!), end: DateTime.now() };
								ontimerangechange?.(selectedTimerange);
							}
						}}
					>
						{timerange.label}
					</button>

					{#if timerange.isCustom}
						{#if selectedTimerange}
							<div>
								<span>{selectedTimerange?.start.toFormat("yyyy-MM-dd")}</span>
								<span> - </span>
								<span>{selectedTimerange?.end.toFormat("yyyy-MM-dd")}</span>
							</div>
						{/if}

						<TimePickerDialog
							bind:dialog={timePickerDialog}
							startDate={DateTime.now()}
							endDate={DateTime.now().plus({ days: 1 })}
							onchange={({ start, end }) => {
								if (!end) return;
								selectedTimerange = { start, end };
								ontimerangechange?.(selectedTimerange);
							}}
							onclose={() => {
								selectedOption = lastSelectedOption;
								timePickerDialog.close();
							}}
							class="z-60 backdrop:backdrop-blur-xs"
							title="Select Custom Time Range"
							multiSelect
						/>
					{/if}
				</div>
			{/each}
		</div>

		{#if !measuredStatistic}
			<div class="h-[300px] min-w-[300px] animate-pulse rounded-md bg-muted-foreground/20 p-4"></div>
		{:else}
			<div class="wrapper bg-revert h-[300px] min-w-[300px] p-4">
				<Chart
					data={measuredStatistic.values.map((item) => ({
						date: new Date(item.date),
						value: formatBytes(item.value, true)
					}))}
					x="date"
					y="value"
					yNice
					padding={{ left: 16, bottom: 24 }}
					tooltip={{ mode: "bisect-x" }}
				>
					<Svg>
						<Axis
							placement="left"
							grid
							rule
							label={isUnitBytes(measuredStatistic.unit) ? getUnit(measuredStatistic.total, true) : ""}
							labelPlacement="start"
						/>
						<Axis placement="bottom" rule />
						<Spline class="stroke-accent stroke-2" />
						<Highlight points lines />
					</Svg>
				</Chart>
			</div>
		{/if}
	</div>
</dialog>

<style>
	.wrapper :global(*) {
		font-family: revert;
		font-weight: revert;
	}

	:global(text > tspan) {
		stroke-width: 0;
		fill: var(--color-text) !important;
	}

	:global(line) {
		stroke: var(--color-muted-foreground) !important;
	}
</style>
