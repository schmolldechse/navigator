<script lang="ts">
	import "$src/app.css";
	import Logo from "$components/Logo.svelte";
	import TimetableNavbar from "$components/timetable/TimetableNavbar.svelte";

	let { children } = $props();
</script>

<div class="flex h-screen w-screen flex-col overflow-hidden">
	<!-- Logo/ TimetableNavbar -->
	<div class="flex items-center justify-between pr-4">
		<a href="/">
			<div class="flex cursor-pointer flex-row items-center">
				<Logo />
				<h1 class="relative top-[-0.25rem] hidden text-[2rem] font-bold md:inline">NAVIGATOR</h1>
			</div>
		</a>

		<TimetableNavbar />
	</div>

	<!-- TimeTableSearch/ Route Planner -->
	{@render children()}
</div>
