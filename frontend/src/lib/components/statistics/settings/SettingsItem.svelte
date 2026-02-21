<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		title: string;
		content: Snippet;
		class?: ClassValue;
	};
	let { title, content, class: classNames }: Props = $props();
</script>

<div class={["flex flex-col gap-y-2", classNames]}>
	<h4 class="text-muted-foreground text-sm font-medium tracking-wider uppercase">{title}</h4>
	{@render content()}
</div>
