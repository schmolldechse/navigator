<script lang="ts">
	import type { Station } from "@lib/api";
	import { searchStations } from "@lib/remote/station.remote";
	import Input from "@lib/components/ui/Input.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import ArrowRight from "@lucide/svelte/icons/arrow-right";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import LoaderCircle from "@lucide/svelte/icons/loader-circle";
	import MapPin from "@lucide/svelte/icons/map-pin";
	import Search from "@lucide/svelte/icons/search";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		onselect: (station: Station) => void;
		class?: ClassValue;
	};
	let { onselect, class: className }: Props = $props();

	let searchTerm: string = $state("");

	let promise: Promise<Station[]> | undefined = $state();
	let dropdownOpen: boolean = $state(false);
	let activeIndex: number = $state(-1);

	const inputId = $props.id();
	const listboxId = `${inputId}-results`;

	const stationRows = (stations: Station[]) => stations.slice(0, 10);

	const search = (value: string | number = searchTerm) => {
		const rawSearchTerm = String(value);
		const nextSearchTerm = rawSearchTerm.trim();

		searchTerm = rawSearchTerm;
		activeIndex = -1;

		if (nextSearchTerm.length < 2) {
			promise = undefined;
			dropdownOpen = false;
			return;
		}

		dropdownOpen = true;

		promise = searchStations({
			request: {
				searchTerm: nextSearchTerm,
				maxResults: 10,
				locationTypes: ["ALL"]
			}
		}).run();
	};

	const selectStation = (station: Station) => {
		searchTerm = station.name;
		promise = undefined;
		dropdownOpen = false;
		activeIndex = -1;

		onselect(station);
	};

	const openDropdown = () => {
		if (searchTerm.trim().length >= 2 && promise) {
			dropdownOpen = true;
		}
	};

	const handleKeydown = (event: KeyboardEvent) => {
		if (event.key === "Escape") {
			dropdownOpen = false;
			activeIndex = -1;
			return;
		}

		if (event.key === "ArrowDown" || event.key === "ArrowUp") {
			dropdownOpen = true;
		}
	};

	const closeDropdownOnFocusOut = (event: FocusEvent) => {
		const nextTarget = event.relatedTarget;
		if (nextTarget instanceof Node && event.currentTarget instanceof Node && event.currentTarget.contains(nextTarget)) {
			return;
		}

		dropdownOpen = false;
		activeIndex = -1;
	};
</script>

<div class={["flex flex-col gap-3", className]} onfocusout={closeDropdownOnFocusOut}>
	<label class="text-foreground/70 flex items-center gap-2 text-sm font-semibold" for={inputId}>
		<Search size={16} class="text-accent" />
		Search station
	</label>

	<div class="relative">
		<Input
			type="text"
			bind:value={searchTerm}
			id={inputId}
			minlength={2}
			maxlength={80}
			debounceTime={300}
			placeholder="Search for a station..."
			role="combobox"
			aria-autocomplete="list"
			aria-controls={listboxId}
			aria-expanded={dropdownOpen}
			aria-activedescendant={activeIndex >= 0 ? `${listboxId}-${activeIndex}` : undefined}
			onchange={search}
			onfocus={openDropdown}
			onkeydown={handleKeydown}
			class="w-full"
		/>

		{#if dropdownOpen}
			<div
				id={listboxId}
				role="listbox"
				class="border-border bg-background absolute top-full right-0 left-0 z-50 mt-2 max-h-80 overflow-auto rounded-lg border shadow-lg"
			>
				<div class="flex flex-col gap-y-2 p-1">
					{#if promise}
						{#await promise}
							<div class="text-foreground/60 flex items-center gap-2 px-3 py-2 text-sm font-semibold">
								<LoaderCircle size={16} class="animate-spin" />
								Searching stations
							</div>

							<Skeleton class="h-10 w-full" />
							<Skeleton class="h-10 w-full" />
							<Skeleton class="h-10 w-full" />
						{:then stations}
							{@const rows = stationRows(stations)}

							{#if rows.length === 0}
								<p class="text-foreground/60 px-3 py-2 text-sm">No station found.</p>
							{:else}
								{#each rows as station, index (station.evaNumber)}
									<button
										id={`${listboxId}-${index}`}
										type="button"
										role="option"
										aria-selected={activeIndex === index}
										onclick={() => selectStation(station)}
										onmouseenter={() => (activeIndex = index)}
										onfocus={() => (activeIndex = index)}
										class={[
											"hover:bg-accent/10 focus-visible:bg-accent/10 grid w-full cursor-pointer grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 rounded-md px-3 py-2 text-left text-sm font-semibold transition-colors outline-none",
											activeIndex === index && "bg-accent/10 text-accent"
										]}
									>
										<MapPin size={16} class="text-accent shrink-0" />
										<span class="block truncate">{station.name}</span>
										<ArrowRight size={16} class="text-foreground/45 shrink-0" />
									</button>
								{/each}
							{/if}
						{:catch error}
							<div class="flex items-start gap-2 p-2">
								<CircleAlert size={18} class="text-destructive mt-0.5 shrink-0" />

								<div>
									<p class="text-foreground text-sm font-semibold">Station search failed</p>
									<p class="text-foreground/60 text-xs">
										{error instanceof Error ? error.message : "Please try again."}
									</p>
								</div>
							</div>
						{/await}
					{/if}
				</div>
			</div>
		{/if}
	</div>
</div>
