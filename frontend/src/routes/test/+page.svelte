<script lang="ts">
	import Button from "@lib/components/interactable/Button.svelte";
	import TimePickerDialog from "@lib/components/interactable/timepicker/TimePickerDialog.svelte";
	import { DateTime } from "luxon";

	let isVisible: boolean = $state(false);
	let value = $state(undefined);
</script>

<Button
	mode="primary"
	onclick={(event: MouseEvent) => {
		event.stopPropagation();
		isVisible = !isVisible;
	}}>Open</Button
>
<TimePickerDialog isRange bind:isVisible bind:value />
