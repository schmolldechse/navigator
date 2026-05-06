<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue, MouseEventHandler } from "svelte/elements";
	import Button from "@lib/components/ui/Button.svelte";

	type Props = {
		mode?: "primary" | "secondary" | "destructive" | "tertiary";
		href?: string;
		disabled?: boolean;
		onclick?: MouseEventHandler<HTMLButtonElement> | null | undefined;
		children: Snippet;
		class?: ClassValue;
	};
	let { mode, href, disabled, onclick, children, class: className }: Props = $props();
</script>

<Button {mode} {href} {disabled} {onclick} class={className}>
	{@render children()}
</Button>
