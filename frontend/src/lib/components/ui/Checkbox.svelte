<script lang="ts">
	import Check from "@lucide/svelte/icons/check";
	import Minus from "@lucide/svelte/icons/minus";
	import type { ClassValue } from "svelte/elements";

	type AttributeValue = string | number | boolean | null | undefined;

	type Props = {
		id?: string;
		name?: string;
		value?: string;
		checked?: boolean;
		onchecked?: (checked: boolean) => void;
		indeterminate?: boolean;
		onindeterminate?: (indeterminate: boolean) => void;
		disabled?: boolean;
		required?: boolean;
		class?: ClassValue;
		[key: `aria-${string}`]: AttributeValue;
		[key: `data-${string}`]: AttributeValue;
	};
	let {
		id,
		name,
		value,
		checked = $bindable(false),
		onchecked,
		indeterminate = $bindable(false),
		onindeterminate,
		disabled = false,
		required = false,
		class: className,
		...rest
	}: Props = $props();

	let input: HTMLInputElement | undefined = $state(undefined);
	let checkboxState = $derived(indeterminate ? "indeterminate" : checked ? "checked" : "unchecked");

	$effect(() => {
		if (!input) return;
		input.indeterminate = indeterminate;
	});

	const handleChange = (event: Event) => {
		const target = event.currentTarget as HTMLInputElement;

		checked = target.checked;
		indeterminate = false;
		onchecked?.(checked);
		onindeterminate?.(indeterminate);
	};
</script>

<div
	data-state={checkboxState}
	data-disabled={disabled ? true : undefined}
	class={["relative flex size-4 shrink-0 items-center justify-center", className]}
>
	<input
		{...rest}
		bind:this={input}
		type="checkbox"
		{id}
		{name}
		{value}
		bind:checked
		{disabled}
		{required}
		aria-checked={checkboxState === "indeterminate" ? "mixed" : checked}
		data-state={checkboxState}
		data-disabled={disabled ? true : undefined}
		onchange={handleChange}
		class={[
			"text-accent-foreground size-4 appearance-none rounded border-2 transition-colors duration-200 outline-none",
			"focus-visible:outline-accent focus-visible:outline-2 focus-visible:outline-offset-2",
			"enabled:hover:border-accent/70 enabled:cursor-pointer disabled:cursor-not-allowed disabled:opacity-50",
			checkboxState === "unchecked" && "border-border bg-background enabled:hover:bg-secondary/70",
			checkboxState !== "unchecked" && "border-accent bg-accent enabled:hover:bg-accent/90"
		]}
	/>

	<Minus
		size={12}
		strokeWidth={4}
		class={[
			"text-accent-foreground pointer-events-none absolute z-10 transition-all duration-200",
			checkboxState === "indeterminate" ? "scale-100 opacity-100" : "scale-75 opacity-0"
		]}
	/>
	<Check
		size={12}
		strokeWidth={4}
		class={[
			"text-accent-foreground pointer-events-none absolute z-10 transition-all duration-200",
			checkboxState === "checked" ? "scale-100 opacity-100" : "scale-75 opacity-0"
		]}
	/>
</div>
