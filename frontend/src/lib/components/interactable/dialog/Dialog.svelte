<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import Dialog from "@lib/components/ui/dialog/Dialog.svelte";

	type Props = {
		isVisible: boolean;
		title?: string | Snippet;
		children: Snippet;
		actions?: Snippet;
		showHeader?: boolean;
		showActions?: boolean;
		isModal?: boolean;
		clickOutsideToClose?: boolean;
		onclose?: () => void;
		class?: ClassValue;
	};
	let {
		isVisible = $bindable(false),
		title,
		children,
		actions,
		showHeader,
		showActions,
		isModal,
		clickOutsideToClose,
		onclose,
		class: className
	}: Props = $props();
</script>

<Dialog
	bind:isVisible
	{title}
	{actions}
	{showHeader}
	{showActions}
	{isModal}
	{clickOutsideToClose}
	{onclose}
	class={className}
>
	{@render children()}
</Dialog>
