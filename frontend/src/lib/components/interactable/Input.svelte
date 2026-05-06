<script lang="ts">
	import type { ClassValue } from "svelte/elements";
	import Input from "@lib/components/ui/Input.svelte";

	type Props = {
		type: "text" | "number";
		value: string | number;
		placeholder?: string;
		disabled?: boolean;
		readonly?: boolean;
		min?: number;
		max?: number;
		step?: number;
		pattern?: string;
		minlength?: number;
		maxlength?: number;
		size?: number;
		debounceTime?: number;
		onchange?: (value: string | number) => void;
		class?: ClassValue;
	};
	let {
		type,
		value = $bindable(),
		placeholder,
		disabled,
		readonly,
		min,
		max,
		step,
		pattern,
		minlength,
		maxlength,
		size,
		debounceTime,
		onchange,
		class: className
	}: Props = $props();
</script>

<Input
	{type}
	bind:value
	{placeholder}
	{disabled}
	{readonly}
	{min}
	{max}
	{step}
	{pattern}
	{minlength}
	{maxlength}
	{size}
	{debounceTime}
	{onchange}
	class={className}
/>
