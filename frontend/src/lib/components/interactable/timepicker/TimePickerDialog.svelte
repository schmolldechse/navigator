<script lang="ts">
	import { DateTime } from "luxon";
	import TimePicker from "./TimePicker.svelte";
	import type { ClassValue } from "svelte/elements";
	import Dialog from "../dialog/Dialog.svelte";

	type TimePickerRange = { start: DateTime; end: DateTime };
	type TimePickerValue = DateTime | TimePickerRange | undefined;

	type Props = {
		isVisible: boolean;
		isRange?: boolean;
		min?: DateTime;
		max?: DateTime;
		value?: TimePickerValue;
		onchange?: (value: TimePickerValue) => void;
		closeOnSelect?: boolean;
		title?: string;
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
		title = "Select Date Range",
		class: classNames
	}: Props = $props();
</script>

<Dialog bind:isVisible {title} class={["absolute right-0 left-auto w-[400px]", classNames]}>
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
