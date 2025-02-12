<script lang="ts">
	import type { Journey } from "$models/connection";
	import ConnectionComponent from "$components/timetable/ConnectionComponent.svelte";

	let { journey }: { journey: Journey } = $props();
</script>

<div class="flex flex-col">
	{#each journey.connections as connection, i}
		<ConnectionComponent {connection} renderBorder={i === 0} renderInformation={i === journey.connections.length - 1} />
	{/each}
</div>
