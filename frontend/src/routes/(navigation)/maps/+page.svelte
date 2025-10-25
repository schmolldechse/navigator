<script lang="ts">
	import Map from "$lib/components/Map.svelte";
</script>

<svelte:head>
	<title>Station Map - Navigator</title>
</svelte:head>

<div class="flex-1 p-4 md:p-6">
	<Map />
</div>

<style>
	:global(html, body) {
		@apply flex h-screen flex-col overflow-hidden;
	}
</style>
