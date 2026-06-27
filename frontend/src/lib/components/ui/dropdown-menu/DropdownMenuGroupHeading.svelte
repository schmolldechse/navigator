<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue, HTMLAttributes } from "svelte/elements";

	type Props = {
		children?: Snippet;
		class?: ClassValue;
	} & Omit<HTMLAttributes<HTMLDivElement>, "children" | "class">;

	let { children, class: className, ...rest }: Props = $props();
</script>

<div {...rest} class={["text-foreground/55 px-2.5 py-1.5 text-xs font-bold tracking-wider uppercase", className]}>
	{@render children?.()}
</div>
