<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		state: boolean;
		changeable?: boolean;
		ontoggle?: (state: boolean) => void;
		children: Snippet;
		class?: ClassValue;
	};
	let { state = $bindable(false), changeable = $bindable(true), ontoggle, children, class: className }: Props = $props();
</script>

<button
	type="button"
	onclick={() => changeable && ontoggle && ontoggle(!state)}
	class={[
		"cursor-pointer rounded-lg border px-3 py-1.5 text-sm  font-semibold transition-all",
		state && "border-emerald-500/20 bg-emerald-500/10 text-emerald-500 hover:bg-emerald-500/20",
		!state && "border-muted-foreground/10 bg-muted/10 text-muted-foreground hover:bg-muted-foreground/10 opacity-75",
		className
	]}
>
	{@render children()}
</button>
