<script lang="ts">
	import { DateTime, Info } from "luxon";
	import ChevronLeft from "@lucide/svelte/icons/chevron-left";
	import ChevronRight from "@lucide/svelte/icons/chevron-right";
	import type { ClassValue } from "svelte/elements";
	import Button from "../Button.svelte";

	type TimePickerRange = { start: DateTime; end: DateTime };
	type TimePickerValue = DateTime | TimePickerRange | undefined;

	type Props = {
		isRange?: boolean;
		min?: DateTime;
		max?: DateTime;
		value?: TimePickerValue;
		onchange?: (value: TimePickerValue) => void;
		class?: ClassValue;
	};
	let { isRange = false, min, max, value = $bindable(undefined), onchange, class: className }: Props = $props();

	let hoveredDate: DateTime | null = $state(null);

	// tracks the first click of a range selection
	let partialStart: DateTime | null = $state(null);

	const startDate = $derived.by(() => {
		if (!isRange) return value as DateTime | undefined;
		if (partialStart) return partialStart;
		return (value as TimePickerRange)?.start;
	});
	const endDate = $derived.by(() => {
		if (!isRange) return undefined;
		if (partialStart) return undefined;
		return (value as TimePickerRange)?.end;
	});

	// svelte-ignore state_referenced_locally
	let currentMonth: DateTime = $state((startDate || DateTime.now()).startOf("month"));

	const canGoBack: boolean = $derived(!min || currentMonth > min.startOf("month"));
	const canGoForward: boolean = $derived(!max || currentMonth < max.startOf("month"));

	const isDayDisabled = (day: DateTime): boolean => {
		if (min && day.startOf("day") < min.startOf("day")) return true;
		if (max && day.startOf("day") > max.startOf("day")) return true;
		return false;
	};

	const getCalendarDays: DateTime[] = $derived.by(() => {
		const startDate: DateTime = currentMonth.startOf("month").startOf("week");
		const endDate: DateTime = currentMonth.endOf("month").endOf("week");

		const days: DateTime[] = [];
		let day: DateTime = startDate;

		while (day <= endDate) {
			days.push(day);
			day = day.plus({ days: 1 });
		}
		return days;
	});

	const visualRange: { start: DateTime; end: DateTime } | null = $derived.by(() => {
		if (!isRange || !startDate) return null;

		const endTarget = endDate || hoveredDate;
		if (!endTarget) return null;

		return startDate.startOf("day") < endTarget.startOf("day")
			? { start: startDate, end: endTarget }
			: { start: endTarget, end: startDate };
	});

	const selectDate = (date: DateTime) => {
		if (isDayDisabled(date)) return;

		if (!isRange) {
			value = date;
			onchange?.(date);
			return;
		}

		if (!partialStart) {
			partialStart = date;
			return;
		}

		const start = date < partialStart ? date : partialStart;
		const end = date >= partialStart ? date : partialStart;

		const rangeValue: TimePickerRange = { start, end };
		value = rangeValue;
		onchange?.(rangeValue);

		partialStart = null;
	};
</script>

<div class={["flex w-full min-w-fit flex-col sm:gap-y-2", className]}>
	<!-- Navigation -->
	<div class="flex items-center justify-between px-4 pb-2">
		<Button
			mode="tertiary"
			disabled={!canGoBack}
			onclick={() => canGoBack && (currentMonth = currentMonth.minus({ months: 1 }))}
			class="group p-2!"
		>
			<ChevronLeft size={24} class="text-secondary-foreground group-hover:text-accent" />
		</Button>

		<span class="text-foreground text-base font-semibold">{currentMonth.toFormat("MMMM yyyy")}</span>

		<Button
			mode="tertiary"
			disabled={!canGoForward}
			onclick={() => canGoForward && (currentMonth = currentMonth.plus({ months: 1 }))}
			class="group p-2!"
		>
			<ChevronRight size={24} class="text-secondary-foreground group-hover:text-accent" />
		</Button>
	</div>

	<!-- Weekday Headers -->
	<div class="grid grid-cols-7 text-center">
		{#each Info.weekdays("short") as weekday}
			<span class="text-secondary-foreground/40 text-xs font-medium tracking-wide uppercase">{weekday}</span>
		{/each}
	</div>

	<!-- Day grid -->
	<div tabindex="0" role="grid" class="grid grid-cols-7 gap-y-0.5 text-center" onmouseleave={() => (hoveredDate = null)}>
		{#each getCalendarDays as calendarDay (calendarDay)}
			{@const isSameDay = (dateTime?: DateTime) => !!dateTime && calendarDay.hasSame(dateTime, "day")}
			{@const disabled = isDayDisabled(calendarDay)}

			{@const isToday = isSameDay(DateTime.now())}
			{@const isCurrentMonth = calendarDay.hasSame(currentMonth, "month")}
			{@const isSelected = isSameDay(startDate) || isSameDay(endDate)}
			{@const isHoverTarget = isRange && !endDate && !!hoveredDate && isSameDay(hoveredDate)}

			{@const isVisualStart = !!visualRange && isSameDay(visualRange.start)}
			{@const isVisualEnd = !!visualRange && isSameDay(visualRange.end)}
			{@const isVisualBetween =
				!!visualRange && calendarDay > visualRange.start.startOf("day") && calendarDay < visualRange.end.startOf("day")}

			{@const isStandalone = (!isRange && isSameDay(startDate)) || (isRange && isSameDay(startDate) && !visualRange)}

			<button
				{disabled}
				onclick={() => selectDate(calendarDay)}
				onmouseenter={() => (hoveredDate = calendarDay)}
				class={[
					"mx-auto h-6 w-full text-xs",
					"not-disabled:cursor-pointer disabled:cursor-not-allowed disabled:opacity-30",

					// base selected style
					{ "bg-accent text-accent-foreground font-bold": isSelected },

					// hovered end date preview
					{ "bg-accent/40": isHoverTarget && !isSelected },

					// range highlighting
					{ "bg-accent/20": isVisualBetween },

					// rounding
					{ "rounded-full": isStandalone || (isVisualStart && isVisualEnd) || (isHoverTarget && !visualRange) },
					{ "rounded-l-full": isVisualStart && !isVisualEnd },
					{ "rounded-r-full": isVisualEnd && !isVisualStart },

					{ "font-bold underline underline-offset-4": isToday },
					{ "text-secondary-foreground/25": !isCurrentMonth && !isSelected },
					{ "text-foreground": isCurrentMonth && !isSelected }
				]}
			>
				{calendarDay.toFormat("d")}
			</button>
		{/each}
	</div>
</div>
