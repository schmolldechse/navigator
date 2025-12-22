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

		if (isVisible) dialog?.showModal();
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
	}

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
	class={[
		"m-0 h-fit max-h-none w-full max-w-none bg-transparent p-0 outline-none",
		"fixed inset-x-0 bottom-0 top-auto", // mobile: bottom sheet
		"md:inset-auto md:left-1/2 md:top-1/2 md:max-w-md md:-translate-x-1/2 md:-translate-y-1/2", // desktop: centered
		classes
	]}
>
	<div class="flex w-full flex-col space-y-4 rounded-t-2xl border-t-2 border-muted-foreground/20 bg-background p-6 shadow-2xl md:rounded-2xl md:border-2">
		<!-- Header -->
		<div class="flex items-center justify-between">
			<h2 class="text-lg font-bold text-text">{title}</h2>
			<button
				onclick={() => handleClose()}
				class="rounded-full p-1 transition-colors hover:bg-muted-foreground/10 hover:stroke-accent cursor-pointer"
			>
				<X class="h-5 w-5 stroke-muted-foreground hover:stroke-accent" />
			</button>
		</div>

		<!-- Navigation -->
		<div class="flex items-center justify-between bg-muted/30 rounded-lg p-1">
			<button
				class="group flex items-center gap-1 rounded-md px-2 py-1 duration-300 transition-colors hover:bg-accent/10 cursor-pointer"
				onclick={() => (currentMonth = currentMonth.minus({ months: 1 }))}
			>
				<ChevronLeft class="h-5 w-5 text-muted-foreground group-hover:text-accent" />
				<span class="hidden text-sm sm:inline text-text">{currentMonth.minus({ months: 1 }).toFormat("MMMM")}</span>
			</button>

			<span class="text-base font-semibold text-text">{currentMonth.toFormat("MMMM yyyy")}</span>

			<button
				class="group flex items-center gap-1 rounded-md px-2 py-1 duration-300 transition-colors hover:bg-accent/10 cursor-pointer"
				onclick={() => (currentMonth = currentMonth.plus({ months: 1 }))}
			>
				<span class="hidden text-sm sm:inline text-text">{currentMonth.plus({ months: 1 }).toFormat("MMMM")}</span>
				<ChevronRight class="h-5 w-5 text-muted-foreground group-hover:text-accent" />
			</button>
		</div>

		<!-- Calendar -->
		<div class="grid grid-cols-7 gap-y-1">
			{#each weekdays as weekday}
				<span class="py-2 text-center text-xs font-bold uppercase tracking-wider text-muted-foreground">{weekday}</span>
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
						"cursor-pointer relative flex aspect-square items-center justify-center text-sm duration-300 transition-colors",
						{ "font-bold underline underline-offset-4": isToday },
						{ "text-text": isCurrentMonth && !(isInRange || isSelected) },
						{ "text-muted-foreground": !isCurrentMonth && !isInRange && !isSelected },
						{ "bg-accent font-bold text-background z-10": isSelected },
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
		<div class="flex items-center justify-end pt-2 gap-x-2">
			<button 
				class="text-sm font-medium text-muted-foreground rounded-md px-2 py-1 duration-300 transition-colors hover:bg-accent/10 cursor-pointer" 
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
				class="cursor-pointer rounded-md bg-accent px-2 py-1 text-sm font-bold text-background"
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

    @media (max-width: 767px) {
        dialog[open] {
            animation: slide-up 0.3s ease-out;
        }
    }

    @keyframes slide-up {
        from { transform: translateY(100%); }
        to { transform: translateY(0); }
    }
</style>