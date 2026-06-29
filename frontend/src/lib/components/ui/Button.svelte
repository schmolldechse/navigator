<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue, MouseEventHandler } from "svelte/elements";

	type AttributeValue = string | number | boolean | null | undefined;
	type Props = {
		mode?: "primary" | "secondary" | "destructive" | "tertiary";
		href?: string;
		id?: string;
		type?: HTMLButtonElement["type"];
		disabled?: boolean;
		onclick?: MouseEventHandler<HTMLButtonElement> | null | undefined;
		children: Snippet;
		class?: ClassValue;
		[key: `aria-${string}`]: AttributeValue;
		[key: `data-${string}`]: AttributeValue;
	};
	let {
		mode = "primary",
		href,
		id,
		type = "button",
		disabled = false,
		onclick,
		children,
		class: className,
		...rest
	}: Props = $props();
</script>

<svelte:element
	this={href ? "a" : "button"}
	{...rest}
	{id}
	type={href ? undefined : type}
	disabled={href ? undefined : disabled}
	href={href && !disabled ? href : undefined}
	role={href && disabled ? "link" : undefined}
	{onclick}
	class={[
		"rounded-lg px-3 py-1.5 font-medium transition-colors duration-200",
		"enabled:cursor-pointer disabled:cursor-not-allowed disabled:opacity-50",

		mode === "primary" && "bg-accent text-accent-foreground enabled:hover:bg-accent/90 enabled:active:bg-accent/80",
		mode === "secondary" &&
			"border-border bg-secondary text-secondary-foreground enabled:hover:border-accent/40 enabled:hover:bg-accent/15 enabled:hover:text-accent enabled:active:border-accent/50 enabled:active:bg-accent/25 enabled:active:text-accent border",
		mode === "destructive" &&
			"bg-destructive text-destructive-foreground enabled:hover:bg-destructive/90 enabled:active:bg-destructive/80",
		mode === "tertiary" &&
			"text-foreground enabled:hover:bg-accent/15 enabled:hover:text-accent enabled:active:bg-accent/25 enabled:active:text-accent bg-transparent",

		className
	]}
>
	{@render children()}
</svelte:element>
