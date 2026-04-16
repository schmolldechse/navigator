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

	const resolvedTitle = $derived.by(() => {
		if (title) return title;
		return isRange ? "Select Time Range" : "Select Time";
	});
</script>

<Dialog bind:isVisible title={resolvedTitle} class={["w-[calc(100vw-2rem)] max-w-sm sm:w-max sm:min-w-96", classNames]}>
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
