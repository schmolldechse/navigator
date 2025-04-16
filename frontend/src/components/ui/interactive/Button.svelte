<script lang="ts">
	import type { Snippet } from "svelte";

	let { class: className = "", onclick, children }: {
		class?: string;
		onclick?: () => void;
		children: Snippet;
	} = $props();
</script>

<button
	class={["bg-accent hover:bg-accent/90 cursor-pointer transition-colors", className]}
	{onclick}
>
	{@render children?.()}
</button>