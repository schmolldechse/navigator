<script lang="ts" generics="T extends ToggleItem">
	import { untrack, type Snippet } from "svelte";
	import { getToggleGroupContext, type ToggleItem } from "./toggle-group-context.svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		item: T;
		disabled?: boolean;
		onselect?: (option: T, isActive: boolean) => void;
		children: Snippet<[{ isActive: boolean; isDisabled: boolean }?]>;
		class?: ClassValue;
	};
	let { item, disabled, onselect, children, class: className }: Props = $props();

	const context = getToggleGroupContext<T>();
	if (!context) throw new Error("ToggleGroupItem must be used within a ToggleGroup.");

	$effect(() => {
		untrack(() => context.register(item));
		return () => untrack(() => context.unregister(item));
	});

	let isActive = $derived(context.isSelected(item));
</script>

<button
	type="button"
	data-active={isActive ? true : undefined}
	{disabled}
	class={[
		"border-border rounded-md border px-3 py-1.5 text-sm",
		"enabled:cursor-pointer disabled:cursor-not-allowed disabled:opacity-50",
		className
	]}
	onclick={() => {
		if (disabled) return;
		context.toggle(item);
		onselect?.(item, !isActive);
	}}
>
	{@render children?.({ isActive, isDisabled: !!disabled })}
</button>
