<script module lang="ts">
	import type { Snippet } from "svelte";

	type DropdownMenuTriggerChildProps = {
		props: {
			id: string;
			"aria-controls": string;
			"aria-expanded": boolean;
			"aria-haspopup": "menu";
			"data-state": "open" | "closed";
			"data-disabled"?: true;
			disabled?: boolean;
			onclick: (event: MouseEvent) => void;
			onfocus: (event: FocusEvent) => void;
			onkeydown: (event: KeyboardEvent) => void;
		};
		open: boolean;
		disabled: boolean;
	};

	export type { DropdownMenuTriggerChildProps };
</script>

<script lang="ts">
	import type { ClassValue, HTMLButtonAttributes } from "svelte/elements";
	import { getDropdownMenuContext } from "./dropdown-menu-context.svelte";

	type AttributeValue = string | number | boolean | null | undefined;
	type Props = {
		disabled?: boolean;
		openOnFocus?: boolean;
		clickBehavior?: "toggle" | "open";
		child?: Snippet<[DropdownMenuTriggerChildProps]>;
		children?: Snippet;
		class?: ClassValue;
		[key: `data-${string}`]: AttributeValue;
	} & Omit<
		HTMLButtonAttributes,
		| "aria-controls"
		| "aria-expanded"
		| "aria-haspopup"
		| "children"
		| "class"
		| "data-state"
		| "disabled"
		| "onclick"
		| "onfocus"
		| "onkeydown"
	>;

	let {
		type = "button",
		disabled = false,
		openOnFocus = false,
		clickBehavior = "toggle",
		child,
		children,
		class: className,
		...rest
	}: Props = $props();

	const menu = getDropdownMenuContext();
	let triggerElement: HTMLElement | undefined = $state(undefined);
	const isDisabled = $derived(menu.disabled || disabled);

	const handleClick = (event: MouseEvent) => {
		if (isDisabled) {
			event.preventDefault();
			return;
		}

		if (clickBehavior === "open") menu.openMenu();
		else menu.toggleMenu();
	};

	const handleFocus = () => {
		if (isDisabled || !openOnFocus) return;
		menu.openMenu();
	};

	const handleKeydown = (event: KeyboardEvent) => menu.handleTriggerKeydown(event);

	const childProps = $derived({
		id: menu.triggerId,
		"aria-controls": menu.contentId,
		"aria-expanded": menu.open,
		"aria-haspopup": "menu" as const,
		"data-state": menu.state,
		"data-disabled": isDisabled ? (true as const) : undefined,
		disabled: isDisabled ? true : undefined,
		onclick: handleClick,
		onfocus: handleFocus,
		onkeydown: handleKeydown
	});

	$effect(() => {
		if (!triggerElement) return;

		menu.triggerElement = triggerElement;
		return () => {
			if (menu.triggerElement === triggerElement) menu.triggerElement = undefined;
		};
	});
</script>

{#if child}
	<span
		bind:this={triggerElement}
		data-dropdown-menu-trigger={true}
		data-state={menu.state}
		data-disabled={isDisabled ? true : undefined}
		class={["inline-block", className]}
	>
		{@render child({ props: childProps, open: menu.open, disabled: isDisabled })}
	</span>
{:else}
	<button
		{...rest}
		id={menu.triggerId}
		bind:this={triggerElement}
		{type}
		disabled={isDisabled}
		aria-controls={menu.contentId}
		aria-expanded={menu.open}
		aria-haspopup="menu"
		data-dropdown-menu-trigger={true}
		data-state={menu.state}
		data-disabled={isDisabled ? true : undefined}
		onclick={handleClick}
		onfocus={handleFocus}
		onkeydown={handleKeydown}
		class={[
			"rounded-lg px-3 py-1.5 font-medium transition-colors duration-200",
			"enabled:cursor-pointer disabled:cursor-not-allowed disabled:opacity-50",
			"border-border bg-secondary text-secondary-foreground enabled:hover:border-accent/40 enabled:hover:bg-accent/15 enabled:hover:text-accent enabled:active:border-accent/50 enabled:active:bg-accent/25 enabled:active:text-accent border",
			className
		]}
	>
		{@render children?.()}
	</button>
{/if}
