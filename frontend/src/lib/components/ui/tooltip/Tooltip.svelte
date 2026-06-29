<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import { setTooltipContext, TooltipContext } from "./tooltip-context.svelte";

	type Props = {
		delay?: number;
		class?: ClassValue;
		children: Snippet;
	};
	let { delay = 150, class: className, children }: Props = $props();

	// svelte-ignore state_referenced_locally
	const context: TooltipContext = new TooltipContext(delay);
	setTooltipContext(context);
</script>

<div
	role="tooltip"
	class={["relative inline-flex", className]}
	onmouseenter={context.show}
	onmouseleave={context.hide}
	onfocusin={context.show}
	onfocusout={context.hide}
>
	{@render children()}
</div>
