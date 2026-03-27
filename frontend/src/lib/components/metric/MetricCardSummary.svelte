<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		children: Snippet;
		class?: ClassValue;
	};
	let { children, class: className }: Props = $props();
</script>

<div class={["flex flex-row gap-x-4", className]}>
	{@render children()}
</div>
