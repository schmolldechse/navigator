<script lang="ts">
    import {onMount} from 'svelte';
    import Search from "$components/svg/Search.svelte";
    import type {Station} from "$models/station";

    let isOpen = $state(false);
    let selectedIndex = $state(-1);
    let { selectedStation = $bindable() } : { selectedStation: Station | undefined } = $props();
    let inputText = $state("");

    let inputElement: HTMLInputElement;

    let stations: Station[] = $state([]);

    const handleInputClick = () => {
        isOpen = true;
    }

    const handleKeydown = (e: KeyboardEvent) => {
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
                if (selectedIndex < 0) return;
                selectStation(stations[selectedIndex]);
                break;
            case 'Escape':
                isOpen = false;
                break;
        }
    }

    function selectStation(station: Station | undefined) {
        selectedStation = station;
        inputText = station?.name ?? "";
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
            body: JSON.stringify({query: query}),
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
        if (query.length === 0) {
            selectStation(undefined);
            return;
        }

        inputText = query;

        // only search when no station is selected
        if (!selectedStation || query !== selectedStation.name) {
            clearTimeout(debounceTimeout);
            debounceTimeout = setTimeout(() => {
                if (query) {
                    searchStations(query);
                } else {
                    isOpen = false;
                }
            }, 500);
        }
    }

    onMount(() => {
        document.addEventListener('click', clickOutside);
        return () => document.removeEventListener('click', clickOutside);
    });
</script>

<div class="relative w-full">
    <div class="flex flex-row items-center gap-x-2 bg-primary rounded-2xl px-2 focus-within:ring-2 focus-within:ring-accent">
        <Search height="50px" width="50px" />
        <input
                bind:this={inputElement}
                bind:value={inputText}
                onclick={handleInputClick}
                onfocus={handleInputClick}
                onkeydown={handleKeydown}
                oninput={handleInput}
                aria-expanded={isOpen}
                aria-activedescendant={selectedIndex >= 0 ? `option-${selectedIndex}` : undefined}
                class="bg-primary w-full p-2 border rounded-lg border-none outline-none text-text placeholder:text-text"
                placeholder="Search your station..."
        />
    </div>

    {#if isOpen && stations.length > 0}
        <div class="max-w-96 absolute top-full left-0 h-fit mt-1 z-50 bg-primary rounded-lg border border-text p-2">
            {#each stations as station, index (station)}
                <button
                        tabindex="0"
                        onclick={() => selectStation(station)}
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

<style>
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