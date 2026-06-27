<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue, HTMLAttributes } from "svelte/elements";
	import { DropdownMenuContext, setDropdownMenuContext } from "./dropdown-menu-context.svelte";

	type AttributeValue = string | number | boolean | null | undefined;
	type Props = {
		open?: boolean;
		disabled?: boolean;
		closeOnEscape?: boolean;
		closeOnInteractOutside?: boolean;
		loop?: boolean;
		onopenchange?: (open: boolean) => void;
		children?: Snippet;
		class?: ClassValue;
		[key: `data-${string}`]: AttributeValue;
	} & Omit<HTMLAttributes<HTMLDivElement>, "children" | "class">;

	const uid = $props.id();
	let {
		id = `${uid}-dropdown-menu`,
		open = $bindable(false),
		disabled = false,
		closeOnEscape = true,
		closeOnInteractOutside = true,
		loop = true,
		onopenchange,
		children,
		class: className,
		...rest
	}: Props = $props();

	let rootElement: HTMLDivElement | undefined = $state(undefined);
	let triggerElement: HTMLElement | undefined = $state(undefined);
	let contentElement: HTMLElement | undefined = $state(undefined);

	const menu = new DropdownMenuContext({
		get open() {
			return open;
		},
		set open(value: boolean) {
			open = value;
		},
		get disabled() {
			return disabled;
		},
		get loop() {
			return loop;
		},
		get closeOnEscape() {
			return closeOnEscape;
		},
		get closeOnInteractOutside() {
			return closeOnInteractOutside;
		},
		get triggerId() {
			return `${id}-trigger`;
		},
		get contentId() {
			return `${id}-content`;
		},
		get rootElement() {
			return rootElement;
		},
		get triggerElement() {
			return triggerElement;
		},
		set triggerElement(value: HTMLElement | undefined) {
			triggerElement = value;
		},
		get contentElement() {
			return contentElement;
		},
		set contentElement(value: HTMLElement | undefined) {
			contentElement = value;
		},
		onopenchange: (value: boolean) => onopenchange?.(value)
	});
	setDropdownMenuContext(menu);
</script>

<svelte:document onpointerdown={menu.handleDocumentPointerDown} />
<svelte:window onresize={menu.updateTriggerWidth} />

<div
	{...rest}
	{id}
	bind:this={rootElement}
	data-dropdown-menu-root={true}
	data-state={menu.state}
	data-disabled={disabled ? true : undefined}
	class={["relative inline-block", className]}
>
	{@render children?.()}
</div>
