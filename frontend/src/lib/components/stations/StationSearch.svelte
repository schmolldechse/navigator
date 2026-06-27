<script lang="ts">
	import type { Station } from "@lib/api";
	import TransportTypePill from "@lib/components/transport-types/TransportTypePill.svelte";
	import { getTransportTypeIconGroups } from "@lib/components/transport-types/transport-type-icons";
	import * as DropdownMenu from "@lib/components/ui/dropdown-menu";
	import type { DropdownMenuTriggerChildProps } from "@lib/components/ui/dropdown-menu";
	import Input, { type InputValue } from "@lib/components/ui/Input.svelte";
	import { searchStations } from "@lib/remote/station.remote";
	import LoaderCircle from "@lucide/svelte/icons/loader-circle";
	import Search from "@lucide/svelte/icons/search";
	import X from "@lucide/svelte/icons/x";
	import { onDestroy } from "svelte";
	import type { ClassValue } from "svelte/elements";

	const searchId = $props.id();

	type Props = {
		value?: string;
		label?: string;
		placeholder?: string;
		maxResults?: number;
		onselect?: (station: Station) => void;
		class?: ClassValue;
	};

	let {
		value = $bindable(""),
		label = "Find station statistics",
		placeholder = "Search station by name",
		maxResults = 10,
		onselect,
		class: className
	}: Props = $props();

	let open = $state(false);
	let results: Station[] = $state([]);
	let loading = $state(false);
	let errorMessage: string | undefined = $state(undefined);
	let hasSearched = $state(false);
	let debounceTimer: ReturnType<typeof setTimeout> | undefined = $state(undefined);
	let requestSequence = 0;

	const trimmedValue = $derived(value.trim());
	const canSearch = $derived(trimmedValue.length >= 2);
	const statusMessage = $derived.by(() => {
		if (!trimmedValue) return "Type a station name to see suggestions.";
		if (!canSearch) return "Enter at least 2 characters.";
		if (loading) return "Searching stations...";
		if (errorMessage) return errorMessage;
		if (hasSearched && results.length === 0) return "No stations found.";
		return undefined;
	});

	const resetSearchState = () => {
		results = [];
		hasSearched = false;
		errorMessage = undefined;
		loading = false;
	};

	const clearDebounce = () => {
		if (!debounceTimer) return;

		clearTimeout(debounceTimer);
		debounceTimer = undefined;
	};

	const runSearch = async (searchTerm: string) => {
		const currentRequest = ++requestSequence;
		loading = true;
		errorMessage = undefined;
		hasSearched = true;

		try {
			const stations = await searchStations({
				request: {
					searchTerm,
					maxResults,
					locationTypes: ["ALL"]
				}
			}).run();

			if (currentRequest !== requestSequence) return;

			results = stations;
			open = true;
		} catch (error) {
			if (currentRequest !== requestSequence) return;

			results = [];
			errorMessage = error instanceof Error ? error.message : "Station search failed.";
		} finally {
			if (currentRequest === requestSequence) loading = false;
		}
	};

	const scheduleSearch = (query = value) => {
		const searchTerm = query.trim();
		clearDebounce();

		if (searchTerm.length < 2) {
			requestSequence += 1;
			resetSearchState();
			open = Boolean(searchTerm);
			return;
		}

		open = true;
		debounceTimer = setTimeout(() => {
			debounceTimer = undefined;
			void runSearch(searchTerm);
		}, 250);
	};

	const reopenSuggestions = () => {
		if (!trimmedValue) return;

		open = true;
		if (canSearch && !hasSearched && !loading) void runSearch(trimmedValue);
	};

	const handleInputChange = (nextValue: InputValue) => {
		value = String(nextValue);
		scheduleSearch(value);
	};

	const selectStation = (station: Station) => {
		value = station.name;
		open = false;
		onselect?.(station);
	};

	const clear = () => {
		value = "";
		open = false;
		requestSequence += 1;
		resetSearchState();
		clearDebounce();
	};

	const getStationTransportGroups = (station: Station) => getTransportTypeIconGroups(station.transports);

	onDestroy(clearDebounce);
</script>

{#snippet stationSearchTrigger({ props }: DropdownMenuTriggerChildProps)}
	<div class="relative">
		<Search size={17} class="text-foreground/45 pointer-events-none absolute top-1/2 left-3 -translate-y-1/2" />
		<Input
			{...props}
			type="search"
			{value}
			{placeholder}
			autocomplete="off"
			spellcheck="false"
			onchange={handleInputChange}
			onclick={(event) => {
				props.onclick(event);
				reopenSuggestions();
			}}
			onfocus={(event) => {
				props.onfocus(event);
				reopenSuggestions();
			}}
			onkeydown={(event) => {
				props.onkeydown(event);
				if (event.defaultPrevented) return;
				if (event.key === "Escape") open = false;
			}}
			class={[
				"border-border bg-background text-foreground w-full rounded-lg border-2 py-2 pr-10 pl-9 text-sm font-semibold transition-colors outline-none",
				"placeholder:text-foreground/35 focus:border-accent/70 disabled:cursor-not-allowed disabled:opacity-50"
			]}
		/>

		{#if value}
			<button
				type="button"
				aria-label="Clear station search"
				onclick={(event) => {
					event.stopPropagation();
					clear();
				}}
				class="text-foreground/40 hover:bg-secondary hover:text-foreground focus:bg-secondary focus:text-foreground absolute top-1/2 right-2 inline-flex size-7 -translate-y-1/2 items-center justify-center rounded-md transition-colors outline-none"
			>
				<X size={15} />
			</button>
		{/if}
	</div>
{/snippet}

<div class={["grid gap-2", className]}>
	<label for={`${searchId}-trigger`} class="text-foreground/60 text-xs font-bold tracking-wider uppercase">{label}</label>

	<DropdownMenu.Root id={searchId} bind:open closeOnEscape closeOnInteractOutside class="w-full">
		<DropdownMenu.Trigger child={stationSearchTrigger} openOnFocus clickBehavior="open" class="w-full" />

		<DropdownMenu.Content matchTriggerWidth class="navigator-scrollbar max-h-80">
			{#if statusMessage}
				<div class="text-foreground/60 flex items-center gap-2 px-3 py-2 text-sm font-semibold" aria-live="polite">
					{#if loading}
						<LoaderCircle size={15} class="animate-spin" />
					{/if}
					<span>{statusMessage}</span>
				</div>
			{/if}

			{#if results.length > 0}
				<DropdownMenu.Group>
					<DropdownMenu.GroupHeading>Stations</DropdownMenu.GroupHeading>
					{#each results as station (station.evaNumber)}
						{@const transportGroups = getStationTransportGroups(station)}
						<DropdownMenu.Item
							textValue={station.name}
							onselect={() => selectStation(station)}
							class="items-start whitespace-normal"
						>
							<span class="min-w-0 max-w-full">
								<span class="block whitespace-normal break-words">{station.name}</span>
								{#if station.ril100?.length}
									<span class="text-foreground/50 mt-0.5 block whitespace-normal break-words text-xs">
										{station.ril100.join(", ")}
									</span>
								{/if}
								{#if transportGroups.length}
									<span class="mt-1.5 flex max-w-full flex-wrap gap-1">
										{#each transportGroups as group (group.key)}
											<TransportTypePill {group} />
										{/each}
									</span>
								{/if}
							</span>
						</DropdownMenu.Item>
					{/each}
				</DropdownMenu.Group>
			{/if}
		</DropdownMenu.Content>
	</DropdownMenu.Root>
</div>
