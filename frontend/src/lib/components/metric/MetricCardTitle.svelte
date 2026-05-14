<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import CardHeader from "@lib/components/ui/card/CardHeader.svelte";

	type Props = {
		title: string;
		class?: ClassValue;
		children?: Snippet;
	};
	let { title, class: className, children }: Props = $props();
</script>

<CardHeader {title} class={className}>
	{@render children?.()}
</CardHeader>
