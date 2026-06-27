<script lang="ts">
	import type { TransportTypeIconGroup } from "@lib/components/transport-types/transport-type-icons";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		group: TransportTypeIconGroup;
		class?: ClassValue;
	};

	let { group, class: className }: Props = $props();
	const Icon = $derived(group.Icon);
</script>

<span
	class={[
		"border-border/80 bg-secondary/45 text-foreground/75 inline-flex max-w-full items-center gap-1.5 rounded-md border px-1.5 py-0.5 text-[0.65rem] leading-none font-bold",
		className
	]}
	title={group.label}
>
	<Icon type="rounded-corners" class="size-4 shrink-0" />
	<span class="whitespace-normal break-words">{group.shortLabel}</span>
</span>
