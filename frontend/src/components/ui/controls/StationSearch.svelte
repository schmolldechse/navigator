<script lang="ts">
	import type { Station } from "$models/station";
	import { onMount, type Snippet } from "svelte";
	import { env } from "$env/dynamic/public";

	let {
		station = $bindable<Station | undefined>(undefined),
		placeholder = "Search a station...",
		class: className = "",
		children
	}: {
		station?: Station;
		placeholder: string;
		class?: string;
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
	let stationsQueried = $state<Station[]>([]);

	const queryStations = async (query: string) => {
		if (!query) return;

		const searchParams = new URLSearchParams({ query });
		const request = await fetch(`${env.PUBLIC_BACKEND_BASE_URL}/api/v1/stations?${searchParams.toString()}`);
		if (!request.ok) return;

		const response = await request.json();
		if (!Array.isArray(response)) return;

		stationsQueried = response as Station[];
		dropdownOpen = stationsQueried.length > 0;
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

	let debounceTimer = $state<number>();
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
		const onClickOutside = (event: MouseEvent) => {
			if (!inputElement) return;
			if (inputElement.contains(event.target as Node)) return;
			dropdownOpen = false;
		};

		document.addEventListener("click", onClickOutside);
		return () => document.removeEventListener("click", onClickOutside);
	});
</script>

<div class="text-text placeholder:text-text relative flex w-full flex-col">
	<div
		class={["bg-input-background focus-within:ring-accent flex flex-row items-center gap-x-1 rounded-2xl px-2 font-medium focus-within:ring-2 md:text-2xl", className]}
	>
		{@render children?.()}
		<input
			bind:this={inputElement}
			type="text"
			class="w-full border-none p-2 outline-hidden"
			{placeholder}
			onclick={() => (dropdownOpen = true)}
			oninput={handleInput}
			onkeydown={handleNavigation}
		/>
	</div>

	{#if dropdownOpen && stationsQueried.length > 0}
		<div
			class="border-text bg-primary-dark absolute top-full left-0 z-50 mt-1 flex h-fit max-w-96 flex-col rounded-lg border p-2"
		>
			{#each stationsQueried as station, index (station)}
				<button
					tabindex="0"
					onclick={() => selectStation(index)}
					onfocus={() => (selectedIndex = index)}
					class:bg-secondary={selectedIndex === index}
					class="w-full cursor-pointer rounded-md p-1 text-left"
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
