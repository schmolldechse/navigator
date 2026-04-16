<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		title: string;
		children: Snippet;
		class?: ClassValue;
	};
	let { title, children, class: classNames }: Props = $props();
</script>

<div class={["flex flex-col gap-y-2", classNames]}>
	<h4 class="text-foreground text-sm font-medium tracking-wider uppercase">{title}</h4>
	{@render children()}
</div>
