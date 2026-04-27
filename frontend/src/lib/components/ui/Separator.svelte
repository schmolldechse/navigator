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
		"bg-border shrink-0",
		{ "h-0.5 w-full": orientation === "horizontal" },
		{ "h-full w-0.5": orientation === "vertical" },
		classNames
	]}
></div>
