<script lang="ts">
	import { StatisticsBucket } from "@lib/api";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import { statisticsBucketOptions, type StatisticsBucketOption } from "../statistics-dashboard";

	type Props = {
		title?: string;
		value: StatisticsBucket;
		onchange: (bucket: StatisticsBucket) => void;
		options?: StatisticsBucketOption[];
	};

	let { title = "Interval", value, onchange, options = statisticsBucketOptions }: Props = $props();
	const id = $props.id();
</script>

<div role="group" aria-labelledby={`${id}-title`} class="flex min-w-0 flex-col items-start gap-1.5">
	<span id={`${id}-title`} class="text-foreground/50 text-[0.65rem] font-bold tracking-wider uppercase">
		{title}
	</span>
	<ToggleGroup
		mode="single"
		allowEmpty={false}
		selected={options.find((option) => option.value === value) ?? options[0] ?? statisticsBucketOptions[0]}
		keyFn={(option: StatisticsBucketOption) => option.value}
		onselect={(option: StatisticsBucketOption | undefined) => onchange(option?.value ?? StatisticsBucket.DAY)}
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
