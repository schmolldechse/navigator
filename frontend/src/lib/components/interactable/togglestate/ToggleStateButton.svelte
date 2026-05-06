<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		state: boolean;
		disabled?: boolean;
		ontoggle?: (state: boolean) => void;
		children: Snippet;
		class?: ClassValue;
	};
	let { state = $bindable(false), disabled = false, ontoggle, children, class: className }: Props = $props();
</script>

<button
	type="button"
	{disabled}
	data-active={state ? true : undefined}
	class={[
		"border-border rounded-md border px-3 py-1.5 text-sm transition-colors",
		"enabled:cursor-pointer disabled:cursor-not-allowed disabled:opacity-50",
		state && "border-accent/40 bg-accent/15 text-accent",
		!state && "bg-secondary text-secondary-foreground hover:bg-secondary/80",
		className
	]}
	onclick={() => {
		if (disabled) return;
		state = !state;
		ontoggle?.(state);
	}}
>
	{@render children()}
</button>
