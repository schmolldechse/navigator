<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	interface Props {
		head?: Snippet;
		body?: Snippet;
		class?: ClassValue;
	}

	let { head, body, class: className }: Props = $props();
</script>

<div
	class={[
		"border-muted-foreground/20 bg-muted/20 hover:border-accent/20 flex flex-col rounded-lg border-2 p-4 transition-colors duration-300",
		className
	]}
>
	{@render head?.()}
	{@render body?.()}
</div>
