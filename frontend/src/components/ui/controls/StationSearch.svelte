<script lang="ts">
	import { onMount, type Snippet } from "svelte";
	import { env } from "$env/dynamic/public";
	import type { Station } from "$models/models";

	let {
		station = $bindable<Station | undefined>(undefined),
		placeholder = "Search a station...",
		children
	}: {
		station?: Station;
		placeholder: string;
		children?: Snippet;
	} = $props();
	let inputElement: HTMLInputElement;

	// Synchronizes the input value with the station name.
	$effect(() => {
		if (!station) return;
		if (!inputElement) return;
		inputElement.value = station.name;
	});

	let dropdownOpen = $state<boolean>(false);
	let dropdownElement: HTMLDivElement | undefined = $state<HTMLDivElement | undefined>(undefined);

	let stationsQueried = $state<Station[]>([]);
	const queryStations = async (query: string) => {
		if (!query) return;

		const searchParams = new URLSearchParams({ query });
		const request = await fetch(`${env.PUBLIC_BACKEND_URL}/api/station/?${searchParams.toString()}`);
		if (!request.ok) return;

		const response = await request.json();
		if (!Array.isArray(response)) return;

		stationsQueried = response as Station[];
		dropdownOpen = stationsQueried.length > 0;
		selectedIndex = 0;
	};

	let selectedIndex = $state<number>(-1);
	const selectStation = (index: number) => {
		if (index < 0 || index > stationsQueried.length) return;
		if (!stationsQueried[index]) return;

		station = stationsQueried[index];
		inputElement.value = station.name;
		dropdownOpen = false;

		selectedIndex = -1;
	};

	let debounceTimer = $state<ReturnType<typeof setTimeout>>();
	const handleInput = () => {
		if (!inputElement) return;
		if (inputElement.value.length === 0) {
			station = undefined;
			return;
		}

		if (station?.name === inputElement.value) return;

		if (debounceTimer) clearTimeout(debounceTimer);
		debounceTimer = setTimeout(() => queryStations(inputElement.value), 500);
	};

	const handleNavigation = (event: KeyboardEvent) => {
		if (!dropdownOpen) return;
		if (inputElement.value.length === 0) return;

		switch (event.key) {
			case "ArrowDown":
			case "Tab":
				event.preventDefault();
				selectedIndex = selectedIndex === stationsQueried.length - 1 ? 0 : selectedIndex + 1;
				break;
			case "ArrowUp":
				event.preventDefault();
				selectedIndex = selectedIndex === 0 ? stationsQueried.length - 1 : selectedIndex - 1;
				break;
			case "Enter":
				event.preventDefault();
				if (selectedIndex < 0 || selectedIndex > stationsQueried.length) return;
				selectStation(selectedIndex);
				break;
			case "Escape":
				event.preventDefault();
				dropdownOpen = false;
				break;
			default:
				break;
		}
	};

	/**
	 * Registers an event handler.
	 * Closes the dropdown when there's a click outside the input/ dropdown
	 */
	onMount(() => {
		const clickOutside = (event: MouseEvent) => {
			if (!dropdownElement) return;
			if (dropdownElement.contains(event.target as Node)) return;
			if (inputElement && inputElement.contains(event.target as Node)) return;

			dropdownOpen = false;
			if (!station && stationsQueried.length > 0) {
				selectStation(0);
				return;
			}
		};

		document.addEventListener("click", clickOutside);
		return () => document.removeEventListener("click", clickOutside);
	});
</script>

<div class="relative">
	<button
		class={[
			"bg-primary/25 group relative w-full cursor-text overflow-hidden rounded-2xl px-4 py-1 shadow-sm",
			{ "rounded-t-2xl rounded-b-none": placeholder === "Start" },
			{ "rounded-t-none rounded-b-2xl": placeholder === "Destination" }
		]}
		onclick={(event) => {
			event.stopPropagation();
			inputElement.focus();
			inputElement.setSelectionRange(inputElement.value.length, inputElement.value.length);
			dropdownOpen = true;
		}}
		aria-expanded={dropdownOpen}
		aria-haspopup="true"
	>
		<!-- highlight effect on hover -->
		<div class="bg-accent/5 absolute inset-0 opacity-0 transition-opacity group-hover:opacity-100"></div>

		<div class="flex items-center">
			{@render children?.()}
			<input
				bind:this={inputElement}
				type="text"
				class="w-full border-none bg-transparent p-2 font-medium outline-none md:text-lg"
				{placeholder}
				oninput={handleInput}
				onkeydown={handleNavigation}
			/>
		</div>
	</button>

	{#if dropdownOpen && stationsQueried?.length > 0}
		<div
			bind:this={dropdownElement}
			class="border-primary absolute z-10 mt-1 flex w-full flex-col gap-y-1 rounded-b-xl border bg-black p-3 shadow-md"
		>
			{selectStation(stationsQueried[0])}
			{#each stationsQueried as station, index (station)}
				<button
					tabindex="0"
					onclick={() => selectStation(index)}
					onfocus={() => (selectedIndex = index)}
					class={[
						"w-full cursor-pointer rounded-md px-2 py-1 text-left hover:bg-white/15",
						{ "bg-white/15": selectedIndex === index }
					]}
				>
					{station?.name}
				</button>
			{/each}
		</div>
	{/if}
</div>

<style lang="postcss">
	::placeholder {
		color: var(--text);
		opacity: 0.75;
	}

	::-webkit-input-placeholder {
		color: var(--text);
	}

	::-moz-placeholder {
		color: var(--text);
		opacity: 0.75;
	}

	:-ms-input-placeholder {
		color: var(--text);
	}
</style>
