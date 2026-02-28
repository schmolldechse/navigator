<script lang="ts">
	import { DateTime } from "luxon";
	import ChevronLeft from "@lucide/svelte/icons/chevron-left";
	import ChevronRight from "@lucide/svelte/icons/chevron-right";
	import type { ClassValue } from "svelte/elements";

	interface Props {
		multiSelect?: boolean;
		dates: {
			start: DateTime;
			end?: DateTime;
		};
		onchange?: (params: { start: DateTime; end?: DateTime }) => void;
		class?: ClassValue;
	}

	let { multiSelect = false, dates = $bindable({ start: DateTime.now() }), onchange, class: className }: Props = $props();

	let currentMonth: DateTime = $state(dates.start.startOf("month"));

	const weekdays = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

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

	const selectDate = (date: DateTime) => {
		if (!multiSelect) {
			dates.start = date;
			dates.end = undefined;

			onchange?.({ start: dates.start, end: dates.end });
			return;
		}

		if (dates.start && dates.end) {
			dates.start = date;
			dates.end = undefined;
		} else if (dates.start && !dates.end) {
			if (date < dates.start) {
				dates.end = dates.start;
				dates.start = date;
			} else dates.end = date;
		} else dates.start = date;

		onchange?.({ start: dates.start, end: dates.end });
	};
</script>

<div class={["flex flex-col space-y-2", className]}>
	<!-- Navigation -->
	<div class="flex items-center justify-between">
		<button
			class="group hover:bg-accent/10 flex cursor-pointer items-center rounded-md p-2 transition-colors duration-300"
			onclick={() => (currentMonth = currentMonth.minus({ months: 1 }))}
		>
			<ChevronLeft class="text-muted-foreground group-hover:text-accent h-5 w-5" />
		</button>

		<span class="text-text text-base font-semibold">{currentMonth.toFormat("MMMM yyyy")}</span>

		<button
			class="group hover:bg-accent/10 flex cursor-pointer items-center rounded-md p-2 transition-colors duration-300"
			onclick={() => (currentMonth = currentMonth.plus({ months: 1 }))}
		>
			<ChevronRight class="text-muted-foreground group-hover:text-accent h-5 w-5" />
		</button>
	</div>

	<!-- Calendar -->
	<div class="grid grid-cols-7 gap-y-0.5 text-center">
		{#each weekdays as day}
			<span class="text-muted-foreground text-xs font-bold uppercase">{day}</span>
		{/each}

		{#each getCalendarDays as day}
			{@const isStart = day.hasSame(dates.start, "day")}
			{@const isEnd = !!dates.end && day.hasSame(dates.end, "day")}
			{@const inRange = multiSelect && dates.end && day > dates.start && day < dates.end}
			{@const isCurrentMonth = day.hasSame(currentMonth, "month")}

			<div class="relative py-0.5">
				{#if multiSelect && dates.end}
					<div
						class={[
							"absolute inset-y-0.5 z-0",
							isStart ? "bg-accent/20 right-0 left-1/2" : "",
							isEnd ? "bg-accent/20 right-1/2 left-0" : "",
							inRange ? "bg-accent/20 inset-x-0" : ""
						]}
					></div>
				{/if}

				<button
					onclick={() => selectDate(day)}
					class={[
						"relative z-10 mx-auto flex h-7.5 w-7.5 cursor-pointer items-center justify-center text-sm transition-all duration-300",
						day.hasSame(DateTime.now(), "day") ? "font-bold underline underline-offset-4" : "",
						isStart || isEnd ? "bg-accent text-background rounded-lg font-bold" : "",
						isCurrentMonth && !isStart && !isEnd ? "text-text" : "",
						!isCurrentMonth && !isStart && !isEnd ? "text-muted-foreground opacity-75" : "",
						!isStart && !isEnd && !inRange ? "hover:bg-accent/20 rounded-lg" : ""
					]}
				>
					{day.day}
				</button>
			</div>
		{/each}
	</div>
</div>
