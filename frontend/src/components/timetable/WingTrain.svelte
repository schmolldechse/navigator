<script lang="ts">
	import type { Journey } from "$models/connection";
	import ConnectionComponent from "$components/timetable/ConnectionComponent.svelte";

	let { journey }: { journey: Journey } = $props();
</script>

<div class="flex flex-col">
	{#each journey.connections as connection}
		<ConnectionComponent {connection} />
	{/each}
</div>