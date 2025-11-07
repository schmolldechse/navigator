<script lang="ts">
	import { DateTime } from "luxon";
	import X from "@lucide/svelte/icons/x";
	import ChevronLeft from "@lucide/svelte/icons/chevron-left";
	import ChevronRight from "@lucide/svelte/icons/chevron-right";

	interface Props {
		dialog: HTMLDialogElement;
		multiSelect?: boolean;
		startDate: DateTime;
		endDate?: DateTime;
		onchange: (params: { start: DateTime; end?: DateTime }) => void;
		onclose?: () => void;
		class?: string;
		title?: string;
	}

	let {
		dialog = $bindable(),
		multiSelect = false,
		startDate,
		endDate,
		onchange,
		onclose,
		class: classes = "",
		title = "Choose Date"
	}: Props = $props();

	let currentMonth: DateTime = $state(DateTime.now().startOf("month"));
	let selectedStartDate: DateTime = $state(startDate);
	let selectedEndDate: DateTime | null = $state(endDate ?? null);

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
			selectedStartDate = date;
			selectedEndDate = null;
			// onchange({ start: selectedStartDate });
			return;
		}

		if (selectedStartDate && selectedEndDate) {
			selectedStartDate = date;
			selectedEndDate = null;
		} else if (selectedStartDate && !selectedEndDate) {
			if (date < selectedStartDate) {
				selectedEndDate = selectedStartDate;
				selectedStartDate = date;
			} else selectedEndDate = date;
		} else selectedStartDate = date;

		// onchange({ start: selectedStartDate, end: selectedEndDate ?? undefined });
	};
</script>

<dialog
	bind:this={dialog}
	onclick={(event) => {
		if (event.target !== dialog) return;
		onclose?.();
	}}
	class={[
		"fixed top-auto left-1/2 min-w-md -translate-x-1/2 bg-transparent md:inset-auto md:top-1/2 md:bottom-auto md:left-1/2 md:w-full md:max-w-xl md:-translate-x-1/2 md:-translate-y-1/2",
		classes
	]}
>
	<div class="flex flex-col space-y-6 rounded-xl border-2 border-muted-foreground/20 bg-background p-4 shadow-lg">
		<!-- Header -->
		<div class="flex flex-row items-center justify-between">
			<h2 class="text-md text-text">{title}</h2>
			<button>
				<X
					class="h-6 w-6 cursor-pointer rounded-md stroke-muted-foreground hover:bg-muted-foreground/10 hover:stroke-accent"
					onclick={() => onclose?.()}
				/>
			</button>
		</div>

		<!-- Navigation -->
		<div class="flex items-center justify-between px-2">
			<button
				class="flex cursor-pointer text-white/75 transition-colors duration-300 hover:text-accent"
				onclick={() => (currentMonth = currentMonth.minus({ months: 1 }))}
			>
				<ChevronLeft class="h-6 w-6" />
				<span>{currentMonth.minus({ months: 1 }).toFormat("MMMM")}</span>
			</button>

			<span class="text-lg font-bold text-text">{currentMonth.toFormat("MMMM yyyy")}</span>

			<button
				class="flex cursor-pointer text-white/75 transition-colors duration-300 hover:text-accent"
				onclick={() => (currentMonth = currentMonth.plus({ months: 1 }))}
			>
				<span>{currentMonth.plus({ months: 1 }).toFormat("MMMM")}</span>
				<ChevronRight class="h-6 w-6" />
			</button>
		</div>

		<!-- Calendar -->
		<div class="grid grid-cols-7 gap-y-1 text-center">
			{#each weekdays as weekday}
				<span class="py-1 font-medium text-text/75">{weekday}</span>
			{/each}

			{#each getCalendarDays as calendarDay}
				{@const isCurrentMonth = calendarDay.hasSame(currentMonth, "month")}
				{@const isSelectedStart = selectedStartDate.hasSame(calendarDay, "day")}
				{@const isSelectedEnd = multiSelect && selectedEndDate?.hasSame(calendarDay, "day")}
				{@const isSelected = isSelectedStart || isSelectedEnd}
				{@const isInRange =
					multiSelect && selectedEndDate ? calendarDay > selectedStartDate && calendarDay < selectedEndDate : false}

				<!-- clickable wrapper to provide a larger hitbox -->
				<button
					class={[
						"flex cursor-pointer items-center justify-center transition-colors duration-300 ease-in-out group-hover:bg-accent/20",
						{ "text-text/30": !isCurrentMonth && !isInRange && !isSelected },
						{ "rounded-l-2xl": isSelectedStart && multiSelect },
						{ "rounded-r-2xl": isSelectedEnd && multiSelect },
						{ "rounded-2xl": !multiSelect && isSelected },
						{ "bg-accent font-bold text-background": isSelected },
						{ "text-text": !isSelected },
						{ "bg-accent/50": isInRange }
					]}
					onclick={() => selectDate(calendarDay)}
					onkeydown={(event) => {
						if (event.key !== "Enter" && event.key !== " ") return;
						event.preventDefault();
						selectDate(calendarDay);
					}}
				>
					{calendarDay.toFormat("dd")}
				</button>
			{/each}
		</div>

		<!-- Interaction -->
		<div class="flex flex-row-reverse">
			<button
				class="cursor-pointer rounded-md border-2 border-muted-foreground/30 px-4 py-2 text-sm font-bold text-text hover:bg-accent/30"
				onclick={() => {
					onchange({ start: selectedStartDate, end: selectedEndDate ?? undefined });
					onclose?.();
				}}
			>
				Apply
			</button>
		</div>
	</div>
</dialog>
