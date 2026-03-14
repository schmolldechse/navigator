<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		mode?: "primary" | "secondary" | "destructive";
		disabled?: boolean;
		onclick?: (event: MouseEvent) => void;
		children: Snippet;
		class?: ClassValue;
	};

	let { mode = "primary", disabled = false, onclick, children, class: className }: Props = $props();
</script>

<button
	type="button"
	{disabled}
	{onclick}
	class={[
		"flex items-center justify-center gap-x-2 rounded-lg px-4 py-2 transition-all",
		disabled && "cursor-not-allowed opacity-50",
		!disabled && "cursor-pointer",

		// primary
		mode === "primary" && "bg-accent text-background hover:opacity-90",
		mode === "primary" && disabled && "bg-accent/50 text-background/50",

		// secondary
		mode === "secondary" && "border-muted-foreground/20 bg-muted/10 hover:bg-muted-foreground/10 border",

		// destructive
		mode === "destructive" && "bg-destructive hover:opacity-90",
		mode === "destructive" && disabled && "bg-destructive/50",

		className
	]}
>
	{@render children()}
</button>
