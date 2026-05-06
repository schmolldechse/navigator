<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import Card from "@lib/components/ui/card/Card.svelte";

	type Props = {
		children: Snippet;
		class?: ClassValue;
	};
	let { children, class: className }: Props = $props();
</script>

<Card class={className}>
	{@render children()}
</Card>
