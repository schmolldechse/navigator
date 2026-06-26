<script module lang="ts">
	type MetricThreshold = "under6" | "under15";

	export type { MetricThreshold };
</script>

<script lang="ts">
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";

	type ThresholdOption = {
		value: MetricThreshold;
		label: string;
	};

	type Props = {
		title?: string;
		value: MetricThreshold;
		onchange: (threshold: MetricThreshold) => void;
	};

	const options: ThresholdOption[] = [
		{ value: "under6", label: "< 6 min" },
		{ value: "under15", label: "< 15 min" }
	];

	let { title = "Threshold", value, onchange }: Props = $props();
	const id = $props.id();
	const selected = $derived(options.find((option) => option.value === value) ?? options[0]);
</script>

<div role="group" aria-labelledby={`${id}-title`} class="flex min-w-0 flex-col items-start gap-1.5">
	<span id={`${id}-title`} class="text-foreground/50 text-[0.65rem] font-bold tracking-wider uppercase">
		{title}
	</span>
	<ToggleGroup
		mode="single"
		allowEmpty={false}
		{selected}
		keyFn={(option: ThresholdOption) => option.value}
		onselect={(option: ThresholdOption | undefined) => onchange(option?.value ?? value)}
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
