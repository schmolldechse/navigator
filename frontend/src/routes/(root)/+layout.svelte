<script lang="ts">
	import Navbar from "$components/Navbar.svelte";
	import type { LayoutProps } from "./$types";
	import { setContext } from "svelte";
	import { DateTime } from "luxon";
	import GitHub from "$components/ui/icons/GitHub.svelte";

	let { children }: LayoutProps = $props();
	let currentType = $state("timetable");

	setContext("currentType", () => currentType);
</script>

<div class="flex min-h-screen w-full flex-col">
	<Navbar bind:currentType dropdownEnabled={true} />
	{@render children()}

	<footer
		class="bg-primary-darker sticky bottom-0 flex w-full flex-row items-center justify-between p-4 text-xs md:text-base">
		<span>&copy; {DateTime.now().year} - Schmolldechse & Contributors</span>

		<a class="flex flex-row items-center gap-2" href="https://github.com/schmolldechse/navigator">
			GitHub
			<GitHub width="24px" height="24px" />
		</a>
	</footer>
</div>