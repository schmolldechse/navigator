<script lang="ts" module>
	type ToggleStateOption = {
		id: string;
		value: string;
	};

	export { type ToggleStateOption };
</script>

<script lang="ts">
	import type { ClassValue } from "svelte/elements";
	import ToggleStateButton from "./ToggleStateButton.svelte";

	type Props = {
		options: ToggleStateOption[];
		selected: ToggleStateOption;
		onselect?: (selected: ToggleStateOption) => void;
		class?: ClassValue;
	};
	let { options, selected = $bindable(), onselect, class: className }: Props = $props();

	const defaultOption = selected;
</script>

<div class={["flex flex-wrap gap-2", className]}>
	{#each options as option (option.id)}
		<ToggleStateButton
			state={selected.id === option.id}
			changeable={selected.id !== option.id}
			ontoggle={(state) => {
				if (state) selected = option;
				else selected = defaultOption;
				onselect?.(selected);
			}}
		>
			{option.value}
		</ToggleStateButton>
	{/each}
</div>
