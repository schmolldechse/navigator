<script lang="ts">
	import Minus from "@lucide/svelte/icons/minus";
</script>

<div class="border-primary/45 space-y-2 rounded-lg border-2 px-4 py-2 text-2xl font-medium">
	<!-- Time Info -->
	<div class="flex flex-row gap-x-2">
		<div class="flex flex-row">
			<span class="bg-primary-dark/40 w-[4em] animate-pulse text-2xl"></span>
			<Minus class="mx-2 mt-[0.25rem]" />
			<span class="bg-primary-dark/40 w-[4em] animate-pulse text-2xl"></span>
		</div>

		<span class="text-primary/90">|</span>
		<span class="bg-primary-dark/40 mt-[0.35rem] w-[3em] animate-pulse text-lg"></span>
	</div>

	<!-- Legs -->
	<div class="flex w-full flex-row gap-x-2">
		<span class="bg-primary-dark/40 h-[2rem] w-[30%] min-w-[5em] animate-pulse text-base"></span>
		<span class="bg-primary-dark/40 w-[55%] min-w-[5em] animate-pulse text-base"></span>
		<span class="bg-primary-dark/40 w-[15%] min-w-[5em] animate-pulse text-base"></span>
	</div>
</div>
