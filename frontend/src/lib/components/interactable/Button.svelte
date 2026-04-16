<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue, MouseEventHandler } from "svelte/elements";

	type Props = {
		mode?: "primary" | "secondary" | "destructive" | "tertiary";
		href?: string;
		disabled?: boolean;
		onclick?: MouseEventHandler<HTMLButtonElement> | null | undefined;
		children: Snippet;
		class?: ClassValue;
	};

	let { mode = "primary", href, disabled = false, onclick, children, class: className }: Props = $props();
</script>

<svelte:element
	this={href ? "a" : "button"}
	type={href ? undefined : "button"}
	disabled={href ? undefined : disabled}
	href={href && !disabled ? href : undefined}
	role={href && disabled ? "link" : undefined}
	{onclick}
	class={[
		"inline-flex items-center justify-center gap-x-2 rounded-lg px-4 py-2 font-medium transition-colors duration-200",
		"enabled:cursor-pointer disabled:cursor-not-allowed disabled:opacity-50",

		mode === "primary" && "bg-accent text-accent-foreground enabled:hover:bg-accent/90 enabled:active:bg-accent/80",
		mode === "secondary" &&
			"border-border bg-secondary text-secondary-foreground enabled:hover:bg-secondary/80 enabled:active:bg-secondary/60 border",
		mode === "destructive" &&
			"bg-destructive text-destructive-foreground enabled:hover:bg-destructive/90 enabled:active:bg-destructive/80",
		mode === "tertiary" &&
			"text-foreground enabled:hover:bg-accent/15 enabled:hover:text-accent enabled:active:bg-accent/25 enabled:active:text-accent bg-transparent",

		className
	]}
>
	{@render children()}
</svelte:element>
