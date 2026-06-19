<script module lang="ts">
	type MetricPerspective = "customer" | "operative";

	export type { MetricPerspective };
</script>

<script lang="ts">
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";

	type PerspectiveOption = {
		value: MetricPerspective;
		label: string;
	};

	type Props = {
		title?: string;
		value: MetricPerspective;
		onchange: (perspective: MetricPerspective) => void;
	};

	const options: PerspectiveOption[] = [
		{ value: "customer", label: "Customer" },
		{ value: "operative", label: "Operative" }
	];

	let { title = "Perspective", value, onchange }: Props = $props();
	const id = $props.id();
</script>

<div role="group" aria-labelledby={`${id}-title`} class="flex min-w-0 flex-col items-start gap-1.5">
	<span id={`${id}-title`} class="text-foreground/50 text-[0.65rem] font-bold tracking-wider uppercase">
		{title}
	</span>
	<ToggleGroup
		mode="single"
		allowEmpty={false}
		selected={options.find((option) => option.value === value)}
		keyFn={(option: PerspectiveOption) => option.value}
		onselect={(option: PerspectiveOption | undefined) => onchange(option?.value ?? "customer")}
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
