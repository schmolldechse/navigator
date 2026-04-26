<script lang="ts" generics="T extends ToggleItem">
	import type { Snippet } from "svelte";
	import {
		setToggleGroupContext,
		ToggleGroupContext,
		type ToggleGroupMode,
		type ToggleItem
	} from "./toggle-group-context.svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		mode?: ToggleGroupMode;
		selected?: T[];
		keyFn?: (option: T) => unknown;
		onselect?: (selected: T[]) => void;
		children?: Snippet;
		class?: ClassValue;
	};
	let {
		mode = "single",
		selected = $bindable([]),
		keyFn = (option: T) => option.id ?? option,
		onselect,
		children,
		class: className
	}: Props = $props();

	const selection = new ToggleGroupContext<T>({
		get selected() {
			return selected;
		},
		set selected(newSelected) {
			selected = newSelected;
			onselect?.(newSelected);
		},
		keyFn,
		mode
	});
	setToggleGroupContext(selection);
</script>

<div class={["flex flex-wrap gap-2", className]}>
	{@render children?.()}
</div>
