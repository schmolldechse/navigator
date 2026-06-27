<script lang="ts">
	import type { ClassValue, HTMLAttributes } from "svelte/elements";
	import Separator from "../Separator.svelte";

	type Props = {
		class?: ClassValue;
	} & Omit<HTMLAttributes<HTMLDivElement>, "role">;

	let { class: className, ...rest }: Props = $props();
</script>

<Separator {...rest} orientation="horizontal" class={["my-1", className]} />
