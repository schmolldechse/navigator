<script lang="ts">
	import { onMount } from 'svelte';
	import type {Station} from "../models/models/station";
	let isOpen = $state(false);
	let selectedIndex = $state(-1);
	let selectedValue: Station | undefined = $state(undefined);
	let inputElement: HTMLInputElement;

	let stations: Station[] = $state([]);

	function handleInputClick() {
		isOpen = true;
	}

	function handleKeydown(e: KeyboardEvent) {
		if (!isOpen) return;

		switch (e.key) {
			case 'ArrowDown':
				selectedIndex = selectedIndex === -1
					? 0
					: (selectedIndex + 1) % stations.length;
				break;
			case 'ArrowUp':
				selectedIndex = selectedIndex === -1
					? stations.length - 1
					: (selectedIndex - 1 + stations.length) % stations.length;
				break;
			case 'Enter':
				if (selectedIndex >= 0) {
					selectedValue = stations[selectedIndex];
					isOpen = false;
				}
				break;
			case 'Escape':
				isOpen = false;
				break;
		}
	}

	function selectItem(station: Station) {
		selectedValue = station;
		isOpen = false;
		selectedIndex = -1;
	}

	function clickOutside(event: MouseEvent) {
		if (inputElement.contains(event.target as Node)) return;
		isOpen = false;
		selectedIndex = -1;
	}

	async function searchStations(query: string) {
		const response = await fetch(`http://localhost:8000/api/v1/stations`, {
			method: 'POST',
			body: JSON.stringify({ query: query }),
            headers: {
				'Content-Type': 'application/json',
            }
		});
		if (!response.ok) return;

		const data = await response.json();
		if (!Array.isArray(data)) return;

		const filtered: Station[] = data
			.filter((location: any) => location.evaNr && location.name)
			.map((location: any) => location as Station);

		stations = filtered;
		isOpen = filtered.length > 0;
	}

	let debounceTimeout: number;

	function handleInput(event: Event) {
		const query = (event.target as HTMLInputElement).value;
		clearTimeout(debounceTimeout);
		debounceTimeout = setTimeout(() => {
			if (query) {
				searchStations(query);
			} else {
				isOpen = false;
			}
		}, 500);
	}

	onMount(() => {
		document.addEventListener('click', clickOutside);
		return () => document.removeEventListener('click', clickOutside);
	});
</script>

<div class="relative w-64">
    <input
            bind:this={inputElement}
            bind:value={selectedValue}
            onclick={handleInputClick}
            onfocus={handleInputClick}
            onkeydown={handleKeydown}
            oninput={handleInput}
            aria-expanded={isOpen}
            aria-activedescendant={selectedIndex >= 0 ? `option-${selectedIndex}` : undefined}
            class="bg-primary w-full px-4 py-2 border rounded-lg border-none outline-none focus:outline-2 focus:outline-offset-2 focus:outline-accent text-text"
            placeholder="Search your station..."
    />

    {#if isOpen && stations.length > 0}
        <div class="absolute top-full left-0 w-fit h-fit mt-1 z-50 bg-primary rounded-lg border border-text p-2">
            {#each stations as station, index (station)}
                <button
                        tabindex="0"
                        onclick={() => selectItem(station)}
                        onfocus={() => selectedIndex = index}
                        class:bg-secondary={selectedIndex === index}
                        class="w-full p-1 cursor-pointer text-left rounded-md"
                >
                    {station?.name}
                </button>
            {/each}
        </div>
    {/if}
</div>
