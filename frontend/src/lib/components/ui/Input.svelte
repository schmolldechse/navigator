<script lang="ts">
	import { onDestroy } from "svelte";
	import type { ClassValue } from "svelte/elements";

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
		class: classNames
	}: Props = $props();

	let input: HTMLInputElement | undefined = $state(undefined);

	let debounceTimer: ReturnType<typeof setTimeout> | undefined = $state(undefined);

	const clamp = (val: number): number => {
		if (min != null && val < min) return min;
		if (max != null && val > max) return max;
		return val;
	};

	const applyValue = (newValue: string | number) => {
		if (type === "number" && typeof newValue === "number" && !Number.isNaN(newValue)) {
			newValue = clamp(newValue);
		}
		value = newValue;
		onchange?.(newValue);
	};

	const handleInput = (event: Event) => {
		const target = event.target as HTMLInputElement;
		const newValue = type === "number" ? target.valueAsNumber : target.value;

		if (debounceTimer) clearTimeout(debounceTimer);

		if (debounceTime) debounceTimer = setTimeout(() => applyValue(newValue), debounceTime);
		else applyValue(newValue);
	};

	onDestroy(() => debounceTimer && clearTimeout(debounceTimer));
</script>

<input
	bind:this={input}
	{type}
	{value}
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
	oninput={handleInput}
	class={[
		"border-border rounded-lg border-2 px-3 py-1.5 text-sm font-semibold transition-all outline-none",
		"disabled:cursor-not-allowed disabled:opacity-50",
		classNames
	]}
/>
