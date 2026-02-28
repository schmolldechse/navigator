<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		isVisible: boolean;
		isModal?: boolean;
		children: Snippet;
		class?: ClassValue;
		onclose?: () => void;
	};
	let { isVisible = $bindable(false), isModal = false, children, class: classNames, onclose }: Props = $props();

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
	class={["bg-background border-muted-foreground/20 z-100 rounded-lg border-2", classNames]}
>
	{@render children()}
</dialog>

<style>
	dialog {
		--dialog-enter-opacity: 0;
		--dialog-enter-scale: 0.95;
		--dialog-animation-duration: 0.15s;

		opacity: var(--dialog-enter-opacity, 1);
		transform: translate3d(0, 0, 0)
			scale3d(var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1));

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
			opacity: var(--dialog-enter-opacity, 1);
			transform: translate3d(0, 0, 0)
				scale3d(var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1), var(--dialog-enter-scale, 1));
		}
	}

	dialog::backdrop {
		background-color: rgba(0, 0, 0, 0);
		transition:
			display var(--dialog-animation-duration, 1) allow-discrete,
			overlay var(--dialog-animation-duration, 1) allow-discrete,
			background-color var(--dialog-animation-duration, 1);
	}

	dialog[open]::backdrop {
		background-color: rgba(0, 0, 0, 0.2);
	}
</style>
