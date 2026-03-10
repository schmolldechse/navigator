<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		isVisible: boolean;
		isModal?: boolean;
		title: string | Snippet;
		children: Snippet;
		class?: ClassValue;
		onclose?: () => void;
	};
	let { isVisible = $bindable(false), isModal = false, title, children, class: classNames, onclose }: Props = $props();

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
	<div class="flex flex-col space-y-3 p-2">
		{#if typeof title === "string"}
			<h3 class="text-muted-foreground text-xl font-semibold">{title}</h3>
		{:else}
			{@render title()}
		{/if}

		{@render children()}

		<!-- Actions -->
		<button
			class="bg-accent text-background hover:bg-accent/90 cursor-pointer self-end rounded-lg px-4 py-1.5 font-semibold transition-colors"
			onclick={handleClose}
		>
			Done
		</button>
	</div>
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
