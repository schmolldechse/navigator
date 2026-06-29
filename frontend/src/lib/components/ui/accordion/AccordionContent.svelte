<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import { slide } from "svelte/transition";
	import { getAccordionItemContext } from "./accordion-context.svelte";

	const TRANSITION_DURATION = 200;

	type AttributeValue = string | number | boolean | null | undefined;
	type DataAttributes = {
		[key: `data-${string}`]: AttributeValue;
	};

	type Props = {
		forceMount?: boolean;
		hiddenUntilFound?: boolean;
		children: Snippet;
		class?: ClassValue;
	} & DataAttributes;

	let { forceMount = false, hiddenUntilFound = false, children, class: className, ...rest }: Props = $props();

	const item = getAccordionItemContext();
	if (!item) throw new Error("Accordion.Content must be used within Accordion.Item.");

	let contentElement: HTMLDivElement | undefined = $state(undefined);
	let closedHidden = $derived(hiddenUntilFound ? ("until-found" as const) : forceMount ? true : undefined);

	$effect(() => {
		if (!contentElement || !hiddenUntilFound) return;

		const handleBeforeMatch = () => item.openItem();
		contentElement.addEventListener("beforematch", handleBeforeMatch);

		return () => contentElement?.removeEventListener("beforematch", handleBeforeMatch);
	});
</script>

{#if item.open}
	<div
		{...rest}
		bind:this={contentElement}
		id={item.contentId}
		role="region"
		aria-labelledby={item.triggerId}
		data-accordion-content={true}
		data-collapsed={item.collapsed ? true : undefined}
		data-disabled={item.disabled ? true : undefined}
		class={["text-foreground/75 overflow-hidden px-3 text-sm leading-6", className]}
		transition:slide={{ duration: TRANSITION_DURATION }}
	>
		<div class="pb-4">
			{@render children()}
		</div>
	</div>
{:else if forceMount || hiddenUntilFound}
	<div
		{...rest}
		bind:this={contentElement}
		id={item.contentId}
		role="region"
		aria-labelledby={item.triggerId}
		aria-hidden={hiddenUntilFound ? undefined : true}
		inert={hiddenUntilFound ? undefined : true}
		hidden={closedHidden}
		data-accordion-content={true}
		data-collapsed={item.collapsed ? true : undefined}
		data-disabled={item.disabled ? true : undefined}
		class={["text-foreground/75 overflow-hidden px-3 text-sm leading-6", className]}
	>
		<div class="pb-4">
			{@render children()}
		</div>
	</div>
{/if}
