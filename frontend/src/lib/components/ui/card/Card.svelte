<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		children: Snippet;
		class?: ClassValue;
	};
	let { children, class: className }: Props = $props();
</script>

<div class={["border-border bg-background flex flex-col rounded-xl border-2 p-4", className]}>
	{@render children()}
</div>
