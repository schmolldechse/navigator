<script lang="ts">
	import "../app.css";
	import Navbar from "$components/Navbar.svelte";
	import Logo from "$components/Logo.svelte";

	let { children } = $props();

	let type = $state("timetable");
</script>

<div class="m-4">
	<div class="flex justify-between items-center pr-4 bg-slate-300 dark:bg-transparent">

		<!-- Logo -->
		<a href="/">
			<span class="flex flex-row items-center font-bold cursor-pointer">
				<Logo />
				<h1 class="hidden md:inline text-[2rem] relative top-[-0.25rem]">NAVIGATOR</h1>
			</span>
		</a>

		<Navbar bind:type />
	</div>

	{@render children()}

</div>

