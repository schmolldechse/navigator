<script lang="ts">
	import "$src/app.css";
	import Navbar from "$components/Navbar.svelte";
	import type { LayoutProps } from "./$types";

	let { children }: LayoutProps = $props();
</script>

<div class="flex h-screen w-screen flex-col overflow-hidden">
	<Navbar userInfo={false} />
	{@render children()}
</div>
