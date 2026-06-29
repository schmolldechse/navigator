<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		title: string;
		class?: ClassValue;
		children?: Snippet;
	};
	let { title, class: className, children }: Props = $props();
</script>

<div class={["flex items-center", className]}>
	<p class="text-foreground/60 text-base font-semibold tracking-wider uppercase">{title}</p>
	{@render children?.()}
</div>
