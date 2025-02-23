<script lang="ts">
	import "$src/app.css";
	import Navbar from "$components/Navbar.svelte";
	import type { LayoutProps } from "./$types";

	let { children }: LayoutProps = $props();
</script>

<div class="min-h-screen flex flex-col">
	<Navbar />

	{@render children()}
</div>
