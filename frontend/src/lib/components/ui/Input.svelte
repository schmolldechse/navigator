<script module lang="ts">
	type InputValue = string | number;

	export { type InputValue };
</script>

<script lang="ts">
	import { onDestroy } from "svelte";
	import type { ClassValue, HTMLInputAttributes } from "svelte/elements";

	type Props = {
		type: "text" | "number" | "search";
		value: InputValue;
		debounceTime?: number;
		onchange?: (value: InputValue) => void;
		class?: ClassValue;
	} & Omit<HTMLInputAttributes, "type" | "value" | "oninput" | "onchange">;

	let { type = "text", value = $bindable(), debounceTime = 0, onchange, class: classNames, ...rest }: Props = $props();

	let input: HTMLInputElement | undefined = $state(undefined);
	let debounceTimer: ReturnType<typeof setTimeout> | undefined = $state(undefined);

	const toNumber = (value: unknown): number | undefined => {
		if (value == null || value === "") return undefined;

		const numberValue = Number(value);
		return Number.isFinite(numberValue) ? numberValue : undefined;
	};

	const clamp = (val: number): number => {
		const min = toNumber(rest.min);
		const max = toNumber(rest.max);

		if (min != null && val < min) return min;
		if (max != null && val > max) return max;

		return val;
	};

	const getInputValue = (target: HTMLInputElement): InputValue => {
		if (type === "text" || type === "search") return target.value;
		if (target.value === "") return "";

		const numberValue = target.valueAsNumber;
		if (Number.isNaN(numberValue)) return target.value;

		return clamp(numberValue);
	};

	const applyValue = (newValue: InputValue) => {
		onchange?.(newValue);
	};

	const handleInput = (event: Event) => {
		const target = event.target as HTMLInputElement;
		const newValue = getInputValue(target);

		if (debounceTimer) clearTimeout(debounceTimer);

		if (debounceTime > 0) debounceTimer = setTimeout(() => applyValue(newValue), debounceTime);
		else applyValue(newValue);
	};

	onDestroy(() => debounceTimer && clearTimeout(debounceTimer));
</script>

<input
	bind:this={input}
	{...rest}
	{type}
	{value}
	oninput={handleInput}
	class={[
		"border-border rounded-lg border-2 px-3 py-1.5 text-sm font-semibold transition-all outline-none",
		"disabled:cursor-not-allowed disabled:opacity-50",
		classNames
	]}
/>

<style>
	input[type="search"]::-webkit-search-cancel-button,
	input[type="search"]::-webkit-search-decoration {
		appearance: none;
	}
</style>
