<script lang="ts">
	import ChevronDown from "@lucide/svelte/icons/chevron-down";
	import type { Snippet } from "svelte";
	import Button from "../Button.svelte";
	import { getAccordionItemContext } from "./accordion-context.svelte";

	type AttributeValue = string | number | boolean | null | undefined;
	type DataAttributes = {
		[key: `data-${string}`]: AttributeValue;
	};

	type Props = {
		children: Snippet;
	} & DataAttributes;

	let { children, ...rest }: Props = $props();

	const item = getAccordionItemContext();
	if (!item) throw new Error("Accordion.Trigger must be used within Accordion.Item.");
</script>

<Button
	{...rest}
	mode="tertiary"
	id={item.triggerId}
	disabled={item.disabled}
	onclick={item.toggle}
	aria-expanded={item.open}
	aria-controls={item.contentId}
	data-accordion-trigger={true}
	data-collapsed={item.collapsed ? true : undefined}
	data-disabled={item.disabled ? true : undefined}
	class="group flex w-full items-center justify-between gap-3 bg-transparent! px-3! py-4! hover:bg-transparent! focus:bg-transparent! focus-visible:bg-transparent! active:bg-transparent! enabled:hover:bg-transparent! enabled:active:bg-transparent!"
>
	<span class="text-left">{@render children()}</span>
	<span
		class={[
			"inline-flex size-8 shrink-0 items-center justify-center rounded-lg transition-colors duration-200",
			!item.disabled &&
				"group-hover:bg-accent/15 group-hover:text-accent group-focus:bg-accent/15 group-focus:text-accent group-focus-visible:bg-accent/15 group-focus-visible:text-accent group-active:bg-accent/25 group-active:text-accent"
		]}
	>
		<ChevronDown
			size={18}
			aria-hidden="true"
			class={["transition-transform duration-200 ease-out", !item.collapsed && "rotate-180"]}
		/>
	</span>
</Button>
