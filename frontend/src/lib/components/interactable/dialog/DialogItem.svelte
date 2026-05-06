<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import DialogItem from "@lib/components/ui/dialog/DialogItem.svelte";

	type Props = {
		title: string;
		children: Snippet;
		class?: ClassValue;
	};
	let { title, children, class: className }: Props = $props();
</script>

<DialogItem {title} class={className}>
	{@render children()}
</DialogItem>
