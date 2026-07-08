<script module lang="ts">
	type MetricOption<TValue extends string> = {
		value: TValue;
		label: string;
	};

	export type { MetricOption };
</script>

<script lang="ts" generics="TValue extends string">
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";

	type Props = {
		title?: string;
		value: TValue;
		options: MetricOption<TValue>[];
		onchange: (value: TValue) => void;
	};

	let { title = "Metric", value, options, onchange }: Props = $props();
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
		keyFn={(option: MetricOption<TValue>) => option.value}
		onselect={(option: MetricOption<TValue> | undefined) => onchange(option?.value ?? value)}
		class="flex-wrap gap-1"
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
