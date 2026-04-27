<script lang="ts">
	import { DateTime } from "luxon";
	import TimePicker, { type TimePickerValue } from "./TimePicker.svelte";
	import type { ClassValue } from "svelte/elements";
	import Dialog from "../dialog/Dialog.svelte";

	type Props = {
		isVisible: boolean;
		isRange?: boolean;
		min?: DateTime;
		max?: DateTime;
		value?: TimePickerValue;
		onchange?: (value: TimePickerValue) => void;
		closeOnSelect?: boolean;
		class?: ClassValue;
	};
	let {
		isVisible = $bindable(false),
		isRange = false,
		min,
		max,
		value = $bindable(undefined),
		onchange,
		closeOnSelect = false,
		class: classNames
	}: Props = $props();
</script>

<Dialog
	bind:isVisible
	showHeader={false}
	showActions={false}
	class={["w-[calc(100vw-2rem)] max-w-sm sm:w-max sm:min-w-96", classNames]}
>
	<TimePicker
		{isRange}
		{min}
		{max}
		bind:value
		onchange={(value: TimePickerValue) => {
			onchange?.(value);
			if (closeOnSelect) isVisible = false;
		}}
	/>
</Dialog>
