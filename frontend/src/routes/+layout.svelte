<script lang="ts">
	import "../app.css";
	import Navbar from "$components/Navbar.svelte";
	import Logo from "$components/Logo.svelte";
	import { setContext } from "svelte";

	let { children } = $props();

	let type = $state("timetable");

	setContext("type", () => type);
</script>

<div class="m-4">
	<div class="mb-4 flex items-center justify-between bg-accent pr-4 dark:bg-transparent">
		<!-- Logo -->
		<a href="/">
			<span class="flex cursor-pointer flex-row items-center font-bold">
				<Logo />
				<h1 class="relative top-[-0.25rem] hidden text-[2rem] md:inline">NAVIGATOR</h1>
			</span>
		</a>

		<Navbar bind:type />
	</div>

	{@render children()}
</div>
