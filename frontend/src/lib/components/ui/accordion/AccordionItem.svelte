<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import Separator from "../Separator.svelte";
	import { AccordionItemContext, getAccordionRootContext, setAccordionItemContext } from "./accordion-context.svelte";

	type AttributeValue = string | number | boolean | null | undefined;
	type DataAttributes = {
		[key: `data-${string}`]: AttributeValue;
	};

	type Props = {
		disabled?: boolean;
		value?: string;
		children?: Snippet;
		class?: ClassValue;
	} & DataAttributes;

	const componentId = $props.id();
	let { disabled = false, value = componentId, children, class: className, ...rest }: Props = $props();

	const root = getAccordionRootContext();
	if (!root) throw new Error("Accordion.Item must be used within Accordion.Root.");

	const item = new AccordionItemContext(root, {
		get value(): string {
			return value;
		},
		get disabled(): boolean {
			return disabled;
		},
		get itemId(): string {
			return `${componentId}-item`;
		},
		get triggerId(): string {
			return `${componentId}-trigger`;
		},
		get contentId(): string {
			return `${componentId}-content`;
		}
	});
	setAccordionItemContext(item);
</script>

<div
	{...rest}
	id={item.itemId}
	data-accordion-item={true}
	data-collapsed={item.collapsed ? true : undefined}
	data-disabled={item.disabled ? true : undefined}
	class={["group/accordion-item", className]}
>
	{@render children?.()}
</div>

<Separator />
