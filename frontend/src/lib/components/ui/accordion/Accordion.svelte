<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import {
		AccordionRootContext,
		setAccordionRootContext,
		type AccordionType,
		type AccordionValue
	} from "./accordion-context.svelte";

	type AttributeValue = string | number | boolean | null | undefined;
	type DataAttributes = {
		[key: `data-${string}`]: AttributeValue;
	};

	type BaseProps = {
		disabled?: boolean;
		children?: Snippet;
		class?: ClassValue;
	} & DataAttributes;

	type Props =
		| ({
				type: "single";
				value?: string | undefined;
				onchange?: (value: string | undefined) => void;
		  } & BaseProps)
		| ({
				type: "multiple";
				value?: string[];
				onchange?: (value: string[]) => void;
		  } & BaseProps);

	let { type, value = $bindable(), onchange, disabled = false, children, class: className, ...rest }: Props = $props();

	const accordion = new AccordionRootContext({
		get type(): AccordionType {
			return type;
		},
		get value(): AccordionValue {
			return value;
		},
		set value(newValue: AccordionValue) {
			value = newValue as typeof value;
			(onchange as ((value: AccordionValue) => void) | undefined)?.(newValue);
		},
		get disabled(): boolean {
			return disabled;
		}
	});
	setAccordionRootContext(accordion);
</script>

<div {...rest} data-accordion-root={true} data-disabled={disabled ? true : undefined} class={["w-full", className]}>
	{@render children?.()}
</div>
