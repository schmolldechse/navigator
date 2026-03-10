<script lang="ts">
	import { DateTime } from "luxon";
	import TimePicker from "./TimePicker.svelte";
	import type { ClassValue } from "svelte/elements";

	interface Props {
		isVisible: boolean;
		multiSelect?: boolean;
		min?: DateTime;
		max?: DateTime;
		dates: {
			start: DateTime;
			end?: DateTime;
		};
		onchange: (params: { start: DateTime; end?: DateTime }) => void;
		class?: ClassValue;
	}

	let { isVisible = $bindable(false), multiSelect = false, min, max, dates, onchange, class: classNames }: Props = $props();

	let dialog: HTMLDialogElement | undefined = $state(undefined);

	$effect(() => {
		if (!dialog) return;
		isVisible ? dialog.show() : dialog.close();
	});

	const handleClose = () => {
		if (!isVisible) return;
		isVisible = false;
		onchange({ start: dates.start, end: dates.end ?? undefined });
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
	class={[
		"bg-background border-muted-foreground/20 absolute right-0 left-auto z-100 w-[400px] rounded-lg border-2 p-1.5",
		classNames
	]}
>
	<TimePicker
		{multiSelect}
		{min}
		{max}
		bind:dates
		onchange={({ start, end }) => {
			if (!multiSelect || (multiSelect && start && end)) handleClose();
		}}
	/>
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
