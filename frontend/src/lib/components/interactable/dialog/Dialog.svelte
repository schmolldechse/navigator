<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import Button from "../Button.svelte";
	import X from "@lucide/svelte/icons/x";

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
		children,
		title,
		actions,
		showHeader = true,
		showActions = true,
		isModal = true,
		clickOutsideToClose = true,
		onclose,
		class: classNames
	}: Props = $props();

	let dialog: HTMLDialogElement | undefined = $state(undefined);

	$effect(() => {
		if (!dialog) return;

		if (isVisible) isModal ? dialog.showModal() : dialog.show();
		else dialog.close();
	});

	const handleClose = () => {
		if (!isVisible) return;
		isVisible = false;
		onclose?.();
	};

	const handleWindowClick = (event: MouseEvent) => {
		if (!dialog || !isVisible) return;
		if (!clickOutsideToClose) return;
		if (!(event.target instanceof Node) || dialog.contains(event.target as Node)) return;
		handleClose();
	};

	const handleDialogClick = (event: MouseEvent) => {
		if (event.target === dialog) handleClose();
	};
</script>

<svelte:window onclick={handleWindowClick} />

<dialog
	bind:this={dialog}
	onclose={handleClose}
	onclick={handleDialogClick}
	class={["bg-background border-border z-100 rounded-xl border-2 shadow-2xl", classNames]}
>
	<div class="flex flex-col gap-y-2 p-4">
		{#if showHeader}
			<div class="mb-2 flex items-center justify-between gap-x-4">
				{#if typeof title === "string"}
					<h3 class="text-foreground text-xl font-semibold">{title}</h3>
				{:else}
					{@render title?.()}
				{/if}

				<Button mode="tertiary" onclick={handleClose} class="-mr-2 p-1!">
					<X size={20} />
				</Button>
			</div>
		{/if}

		{@render children()}

		{#if showActions}
			{#if typeof actions === "function"}
				{@render actions()}
			{:else}
				<Button mode="primary" onclick={handleClose} class="ml-auto font-semibold">Done</Button>
			{/if}
		{/if}
	</div>
</dialog>

<style>
	/* Scope the variables to a wildcard or explicitly duplicate them so both dialog and backdrop can read them */
	dialog,
	dialog::backdrop {
		--dialog-enter-opacity: 0;
		--dialog-enter-scale: 0.95;
		--dialog-animation-duration: 0.15s;
	}

	dialog {
		opacity: var(--dialog-enter-opacity);
		transform: translate3d(0, 0, 0) scale3d(var(--dialog-enter-scale), var(--dialog-enter-scale), var(--dialog-enter-scale));

		transition:
			opacity var(--dialog-animation-duration) ease-out,
			transform var(--dialog-animation-duration) ease-out,
			display var(--dialog-animation-duration) ease-out allow-discrete,
			overlay var(--dialog-animation-duration) ease-out allow-discrete;
	}

	dialog[open] {
		opacity: 1;
		transform: translate3d(0, 0, 0) scale3d(1, 1, 1);
	}

	@starting-style {
		dialog[open] {
			opacity: var(--dialog-enter-opacity);
			transform: translate3d(0, 0, 0) scale3d(var(--dialog-enter-scale), var(--dialog-enter-scale), var(--dialog-enter-scale));
		}
	}

	dialog::backdrop {
		background-color: rgba(0, 0, 0, 0);
		backdrop-filter: blur(0px);
		transition:
			display var(--dialog-animation-duration) ease-out allow-discrete,
			overlay var(--dialog-animation-duration) ease-out allow-discrete,
			background-color var(--dialog-animation-duration) ease-out,
			backdrop-filter var(--dialog-animation-duration) ease-out;
	}

	dialog[open]::backdrop {
		background-color: rgba(0, 0, 0, 0.5);
		backdrop-filter: blur(4px);
	}

	@starting-style {
		dialog[open]::backdrop {
			background-color: rgba(0, 0, 0, 0);
			backdrop-filter: blur(0px);
		}
	}
</style>
