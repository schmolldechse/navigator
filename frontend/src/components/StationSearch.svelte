<script lang="ts">
	import type { Station } from "$models/station";
	import Search from "$components/svg/Search.svelte";
	import { onMount } from "svelte";
	import { env } from "$env/dynamic/public";

	let { station = $bindable(undefined) }: { station?: Station } = $props();
	let open = $state<boolean>(false);
	let selectedIndex = $state<number>(-1);

	let stations: Station[] = $state([]);

	let inputElement: HTMLInputElement;
	const clickOutside = (event: MouseEvent) => {
		if (inputElement && inputElement.contains(event.target as Node)) return;
		open = false;
		selectedIndex = -1;
	};

	const searchStations = async (query: string) => {
		const searchParams = new URLSearchParams({ query }).toString();

		const response = await fetch(`${env.PUBLIC_BACKEND_BASE_URL}/api/v1/stations?${searchParams}`, { method: "GET" });
		if (!response.ok) return;

		const jsonData = await response.json();
		if (!Array.isArray(jsonData)) return;

		stations = jsonData as Station[];
		open = stations.length > 0;
	};

	const selectStation = (index: number) => {
		station = stations[index];
		inputElement.value = station?.name ?? "";
		open = false;
		selectedIndex = -1;
	};

	let debounce: number;
	const handleInput = () => {
		if (!inputElement) return;
		const value = inputElement.value;

		if (value.length === 0) {
			station = undefined;
			return;
		}

		if (!station || value !== station?.name) {
			clearTimeout(debounce);
			debounce = setTimeout(() => {
				if (value) {
					searchStations(value);
				} else {
					open = false;
				}
			}, 500);
		}
	};

	const handleKeyInput = (event: KeyboardEvent) => {
		if (!open || inputElement.value.length === 0) return;

		switch (event.key) {
			case "ArrowDown":
			case "Tab":
				event.preventDefault();
				selectedIndex = selectedIndex === stations.length - 1 ? 0 : selectedIndex + 1;
				break;
			case "ArrowUp":
				event.preventDefault();
				selectedIndex = selectedIndex === 0 ? stations.length - 1 : selectedIndex - 1;
				break;
			case "Enter":
				if (selectedIndex < 0 || selectedIndex > stations.length) return;
				selectStation(selectedIndex);
				break;
			case "Escape":
				open = false;
				break;
			default:
				break;
		}
	};

	onMount(() => {
		document.addEventListener("click", clickOutside);
		return () => document.removeEventListener("click", clickOutside);
	});
</script>

<div class="relative flex w-full flex-col text-text placeholder:text-text">
	<div
		class="flex flex-row items-center gap-x-1 rounded-2xl bg-primary-dark px-2 font-medium focus-within:ring-2 focus-within:ring-accent md:text-2xl"
	>
		<Search height="50px" width="50px" />
		<input
			bind:this={inputElement}
			type="text"
			class="w-full border-none bg-primary-dark p-2 outline-hidden"
			placeholder="Search for a station"
			onclick={() => (open = true)}
			oninput={handleInput}
			onkeydown={handleKeyInput}
		/>
	</div>

	{#if open && stations.length > 0}
		<div
			class="absolute left-0 top-full z-50 mt-1 flex h-fit max-w-96 flex-col rounded-lg border border-text bg-primary-dark p-2"
		>
			{#each stations as station, index (station)}
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
