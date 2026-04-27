<script lang="ts">
	import Check from "@lucide/svelte/icons/check";

	type Props = {
		checked: boolean;
		disabled?: boolean;
	};
	let { checked = $bindable(false), disabled }: Props = $props();
</script>

<div class="relative flex h-4 w-4 items-center justify-center">
	<input
		type="checkbox"
		bind:checked
		{disabled}
		class={[
			"peer border-border bg-muted/20 h-4 w-4 cursor-pointer appearance-none rounded border-2 transition-all checked:border-emerald-500/40 checked:bg-emerald-500",
			"disabled:cursor-not-allowed disabled:opacity-50"
		]}
	/>

	<Check size={12} strokeWidth={4} class="pointer-events-none absolute hidden peer-checked:block" />
</div>
