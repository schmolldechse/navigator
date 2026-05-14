<script lang="ts">
	import type { TransportType } from "@lib/api";
	import type { Component } from "svelte";
	import ToggleStateButton from "@lib/components/interactable/togglestate/ToggleStateButton.svelte";

	type TransportFilterOption = {
		id: string;
		icon?: Component<{ class?: string }>;
		label: string;
		transportTypes: TransportType[];
	};

	type Props = {
		options: TransportFilterOption[];
		enabled?: TransportType[];
		includeTotalFilter?: boolean;
		onchange: (types: TransportType[]) => void;
	};
	let { options, enabled = [], includeTotalFilter = false, onchange }: Props = $props();

	const allTransportTypes = (): TransportType[] => [
		...new Set(options.flatMap((option: TransportFilterOption) => option.transportTypes))
	];

	// svelte-ignore state_referenced_locally
	let selectedTypes: TransportType[] = $state(
		enabled.length > 0
			? enabled
			: includeTotalFilter
				? allTransportTypes()
				: options.length > 0
					? [...options[0].transportTypes]
					: []
	);

	const isTotalActive = $derived(
		includeTotalFilter &&
			selectedTypes.length > 0 &&
			allTransportTypes().every((type: TransportType) => selectedTypes.includes(type))
	);

	const isOptionActive = (option: TransportFilterOption): boolean => {
		if (isTotalActive) return false;
		if (option.transportTypes.length === 0) return false;
		return option.transportTypes.every((type: TransportType) => selectedTypes.includes(type));
	};

	const selectTotal = () => {
		if (isTotalActive) return;

		selectedTypes = allTransportTypes();
		onchange(selectedTypes);
	};

	const toggleOption = (option: TransportFilterOption) => {
		let nextSelected: TransportType[] = [];

		if (isTotalActive) {
			// "Include All" is active → select only this option
			nextSelected = [...option.transportTypes];
		} else if (isOptionActive(option)) {
			// deselect this option
			const remaining = selectedTypes.filter((type: TransportType) => !option.transportTypes.includes(type));

			// fall back if nothing remains
			if (remaining.length > 0) nextSelected = remaining;
			else if (includeTotalFilter) nextSelected = allTransportTypes();
			else if (options.length > 0) nextSelected = [...options[0].transportTypes];
			else nextSelected = [];
		} else {
			// add this option
			nextSelected = [...selectedTypes, ...option.transportTypes];
		}

		selectedTypes = nextSelected;
		onchange(selectedTypes);
	};
</script>

<div class="flex flex-wrap gap-2">
	{#if includeTotalFilter}
		<ToggleStateButton state={isTotalActive} ontoggle={selectTotal}>Include All</ToggleStateButton>
	{/if}

	{#each options as option (option.id)}
		{@const active = isOptionActive(option)}
		{@const Icon = option.icon}

		<ToggleStateButton state={active} ontoggle={() => toggleOption(option)} class="flex items-center gap-x-2">
			{#if Icon}
				<Icon class="h-4 w-4" />
			{/if}
			{option.label}
		</ToggleStateButton>
	{/each}
</div>
