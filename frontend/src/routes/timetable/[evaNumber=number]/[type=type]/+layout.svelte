<script>
	import Navbar from "$components/Navbar.svelte";

    let { children } = $props();
</script>

<div class="flex min-h-screen flex-col">
	<Navbar />
	{@render children()}
</div>