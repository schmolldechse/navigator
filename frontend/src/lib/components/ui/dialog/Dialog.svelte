<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue, HTMLDialogAttributes } from "svelte/elements";
	import X from "@lucide/svelte/icons/x";
	import Button from "../Button.svelte";

	type Props = {
		isVisible: boolean;
		title?: string | Snippet;
		children: Snippet;
		actions?: Snippet;
		showHeader?: boolean;
		showActions?: boolean;
		showCloseButton?: boolean;
		isModal?: boolean;
		clickOutsideToClose?: boolean;
		onclose?: () => void;
		class?: ClassValue;
	} & Omit<HTMLDialogAttributes, "open" | "class" | "onclose" | "title">;
	let {
		isVisible = $bindable(false),
		title,
		children,
		actions,
		showHeader = true,
		showActions = true,
		showCloseButton = true,
		isModal = true,
		clickOutsideToClose = true,
		onclose,
		class: className,
		...rest
	}: Props = $props();

	const id = $props.id();
	const titleId = `${id}-title`;

	let dialog: HTMLDialogElement | undefined = $state(undefined);

	let hasEmittedClose = $state(false);
	let suppressNextNativeClose = $state(false);

	const hasVisibleTitle = $derived(showHeader && Boolean(title));
	const ariaLabel = $derived(hasVisibleTitle ? undefined : (rest["aria-label"] ?? "Dialog"));

	$effect(() => {
		if (!dialog) return;

		if (isVisible) {
			hasEmittedClose = false;
			suppressNextNativeClose = false;

			if (!dialog.open) {
				if (isModal) dialog.showModal();
				else dialog.show();
			}

			return;
		}

		if (dialog.open) {
			suppressNextNativeClose = true;
			dialog.close();
		}
	});

	const emitClose = () => {
		if (hasEmittedClose) return;

		hasEmittedClose = true;
		onclose?.();
	};

	const requestClose = () => {
		if (!isVisible && !dialog?.open) return;

		emitClose();
		isVisible = false;

		if (dialog?.open) {
			suppressNextNativeClose = true;
			dialog.close();
		}
	};

	const handleNativeClose = () => {
		if (suppressNextNativeClose) {
			suppressNextNativeClose = false;
			return;
		}

		if (!isVisible) return;

		emitClose();
		isVisible = false;
	};

	const handleCancel = (event: Event) => {
		event.preventDefault();
		requestClose();
	};

	const pointerIsInsideDialog = (event: PointerEvent) => {
		if (!dialog) return false;

		const rect = dialog.getBoundingClientRect();
		return (
			event.clientX >= rect.left && event.clientX <= rect.right && event.clientY >= rect.top && event.clientY <= rect.bottom
		);
	};

	const handleDialogPointerDown = (event: PointerEvent) => {
		if (!dialog || !isVisible || !isModal || !clickOutsideToClose) return;
		if (event.target === dialog && !pointerIsInsideDialog(event)) requestClose();
	};

	const handleWindowPointerDown = (event: PointerEvent) => {
		if (!dialog || !isVisible || isModal || !clickOutsideToClose) return;
		if (!(event.target instanceof Node)) return;
		if (dialog.contains(event.target)) return;

		requestClose();
	};
</script>

<svelte:window onpointerdown={handleWindowPointerDown} />

<dialog
	{...rest}
	bind:this={dialog}
	onclose={handleNativeClose}
	oncancel={handleCancel}
	onpointerdown={handleDialogPointerDown}
	aria-labelledby={hasVisibleTitle ? titleId : undefined}
	aria-label={ariaLabel}
	class={[
		"bg-background text-foreground border-border z-100 max-h-[calc(100dvh-2rem)] rounded-xl border-2 p-4 shadow-2xl",
		className
	]}
>
	<div class="flex flex-col gap-y-3">
		{#if showHeader}
			<div class="flex items-start justify-between gap-x-4">
				{#if typeof title === "string"}
					<h3 id={titleId} class="text-foreground text-xl font-semibold">{title}</h3>
				{:else if title}
					<div id={titleId}>
						{@render title()}
					</div>
				{/if}

				{#if showCloseButton}
					<Button mode="tertiary" onclick={requestClose} aria-label="Close dialog" class="-mt-1 -mr-2 p-1!">
						<X size={20} />
					</Button>
				{/if}
			</div>
		{/if}

		{@render children()}

		{@render actions?.()}
	</div>
</dialog>

<style>
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
