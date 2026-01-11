<script lang="ts">
	import { DateTime } from "luxon";
	import X from "@lucide/svelte/icons/x";
	import ChevronLeft from "@lucide/svelte/icons/chevron-left";
	import ChevronRight from "@lucide/svelte/icons/chevron-right";

	interface Props {
		isVisible: boolean;
		multiSelect?: boolean;
		startDate: DateTime;
		endDate?: DateTime;
		onchange: (params: { start: DateTime; end?: DateTime }) => void;
		onclose?: () => void;
		class?: string;
		title?: string;
	}

	let {
		isVisible = $bindable(false),
		multiSelect = false,
		startDate,
		endDate,
		onchange,
		onclose,
		class: classes = "",
		title = "Choose Date"
	}: Props = $props();

	let dialog: HTMLDialogElement | undefined = $state(undefined);

	$effect(() => {
		if (!dialog) return;

		if (isVisible) dialog?.show();
		else dialog?.close();
	});

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

	const handleClose = () => {
		isVisible = false;
		onclose?.();
	};

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
	onclose={handleClose}
	onclick={(event) => event.target === dialog && handleClose()}
	class={["bg-background border-muted-foreground/20 absolute right-0 left-auto z-100 w-[600px] rounded-2xl border-2", classes]}
>
	<div class=" flex w-full flex-col space-y-4 p-6 shadow-2xl">
		<!-- Header -->
		<div class="flex items-center justify-between">
			<h2 class="text-text text-lg font-bold">{title}</h2>
			<button
				onclick={() => handleClose()}
				class="hover:bg-muted-foreground/10 hover:stroke-accent cursor-pointer rounded-full p-1 transition-colors"
			>
				<X class="stroke-muted-foreground hover:stroke-accent h-5 w-5" />
			</button>
		</div>

		<!-- Navigation -->
		<div class="bg-muted/30 flex items-center justify-between rounded-lg p-1">
			<button
				class="group hover:bg-accent/10 flex cursor-pointer items-center gap-1 rounded-md px-2 py-1 transition-colors duration-300"
				onclick={() => (currentMonth = currentMonth.minus({ months: 1 }))}
			>
				<ChevronLeft class="text-muted-foreground group-hover:text-accent h-5 w-5" />
				<span class="text-text hidden text-sm sm:inline">{currentMonth.minus({ months: 1 }).toFormat("MMMM")}</span>
			</button>

			<span class="text-text text-base font-semibold">{currentMonth.toFormat("MMMM yyyy")}</span>

			<button
				class="group hover:bg-accent/10 flex cursor-pointer items-center gap-1 rounded-md px-2 py-1 transition-colors duration-300"
				onclick={() => (currentMonth = currentMonth.plus({ months: 1 }))}
			>
				<span class="text-text hidden text-sm sm:inline">{currentMonth.plus({ months: 1 }).toFormat("MMMM")}</span>
				<ChevronRight class="text-muted-foreground group-hover:text-accent h-5 w-5" />
			</button>
		</div>

		<!-- Calendar -->
		<div class="grid grid-cols-7 gap-y-1">
			{#each weekdays as weekday}
				<span class="text-muted-foreground py-2 text-center text-xs font-bold tracking-wider uppercase">{weekday}</span>
			{/each}

			{#each getCalendarDays as calendarDay}
				{@const isToday = calendarDay.hasSame(DateTime.now(), "day")}
				{@const isCurrentMonth = calendarDay.hasSame(currentMonth, "month")}
				{@const isSelectedStart = selectedStartDate.hasSame(calendarDay, "day")}
				{@const isSelectedEnd = multiSelect && selectedEndDate?.hasSame(calendarDay, "day")}
				{@const isSelected = isSelectedStart || isSelectedEnd}
				{@const isInRange =
					multiSelect && selectedEndDate ? calendarDay > selectedStartDate && calendarDay < selectedEndDate : false}

				<!-- clickable wrapper to provide a larger hitbox -->
				<button
					class={[
						"relative flex aspect-square cursor-pointer items-center justify-center text-sm transition-colors duration-300",
						{ "font-bold underline underline-offset-4": isToday },
						{ "text-text": isCurrentMonth && !(isInRange || isSelected) },
						{ "text-muted-foreground": !isCurrentMonth && !isInRange && !isSelected },
						{ "bg-accent text-background z-10 font-bold": isSelected },
						{ "rounded-l-2xl": isSelectedStart && multiSelect },
						{ "rounded-r-2xl": isSelectedEnd && multiSelect },
						{ "bg-accent/20 text-accent font-medium": isInRange && !(isSelectedStart || isSelectedEnd) },
						{ "hover:bg-accent/10 hover:rounded-lg": !isSelected && !isInRange }
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

		<!-- Interactions -->
		<div class="flex items-center justify-end gap-x-2 pt-2">
			<button
				class="text-muted-foreground hover:bg-accent/10 cursor-pointer rounded-md px-2 py-1 text-sm font-medium transition-colors duration-300"
				onclick={() => {
					selectedStartDate = DateTime.now();
					if (multiSelect) selectedEndDate = DateTime.now();
					else selectedEndDate = null;

					currentMonth = DateTime.now().startOf("month");
				}}
			>
				Today
			</button>

			<button
				class="bg-accent text-background cursor-pointer rounded-md px-2 py-1 text-sm font-bold"
				onclick={() => {
					onchange({ start: selectedStartDate, end: selectedEndDate ?? undefined });
					handleClose();
				}}
			>
				Apply
			</button>
		</div>
	</div>
</dialog>

<style>
	dialog::backdrop {
		background: rgba(0, 0, 0, 0.7);
		backdrop-filter: blur(8px);
	}
</style>
