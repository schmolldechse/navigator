<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		state: boolean;
		changeable?: boolean;
		disabled?: boolean;
		ontoggle?: (state: boolean) => void;
		children: Snippet;
		class?: ClassValue;
	};
	let {
		state = $bindable(false),
		changeable = $bindable(true),
		disabled = false,
		ontoggle,
		children,
		class: className
	}: Props = $props();
</script>

<button
	type="button"
	{disabled}
	onclick={() => {
		if (!changeable || disabled) return;
		state = !state;
		ontoggle?.(state);
	}}
	class={[
		"rounded-lg border px-3 py-1.5 text-sm font-semibold transition-all",
		disabled && "border-muted-foreground/10 bg-muted/10 text-muted-foreground cursor-not-allowed opacity-40",
		!disabled && !changeable && state && "cursor-default border-emerald-500/20 bg-emerald-500/10 text-emerald-500",
		!disabled &&
			changeable &&
			state &&
			"cursor-pointer border-emerald-500/20 bg-emerald-500/10 text-emerald-500 hover:bg-emerald-500/20",
		!disabled &&
			!state &&
			"border-muted-foreground/10 bg-muted/10 text-muted-foreground hover:bg-muted-foreground/10 cursor-pointer opacity-75",
		className
	]}
>
	{@render children()}
</button>
