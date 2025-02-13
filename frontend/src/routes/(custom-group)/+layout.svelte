<script lang="ts">
	import "$src/app.css";
	import Navbar from "$components/Navbar.svelte";
	import Logo from "$components/Logo.svelte";
	import { setContext } from "svelte";

	let { children } = $props();

	let type = $state("timetable");
	setContext("type", () => type);
</script>

<div class="flex h-screen w-screen flex-col overflow-hidden">
	<!-- Logo/ Navbar (Timetables / Route Planner) -->
	<div class="m-4 flex items-center justify-between pr-4">
		<a href="/">
			<div class="flex cursor-pointer flex-row items-center">
				<Logo />
				<h1 class="relative top-[-0.25rem] hidden text-[2rem] font-bold md:inline">NAVIGATOR</h1>
			</div>
		</a>

		<Navbar bind:type />
	</div>

	<!-- TimetableSearch/ Route Planner -->
	{@render children()}
</div>
