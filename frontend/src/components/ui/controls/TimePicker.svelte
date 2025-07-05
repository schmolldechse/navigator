<script lang="ts">
	import { DateTime } from "luxon";
	import Clock from "lucide-svelte/icons/clock";
	import ChevronDown from "lucide-svelte/icons/chevron-down";
	import ChevronLeft from "lucide-svelte/icons/chevron-left";
	import ChevronRight from "lucide-svelte/icons/chevron-right";
	import X from "lucide-svelte/icons/x";
	import Minus from "lucide-svelte/icons/minus";
	import Plus from "lucide-svelte/icons/plus";
	import TimerReset from "lucide-svelte/icons/timer-reset";
	import Button from "$components/ui/interactive/Button.svelte";
	import { onMount } from "svelte";

	let {
		date = $bindable<DateTime>(DateTime.now().set({ second: 0, millisecond: 0 }))
	}: {
		date: DateTime;
	} = $props();
	let dropdownOpen: boolean = $state<boolean>(false);
	let dropdownElement: HTMLDivElement | undefined = $state<HTMLDivElement | undefined>(undefined);

	let inputTimeout = $state<ReturnType<typeof setTimeout> | null>(null);
	let inputBuffer = $state({ hours: "", minutes: "" });

	const handleInput = (event: KeyboardEvent) => {
		event.stopPropagation();

		const bufferType: "hours" | "minutes" = (event.target as HTMLInputElement).dataset.buffertype as "hours" | "minutes";
		if (!bufferType || (bufferType !== "hours" && bufferType !== "minutes")) return;
		if (!/^\d+$/.test(event.key)) return;

		if (inputTimeout) clearTimeout(inputTimeout);
		inputBuffer[bufferType] += event.key;

		if (inputBuffer[bufferType].length !== 2) {
			date = date.set({ [bufferType]: parseInt(inputBuffer[bufferType], 10) });
			inputTimeout = setTimeout(() => {
				inputBuffer[bufferType] = "";
				inputTimeout = null;
			}, 500);
			return;
		}

		// set the input value if the buffer has 2 values

		const maxValue = bufferType === "hours" ? 23 : 59;
		const value = Math.min(parseInt(inputBuffer[bufferType], 10), maxValue);

		date = date.set({ [bufferType]: value });
		inputBuffer[bufferType] = "";
	};

	const weekdays = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

	const getCalendarDays: DateTime[] = $derived.by(() => {
		const startDate = date.startOf("month").startOf("week");
		const endDate = date.endOf("month").endOf("week");

		const days: DateTime[] = [];
		let currentDate = startDate;

		while (currentDate <= endDate) {
			days.push(currentDate);
			currentDate = currentDate.plus({ day: 1 });
		}
		return days;
	});

	const selectedDisplay = $derived(() => date.toFormat("dd.MM.yyyy - HH:mm"));

	onMount(() => {
		const clickOutside = (event: MouseEvent) => {
			if (!dropdownElement) return;
			if (dropdownElement.contains(event.target as Node)) return;
			dropdownOpen = false;
		};

		document.addEventListener("click", clickOutside);
		return () => document.removeEventListener("click", clickOutside);
	});
</script>

<!-- overflow-hidden needed for highlight effect! -->
<button
	class="bg-primary/15 group relative flex w-full cursor-pointer items-center justify-between overflow-hidden rounded-2xl px-4 py-3 shadow-sm"
	onclick={(event: MouseEvent) => {
		event.stopPropagation();
		dropdownOpen = !dropdownOpen;
	}}
	aria-expanded={dropdownOpen}
	aria-haspopup="true"
>
	<!-- highlight effect on hover -->
	<div class="bg-accent/5 absolute inset-0 opacity-0 transition-opacity group-hover:opacity-100"></div>

	<div class="mr-14 flex items-center gap-x-2">
		<Clock class="stroke-accent" size="30" />
		<span class="font-medium">Date</span>
	</div>

	<div class="flex items-center gap-x-2">
		<span class="text-white/75 italic">{selectedDisplay()}</span>
		<ChevronDown class={`stroke-accent transition-transform duration-200 ${dropdownOpen ? "rotate-180" : ""}`} />
	</div>
</button>

{#if dropdownOpen}
	<div
		bind:this={dropdownElement}
		class="bg-black border-primary absolute top-0 left-0 z-10 flex w-full flex-col gap-y-6 border px-4 py-3 md:left-1/2 md:w-[40%] md:-translate-x-1/2 md:translate-y-1/4 md:rounded-2xl"
		role="dialog"
	>
		<!-- Header -->
		<div class="flex flex-row items-center justify-between">
			<span class="text-xl font-bold">Choose date & time</span>
			<X class="hover:stroke-accent cursor-pointer transition-colors" onclick={() => (dropdownOpen = false)} />
		</div>

		<!-- Month navigation -->
		<div class="flex items-center justify-between px-2">
			<button
				class="hover:text-accent flex items-center gap-x-2 text-white/75 transition-colors duration-200 cursor-pointer"
				onclick={() => (date = date.minus({ month: 1 }))}
				aria-label="One month back"
			>
				<ChevronLeft />
				<span>{date.minus({ month: 1 }).toFormat("MMM")}</span>
			</button>

			<span class="text-lg font-bold">{date.toFormat("MMMM yyyy")}</span>

			<button
				class="hover:text-accent flex items-center gap-x-2 text-white/75 transition-colors duration-200 cursor-pointer"
				onclick={() => (date = date.plus({ month: 1 }))}
				aria-label="One month forward"
			>
				<span>{date.plus({ month: 1 }).toFormat("MMM")}</span>
				<ChevronRight />
			</button>
		</div>

		<!-- Calendar -->
		<div class="grid grid-cols-7 gap-y-1 text-center">
			{#each weekdays as day}
				<div class="py-1 font-medium text-white/75">{day}</div>
			{/each}

			{#each getCalendarDays as day}
				{@const isToday = day.hasSame(DateTime.now(), "day")}
				{@const isSelected = day.hasSame(date, "day")}
				{@const isCurrentMonth = day.hasSame(date, "month")}

				<!-- clickable wrapper; provides a larger hitbox -->
				<div
					class="group flex cursor-pointer items-center justify-center p-1"
					role="button"
					tabindex="0"
					aria-label={`Select date ${day.toFormat("DDD")}`}
					aria-pressed={isSelected}
					onclick={() =>
						(date = date.set({
							day: day.day,
							month: day.month,
							year: day.year
						}))}
					onkeydown={(e) => {
						if (e.key !== "Enter" && e.key !== " ") return;
						e.preventDefault();
						date = date.set({
							day: day.day,
							month: day.month,
							year: day.year
						});
					}}
				>
					<!-- visual appearance -->
					<div
						class={[
							"flex h-9 w-9 items-center justify-center rounded-full transition-colors duration-150 ease-in-out",
							{ "border-accent/75 border": isToday },
							{ "bg-accent font-bold text-black": isSelected },
							{ "text-white/30": !isCurrentMonth },
							{ "group-hover:bg-accent/20": !isSelected }
						]}
					>
						{day.toFormat("dd")}
					</div>
				</div>
			{/each}
		</div>

		<!-- Time picker -->
		<div class="flex flex-col gap-y-2">
			<span class="text-white/75">Pick your time</span>

			<!-- hours/ minutes inputs -->
			<div class="flex items-center gap-x-8">
				<button
					class="bg-input-background border-accent/30 hover:border-accent hover:text-accent rounded-md border p-2 text-white/75 transition-colors cursor-pointer"
					onclick={() => (date = date.minus({ minutes: 15 }))}
					aria-label="Decrease date by 15 minutes"
					title="-15min"
				>
					<Minus />
				</button>

				<div class="flex items-center gap-x-4">
					<input
						type="text"
						class="bg-input-background border-accent/30 focus:border-accent w-16 rounded-md border px-3 py-2 text-center focus:outline-none"
						aria-label="Hours"
						data-buffertype="hours"
						value={date.toFormat("HH").padStart(2, "0")}
						onkeydown={handleInput}
						maxlength={2}
					/>
					<span class="text-xl font-bold">:</span>
					<input
						type="text"
						class="bg-input-background border-accent/30 focus:border-accent w-16 rounded-md border px-3 py-2 text-center focus:outline-none"
						aria-label="Minutes"
						data-buffertype="minutes"
						value={date.toFormat("mm").padStart(2, "0")}
						onkeydown={handleInput}
						maxlength={2}
					/>
				</div>

				<button
					class="bg-input-background border-accent/30 hover:border-accent hover:text-accent rounded-md border p-2 text-white/75 transition-colors cursor-pointer"
					onclick={() => (date = date.plus({ minutes: 15 }))}
					aria-label="Increase date by 15 minutes"
					title="+15min"
				>
					<Plus />
				</button>
			</div>
		</div>

		<!-- Control buttons -->
		<div class="flex items-center gap-x-2 self-end">
			<Button
				class="text-background flex w-fit items-center gap-x-2 rounded-md px-4 py-2"
				onclick={() => (date = DateTime.now().set({ second: 0, millisecond: 0 }))}
			>
				<TimerReset size="22" />
			</Button>
			<Button
				class="text-background flex w-fit items-center gap-x-2 rounded-md px-4 py-2 font-bold"
				onclick={() => (dropdownOpen = false)}
			>
				Select
			</Button>
		</div>
	</div>
{/if}
