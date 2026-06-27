<script lang="ts">
	import type { ClassValue, HTMLAttributes } from "svelte/elements";

	type Props = {
		orientation?: "horizontal" | "vertical";
		class?: ClassValue;
	} & Omit<HTMLAttributes<HTMLDivElement>, "role" | "aria-orientation">;
	let { orientation = "horizontal", class: classNames, ...rest }: Props = $props();
</script>

<div
	{...rest}
	aria-hidden="true"
	role="separator"
	aria-orientation={orientation}
	class={[
		"bg-border shrink-0",
		orientation === "horizontal" && "h-0.5 w-full",
		orientation === "vertical" && "h-full w-0.5",
		classNames
	]}
></div>
