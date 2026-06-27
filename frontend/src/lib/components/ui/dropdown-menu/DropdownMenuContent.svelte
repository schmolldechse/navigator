<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue, HTMLAttributes } from "svelte/elements";
	import { getDropdownMenuContext, type DropdownMenuAlign } from "./dropdown-menu-context.svelte";

	type AttributeValue = string | number | boolean | null | undefined;
	type Props = {
		align?: DropdownMenuAlign;
		sideOffset?: number;
		matchTriggerWidth?: boolean;
		children?: Snippet;
		class?: ClassValue;
		[key: `data-${string}`]: AttributeValue;
	} & Omit<HTMLAttributes<HTMLDivElement>, "children" | "class" | "onkeydown">;

	let {
		align = "start",
		sideOffset = 6,
		matchTriggerWidth = false,
		children,
		class: className,
		style,
		...rest
	}: Props = $props();

	const menu = getDropdownMenuContext();
	let contentElement: HTMLDivElement | undefined = $state(undefined);
	const widthStyle = $derived(
		matchTriggerWidth ? `width: ${menu.triggerWidth > 0 ? `${menu.triggerWidth}px` : "100%"}` : undefined
	);
	const contentStyle = $derived(
		[`top: calc(100% + ${sideOffset}px)`, widthStyle, style].filter(Boolean).join("; ")
	);

	$effect(() => {
		if (!contentElement) return;

		menu.contentElement = contentElement;
		return () => {
			if (menu.contentElement === contentElement) menu.contentElement = undefined;
		};
	});
</script>

{#if menu.open}
	<div
		{...rest}
		id={menu.contentId}
		bind:this={contentElement}
		role="menu"
		tabindex="-1"
		aria-labelledby={menu.triggerId}
		data-dropdown-menu-content={true}
		data-state={menu.state}
		onkeydown={menu.handleContentKeydown}
		style={contentStyle}
		class={[
			"border-border bg-background absolute z-50 max-h-72 min-w-48 overflow-y-auto rounded-lg border-2 p-1 shadow-2xl outline-none",
			align === "start" && "left-0",
			align === "center" && "left-1/2 -translate-x-1/2",
			align === "end" && "right-0",
			className
		]}
	>
		{@render children?.()}
	</div>
{/if}
