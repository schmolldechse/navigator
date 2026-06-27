<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue, HTMLAttributes } from "svelte/elements";

	type Props = {
		children?: Snippet;
		class?: ClassValue;
	} & Omit<HTMLAttributes<HTMLDivElement>, "children" | "class" | "role">;

	let { children, class: className, ...rest }: Props = $props();
</script>

<div {...rest} role="group" class={["grid gap-1", className]}>
	{@render children?.()}
</div>
