<script lang="ts">
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import type { NetworkTransportComparisonMode } from "../network-context.svelte";

	type ComparisonModeOption = {
		value: NetworkTransportComparisonMode;
		label: string;
	};

	type Props = {
		value: NetworkTransportComparisonMode;
		onchange: (mode: NetworkTransportComparisonMode) => void;
	};

	const options: ComparisonModeOption[] = [
		{ value: "gap", label: "Punctuality gap" },
		{ value: "outcomes", label: "Planned stop outcomes" }
	];

	let { value, onchange }: Props = $props();
	const id = $props.id();
	const selected = $derived(options.find((option) => option.value === value) ?? options[0]);
</script>

<div role="group" aria-labelledby={`${id}-title`} class="flex min-w-0 flex-col items-start gap-1.5">
	<span id={`${id}-title`} class="text-foreground/50 text-[0.65rem] font-bold tracking-wider uppercase">View</span>
	<ToggleGroup
		mode="single"
		allowEmpty={false}
		{selected}
		keyFn={(option: ComparisonModeOption) => option.value}
		onselect={(option: ComparisonModeOption | undefined) => onchange(option?.value ?? value)}
		class="gap-1"
	>
		{#each options as option (option.value)}
			<ToggleGroupItem
				item={option}
				class="data-active:border-accent data-active:bg-accent data-active:text-accent-foreground px-2.5 py-1.5 text-xs font-semibold"
			>
				{option.label}
			</ToggleGroupItem>
		{/each}
	</ToggleGroup>
</div>
