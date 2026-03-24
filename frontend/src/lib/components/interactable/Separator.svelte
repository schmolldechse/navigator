<script lang="ts">
	import type { ClassValue } from "svelte/elements";

	type Props = {
		orientation?: "horizontal" | "vertical";
		class?: ClassValue;
	};

	let { orientation = "horizontal", class: classNames }: Props = $props();
</script>

<div
	role="separator"
	aria-orientation={orientation}
	class={[
		"bg-muted-foreground/20 shrink-0",
		{ "h-px w-full": orientation === "horizontal" },
		{ "h-full w-px": orientation === "vertical" },
		classNames
	]}
></div>
