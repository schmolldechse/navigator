<script lang="ts">
	import "../app.css";
	import Navbar from "$lib/Navbar.svelte";

	let { children } = $props();
</script>

<div class="m-4">
	<div class="flex justify-between items-center pr-4">

		<!-- Logo -->
		<a href="/">
			<span class="flex flex-row items-center font-bold cursor-pointer">
					<img src="/logo.svg" class="ml-[10px]" width=75 height=75 alt="LOGO" />
					<h1 class="hidden md:inline text-[2rem] relative top-[-0.25rem]">NAVIGATOR</h1>
			</span>
		</a>

		<Navbar />
	</div>

	{@render children()}

</div>

