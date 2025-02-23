<script lang="ts">
	import { onMount } from "svelte";
	import { DateTime } from "luxon";
	import Calendar from "$components/ui/icons/Calendar.svelte";

	let { selectedDate = $bindable(DateTime.now().set({ second: 0, millisecond: 0 })) } = $props();

	let isOpen: boolean = $state(false);
	let inputElement: HTMLDivElement;

	let inputTimeout: number;
	let buffer = { current: { hours: "", minutes: "" } };

	function clickOutside(event: MouseEvent) {
		if (inputElement.contains(event.target as Node)) return;
		isOpen = false;
	}

	onMount(() => {
		document.addEventListener("click", clickOutside);
		return () => document.removeEventListener("click", clickOutside);
	});

	const getDays = (): DateTime[] => {
		const startDate = selectedDate.startOf("month").startOf("week");
		const endDate = selectedDate.endOf("month").endOf("week");

		let days: DateTime[] = [];
		let day = startDate;

		while (day <= endDate) {
			days.push(day);
			day = day.plus({ days: 1 });
		}

		return days;
	};

	const adjustTimeByMinutes = (amount: number) => {
		selectedDate = selectedDate.plus({ minutes: amount });
	};

	const handleKeyPress = (e: KeyboardEvent) => {
		const part = (e.target as HTMLInputElement).dataset.part;
		if (!part || (part !== "minutes" && part !== "hours")) return;
		if (!/^\d+$/.test(e.key)) return;

		clearTimeout(inputTimeout);
		buffer.current[part] += e.key;

		if (buffer.current[part].length === 2) {
			// validate if two digits are entered
			const maxValue = part === "hours" ? 23 : 59;
			const validatedValue = Math.min(parseInt(buffer.current[part], 10), maxValue);

			selectedDate =
				part === "hours" ? selectedDate.set({ hour: validatedValue }) : selectedDate.set({ minute: validatedValue });

			// reset buffer
			buffer.current[part] = "";
		} else {
			selectedDate =
				part === "hours"
					? selectedDate.set({ hour: parseInt(e.key, 10) || 0 })
					: selectedDate.set({ minute: parseInt(e.key, 10) || 0 });

			inputTimeout = setTimeout(() => (buffer.current[part] = ""), 500);
		}
	};
</script>

<div bind:this={inputElement} class="relative w-fit">
	<button
		type="button"
		class:ring-2={isOpen}
		class:ring-accent={isOpen}
		class="hover:ring-accent flex flex-row items-center gap-x-2 rounded-md p-0.5 text-center hover:ring-2 md:p-2 md:text-2xl"
		onclick={() => (isOpen = true)}
	>
		<Calendar />
		<span>{selectedDate.toFormat("dd.MM.yyyy - HH:mm")}</span>
	</button>

	{#if isOpen}
		<div
			class="bg-primary absolute top-1/2 left-1/2 z-10 mt-8 w-80 -translate-x-1/4 transform rounded-sm p-4 shadow-lg md:top-full md:left-auto md:mt-2 md:w-128 md:transform-none"
		>
			<!-- header -->
			<div class="mb-4 flex items-center justify-between">
				<button onclick={() => (selectedDate = selectedDate.minus({ months: 1 }))}>
					&lt; {selectedDate.minus({ months: 1 }).toFormat("MMM")}</button
				>
				<div class={"font-bold"}>{selectedDate.toFormat("MMMM yyyy")}</div>
				<button onclick={() => (selectedDate = selectedDate.plus({ months: 1 }))}
					>{selectedDate.plus({ months: 1 }).toFormat("MMM")}
					&gt;
				</button>
			</div>

			<!-- calendar grid -->
			<div class="grid grid-cols-7 gap-1 text-sm">
				{#each ["Mo", "Tu", "We", "Th", "Fr", "Sa", "Su"] as day}
					<div class="text-center font-bold">
						{day}
					</div>
				{/each}

				{#each getDays() as day}
					<button
						class="p-2 text-center {day.hasSame(selectedDate, 'month') ? '' : 'text-gray-400'} {day.hasSame(
							selectedDate,
							'day'
						)
							? 'bg-accent rounded-md font-bold text-black'
							: ''}"
						onclick={() => (selectedDate = day.set({ hour: selectedDate.hour, minute: selectedDate.minute }))}
					>
						{day.day}
					</button>
				{/each}
			</div>

			<!-- time inputs -->
			<div class="mt-4 flex items-center justify-between space-x-2 font-bold">
				<button class="bg-accent rounded-sm px-4 py-1 text-black md:px-3" onclick={() => adjustTimeByMinutes(-15)}>
					-
				</button>

				<div class="flex items-center space-x-2">
					<input
						type="text"
						name="hours"
						data-part="hours"
						value={String(selectedDate.hour).padStart(2, "0")}
						onkeydown={handleKeyPress}
						maxLength={2}
						readOnly={true}
						class="text-text w-full rounded-sm border border-none bg-transparent text-center text-xl focus:outline-hidden"
					/>
					<span>:</span>
					<input
						type="text"
						name="minutes"
						data-part="minutes"
						value={String(selectedDate.minute).padStart(2, "0")}
						onkeydown={handleKeyPress}
						maxLength={2}
						readOnly={true}
						class="text-text w-full rounded-sm border border-none bg-transparent text-center text-xl focus:outline-hidden"
					/>
				</div>

				<button class="bg-accent rounded-sm px-4 py-1 text-black md:px-3" onclick={() => adjustTimeByMinutes(15)}>
					+
				</button>
			</div>

			<!-- select button -->
			<button class="bg-accent mt-4 w-full rounded-sm py-2 font-bold text-black" onclick={() => (isOpen = false)}>
				Select
			</button>
		</div>
	{/if}
</div>
