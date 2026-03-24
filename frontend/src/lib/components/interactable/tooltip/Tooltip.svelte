<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import { setTooltipContext, TooltipContext } from "./tooltipcontext.svelte";

	type Props = {
		delay?: number;
		class?: ClassValue;
		children: Snippet;
	};
	let { delay = 150, class: className, children }: Props = $props();

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
