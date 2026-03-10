<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	interface Props {
		title: string;
		class?: ClassValue;
		children?: Snippet;
	}

	let { title, class: className, children }: Props = $props();
</script>

<div class={["flex items-center", className]}>
	<p class="text-muted-foreground text-base font-semibold tracking-wider uppercase">{title}</p>
	{@render children?.()}
</div>
