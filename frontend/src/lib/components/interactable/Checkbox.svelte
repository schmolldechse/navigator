<script lang="ts">
	import Checkbox from "@lib/components/ui/Checkbox.svelte";

	type Props = {
		checked: boolean;
		disabled?: boolean;
	};
	let { checked = $bindable(false), disabled }: Props = $props();
</script>

<Checkbox bind:checked {disabled} />
