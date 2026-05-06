<script lang="ts">
	import { DateTime } from "luxon";
	import type { ClassValue } from "svelte/elements";
	import TimePicker, { type TimePickerRange, type TimePickerValue } from "@lib/components/ui/timepicker/TimePicker.svelte";

	type Props = {
		multiSelect?: boolean;
		dates?: { start: DateTime; end: DateTime };
		isRange?: boolean;
		min?: DateTime;
		max?: DateTime;
		value?: TimePickerValue;
		onchange?: (value: TimePickerValue) => void;
		class?: ClassValue;
	};
	let {
		multiSelect = false,
		dates = $bindable(),
		isRange,
		min,
		max,
		value = $bindable(),
		onchange,
		class: className
	}: Props = $props();

	const rangeMode = $derived(isRange ?? multiSelect);
	const pickerValue = $derived.by(() => (rangeMode ? dates : value));
</script>

<TimePicker
	isRange={rangeMode}
	{min}
	{max}
	value={pickerValue}
	onchange={(nextValue: TimePickerValue) => {
		if (rangeMode) {
			const range = nextValue as TimePickerRange | undefined;
			if (range?.start && range?.end) dates = { start: range.start, end: range.end };
		} else {
			value = nextValue;
		}
		onchange?.(nextValue);
	}}
	class={className}
/>
