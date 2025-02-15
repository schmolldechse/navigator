<script lang="ts">
	import "$src/app.css";
	import Logo from "$components/Logo.svelte";
	let { children } = $props();
</script>

<div class="flex flex-col h-screen w-screen overflow-hidden">
	<!-- Logo -->
	<div class="m-4 flex items-center justify-between pr-4">
		<a href="/">
			<div class="flex cursor-pointer flex-row items-center">
				<Logo />
				<h1 class="relative top-[-0.25rem] hidden text-[2rem] font-bold md:inline">NAVIGATOR</h1>
			</div>
		</a>
	</div>

	<!-- Coach-Sequence -->
	{@render children()}
</div>
