<script lang="ts" generics="T extends ToggleItem">
	import type { Snippet } from "svelte";
	import {
		setToggleGroupContext,
		ToggleGroupContext,
		type ToggleGroupValue,
		type ToggleItem
	} from "./toggle-group-context.svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		keyFn?: (option: T) => unknown;
		allowEmpty?: boolean;
		children?: Snippet;
		class?: ClassValue;
	} & (
		| {
				mode?: "single";
				selected?: T;
				onselect?: (selected: T | undefined) => void;
		  }
		| {
				mode: "multiple";
				selected?: T[];
				onselect?: (selected: T[]) => void;
		  }
	);
	let {
		mode = "single",
		selected = $bindable(undefined),
		keyFn = (option: T) => option.id ?? option,
		allowEmpty = true,
		onselect,
		children,
		class: className
	}: Props = $props();

	const notifySelection = (nextSelected: ToggleGroupValue<T>) => {
		if (mode === "multiple") {
			(onselect as ((selected: T[]) => void) | undefined)?.(Array.isArray(nextSelected) ? nextSelected : []);
			return;
		}

		(onselect as ((selected: T | undefined) => void) | undefined)?.(
			Array.isArray(nextSelected) ? nextSelected[0] : nextSelected
		);
	};

	const selection = new ToggleGroupContext<T>({
		get selected() {
			return selected;
		},
		set selected(newSelected) {
			selected = newSelected;
			notifySelection(newSelected);
		},
		get keyFn() {
			return keyFn;
		},
		get mode() {
			return mode;
		},
		get allowEmpty() {
			return allowEmpty;
		}
	});
	setToggleGroupContext(selection);
</script>

<div class={["flex flex-wrap gap-2", className]}>
	{@render children?.()}
</div>
