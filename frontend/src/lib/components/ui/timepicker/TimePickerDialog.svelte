<script lang="ts">
	import TimePicker, { type TimePickerValue } from "./TimePicker.svelte";
	import type { ClassValue } from "svelte/elements";
	import Dialog from "../dialog/Dialog.svelte";
	import type { ComponentProps } from "svelte";

	type Props = {
		closeOnSelect?: boolean;
		class?: ClassValue;
		style?: string;
	} & Pick<ComponentProps<typeof Dialog>, "isVisible" | "isModal" | "clickOutsideToClose"> &
		Pick<ComponentProps<typeof TimePicker>, "isRange" | "min" | "max" | "value" | "onchange">;
	let {
		closeOnSelect = true,
		class: classNames,
		style,
		isVisible = $bindable(false),
		isModal = true,
		clickOutsideToClose = true,
		isRange = false,
		min,
		max,
		value = $bindable(undefined),
		onchange
	}: Props = $props();
</script>

<Dialog
	bind:isVisible
	{isModal}
	{clickOutsideToClose}
	showHeader={false}
	showActions={false}
	{style}
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
