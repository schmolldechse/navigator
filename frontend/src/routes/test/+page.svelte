<script lang="ts">
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import Input from "@lib/components/ui/Input.svelte";
	import Separator from "@lib/components/ui/Separator.svelte";

	let value = $state("");
</script>

<div class="flex items-center gap-4">
	<h1>Checkbox Test Case</h1>

	<Checkbox checked />
	<Checkbox checked disabled />
</div>

<div class="flex items-center gap-4">
	<h1>Input Test Case</h1>

	<Input type="text" bind:value disabled />
</div>

<div class="flex items-center gap-4">
	<h1>Separator Test Case</h1>

	<Separator />
</div>
