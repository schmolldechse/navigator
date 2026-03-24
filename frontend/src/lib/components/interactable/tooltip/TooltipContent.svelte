<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import { getTooltipContext, TooltipContext } from "./tooltipcontext.svelte";

	type Props = {
		position?: "top" | "bottom" | "left" | "right";
		class?: ClassValue;
		children: Snippet;
	};
	let { position = "top", class: className, children }: Props = $props();

	const context: TooltipContext = getTooltipContext();
</script>

{#if context.contentVisible}
	<div
		class={[
			"bg-background border-muted-foreground/20 absolute z-100 w-max rounded-lg border-2 p-2 shadow-2xl backdrop-blur-lg",
			{ "bottom-[calc(100%+8px)] left-1/2 origin-bottom -translate-x-1/2": position === "top" },
			{ "top-[calc(100%+8px)] left-1/2 origin-top -translate-x-1/2": position === "bottom" },
			{ "top-1/2 right-[calc(100%+8px)] origin-right -translate-y-1/2": position === "left" },
			{ "top-1/2 left-[calc(100%+8px)] origin-left -translate-y-1/2": position === "right" },
			className
		]}
	>
		{@render children()}
	</div>
{/if}
