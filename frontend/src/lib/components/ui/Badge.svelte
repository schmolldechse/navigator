<script module lang="ts">
	type BadgeTone = "neutral" | "accent" | "positive" | "negative";

	export type { BadgeTone };
</script>

<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue, HTMLAttributes } from "svelte/elements";

	type Props = {
		tone?: BadgeTone;
		children: Snippet;
		class?: ClassValue;
	} & HTMLAttributes<HTMLSpanElement>;

	let { tone = "neutral", children, class: className, ...rest }: Props = $props();
</script>

<span
	{...rest}
	data-tone={tone}
	class={[
		"inline-flex max-w-full items-center gap-x-1.5 rounded-lg border px-2.5 py-1 text-xs leading-4 font-semibold",
		tone === "neutral" && "border-border bg-background text-foreground",
		tone === "accent" && "border-accent/40 bg-accent/15 text-accent",
		tone === "positive" && "border-emerald-400/30 bg-emerald-500/10 text-emerald-500",
		tone === "negative" && "border-rose-400/30 bg-rose-500/10 text-rose-500",
		className
	]}
>
	{@render children()}
</span>
