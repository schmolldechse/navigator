<script module lang="ts">
	export type ToggleStateOption = {
		id: string;
		value: string;
	};
</script>

<script lang="ts">
	import type { ClassValue } from "svelte/elements";
	import ToggleStateButton from "./ToggleStateButton.svelte";

	type Props = {
		options: ToggleStateOption[];
		selected: ToggleStateOption;
		onselect?: (option: ToggleStateOption) => void;
		class?: ClassValue;
	};
	let { options, selected = $bindable(), onselect, class: className }: Props = $props();
</script>

<div class={["flex flex-wrap gap-2", className]}>
	{#each options as option (option.id)}
		<ToggleStateButton
			state={selected?.id === option.id}
			ontoggle={() => {
				selected = option;
				onselect?.(option);
			}}
		>
			{option.value}
		</ToggleStateButton>
	{/each}
</div>
