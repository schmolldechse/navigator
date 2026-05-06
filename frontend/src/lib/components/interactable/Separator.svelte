<script lang="ts">
	import type { ClassValue } from "svelte/elements";
	import Separator from "@lib/components/ui/Separator.svelte";

	type Props = {
		orientation?: "horizontal" | "vertical";
		class?: ClassValue;
	};
	let { orientation, class: className }: Props = $props();
</script>

<Separator {orientation} class={className} />
