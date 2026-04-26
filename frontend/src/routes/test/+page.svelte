<script lang="ts">
	import Button from "@lib/components/ui/Button.svelte";
	import TimePickerDialog from "@lib/components/ui/timepicker/TimePickerDialog.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";

	let items: { id: number; name: string; disabled: boolean }[] = $state([
		{ id: 1, name: "Option 1", disabled: true },
		{ id: 2, name: "Option 2", disabled: false },
		{ id: 3, name: "Option 3", disabled: false }
	]);
	// select a random option
	let selected: { id: number; name: string; disabled: boolean }[] = $state([items[Math.floor(Math.random() * items.length)]]);

	let isVisible: boolean = $state(false);
</script>

<div class="flex items-center gap-4">
	<h1>BulkSelection Test Cases</h1>
	<ToggleGroup bind:selected mode="multiple">
		{#each items as item (item.id)}
			<ToggleGroupItem {item} disabled={item.disabled} class={["text-white data-[active=true]:text-red-500"]}>
				{item.name}
			</ToggleGroupItem>
		{/each}
	</ToggleGroup>
</div>

<div class="flex items-center gap-4">
	<Button
		mode="secondary"
		onclick={(event: MouseEvent) => {
			event.stopPropagation();
			isVisible = !isVisible;
		}}
	>
		Toggle Visibility
	</Button>
	<Button
		mode="tertiary"
		onclick={(event: MouseEvent) => {
			event.stopPropagation();
			isVisible = !isVisible;
		}}
	>
		Toggle Visibility
	</Button>
</div>

<TimePickerDialog bind:isVisible />
