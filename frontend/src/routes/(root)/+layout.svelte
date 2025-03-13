<script lang="ts">
	import Navbar from "$components/Navbar.svelte";
	import type { LayoutProps } from "./$types";

	let { children }: LayoutProps = $props();
</script>

<div class="flex min-h-screen w-full flex-col">
	<Navbar dropdownEnabled={true} />
	{@render children()}
</div>
