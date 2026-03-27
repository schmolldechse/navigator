<script lang="ts">
	import { DateTime } from "luxon";
	import TimePicker from "./TimePicker.svelte";
	import type { ClassValue } from "svelte/elements";
	import Dialog from "../dialog/Dialog.svelte";

	type Props = {
		isVisible: boolean;
		multiSelect?: boolean;
		min?: DateTime;
		max?: DateTime;
		dates: {
			start: DateTime;
			end?: DateTime;
		};
		onchange: (params: { start: DateTime; end?: DateTime }) => void;
		title?: string;
		class?: ClassValue;
	};
	let {
		isVisible = $bindable(false),
		multiSelect = false,
		min,
		max,
		dates = $bindable(
			multiSelect ? { start: DateTime.now(), end: DateTime.now().plus({ day: 1 }) } : { start: DateTime.now() }
		),
		onchange,
		title = "Select Date Range",
		class: classNames
	}: Props = $props();
</script>

<Dialog bind:isVisible {title} class={["absolute right-0 left-auto w-[400px]", classNames]}>
	<!-- svelte-ignore block_empty -->
	{#snippet actions()}{/snippet}

	<TimePicker
		{multiSelect}
		{min}
		{max}
		bind:dates
		onchange={({ start, end }) => {
			if (!multiSelect) {
				onchange({ start, end });
				isVisible = false;
				return;
			}

			if (start && end) {
				onchange({ start, end });
				isVisible = false;
			}
		}}
	/>
</Dialog>
