<script lang="ts">
	import { DateTime } from "luxon";
	import ChevronLeft from "@lucide/svelte/icons/chevron-left";
	import ChevronRight from "@lucide/svelte/icons/chevron-right";

	interface Props {
		isVisible: boolean;
		multiSelect?: boolean;
		dates: {
			start: DateTime;
			end?: DateTime;
		};
		onchange: (params: { start: DateTime; end?: DateTime }) => void;
		class?: string;
	}

	let { isVisible = $bindable(false), multiSelect = false, dates, onchange, class: classes = "" }: Props = $props();

	let dialog: HTMLDialogElement | undefined = $state(undefined);

	$effect(() => {
		if (!dialog) return;
		isVisible ? dialog.show() : dialog.close();
	});

	let currentMonth: DateTime = $state(DateTime.now().startOf("month"));
	let selectedDates: { start: DateTime; end?: DateTime } = $state(dates);

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
		if (!isVisible) return;

		isVisible = false;
		onchange({ start: selectedDates.start, end: selectedDates.end ?? undefined });
	};

	const selectDate = (date: DateTime) => {
		if (!multiSelect) {
			selectedDates.start = date;
			selectedDates.end = undefined;
			return;
		}

		if (selectedDates.start && selectedDates.end) {
			selectedDates.start = date;
			selectedDates.end = undefined;
		} else if (selectedDates.start && !selectedDates.end) {
			if (date < selectedDates.start) {
				selectedDates.end = selectedDates.start;
				selectedDates.start = date;
			} else selectedDates.end = date;
		} else selectedDates.start = date;
	};
</script>

<svelte:window
	onclick={(event: MouseEvent) => {
		if (!dialog || !isVisible) return;

		if (!(event.target instanceof Node) || dialog.contains(event.target as Node)) return;
		handleClose();
	}}
/>

<dialog
	bind:this={dialog}
	onclose={handleClose}
	onclick={(event) => event.target === dialog && handleClose()}
	class={["bg-background border-muted-foreground/20 absolute right-0 left-auto z-100 w-[400px] rounded-lg border-2", classes]}
>
	<div class="m-2 flex flex-col space-y-2">
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
				{@const isStart = day.hasSame(selectedDates.start, "day")}
				{@const isEnd = !!selectedDates.end && day.hasSame(selectedDates.end, "day")}
				{@const inRange = multiSelect && selectedDates.end && day > selectedDates.start && day < selectedDates.end}
				{@const isCurrentMonth = day.hasSame(currentMonth, "month")}

				<div class="relative py-0.5">
					{#if multiSelect && selectedDates.end}
						<div
							class={[
								"absolute inset-y-0.5 z-0",
								{ "bg-accent/20 right-0 left-1/2": isStart },
								{ "bg-accent/20 right-1/2 left-0": isEnd },
								{ "bg-accent/20 inset-x-0": inRange }
							]}
						></div>
					{/if}

					<button
						onclick={() => selectDate(day)}
						class={[
							"relative z-10 mx-auto flex h-7.5 w-7.5 cursor-pointer items-center justify-center text-sm transition-all duration-300",
							{ "font-bold underline underline-offset-4": day.hasSame(DateTime.now(), "day") },
							{ "bg-accent text-background rounded-lg font-bold": isStart || isEnd },
							{ "text-text": isCurrentMonth && !isStart && !isEnd },
							{ "text-muted-foreground opacity-75": !isCurrentMonth && !isStart && !isEnd },
							{ "hover:bg-accent/20 rounded-lg": !isStart && !isEnd && !inRange }
						]}
					>
						{day.day}
					</button>
				</div>
			{/each}
		</div>
	</div>
</dialog>

<style>
	dialog {
		--dialog-enter-opacity: 0;
		--dialog-enter-scale: 0.95;
		--dialog-animation-duration: 0.15s;

		opacity: var(--dialog-enter-opacity, 1);
		transform: translate3d(0, 0, 0)
			scale3d(var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1));

		transition:
			opacity var(--dialog-animation-duration) ease-out,
			transform var(--dialog-animation-duration) ease-out,
			display var(--dialog-animation-duration) ease-out allow-discrete,
			overlay var(--dialog-animation-duration) ease-out allow-discrete;
	}

	dialog[open] {
		opacity: 1;
		transform: translate3d(0, 0, 0) scale3d(1, 1, 1);
	}

	@starting-style {
		dialog[open] {
			opacity: var(--dialog-enter-opacity, 1);
			transform: translate3d(0, 0, 0)
				scale3d(var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1));
		}
	}

	dialog::backdrop {
		background-color: rgba(0, 0, 0, 0);
		transition:
			display var(--dialog-animation-duration, 1) allow-discrete,
			overlay var(--dialog-animation-duration, 1) allow-discrete,
			background-color var(--dialog-animation-duration, 1);
	}

	dialog[open]::backdrop {
		background-color: rgba(0, 0, 0, 0.2);
	}
</style>
