<script lang="ts">
	import { StatisticsBucket } from "@lib/api";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import { statisticsBucketOptions, type StatisticsBucketOption } from "./statistics-dashboard";

	type Props = {
		value: StatisticsBucket;
		onchange: (bucket: StatisticsBucket) => void;
		options?: StatisticsBucketOption[];
	};

	let { value, onchange, options = statisticsBucketOptions }: Props = $props();
</script>

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
