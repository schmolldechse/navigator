<script lang="ts">
	import Navbar from "$components/Navbar.svelte";
	import type { LayoutProps } from "./$types";
	import { setContext } from "svelte";

	let { children }: LayoutProps = $props();
	let currentType = $state("timetable");

	setContext("currentType", () => currentType);
</script>

<div class="flex min-h-screen w-full flex-col">
	<Navbar bind:currentType dropdownEnabled={true} />
	{@render children()}
</div>
