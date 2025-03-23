<script lang="ts">
	import Minus from "lucide-svelte/icons/minus";
</script>

<div class="border-primary-dark/75 space-y-2 rounded-lg border-2 px-4 py-2 text-2xl font-medium">
	<!-- Time Info -->
	<div class="flex flex-row gap-x-2">
		<div class="flex flex-row">
			<span class="animate-pulse w-[4em] bg-primary-dark/40 text-2xl"></span>
			<Minus class="mx-2 mt-[0.25rem]" />
			<span class="animate-pulse w-[4em] bg-primary-dark/40 text-2xl"></span>
		</div>

		<span class="text-primary/90">|</span>
		<span class="mt-[0.35rem] text-lg animate-pulse w-[3em] bg-primary-dark/40"></span>
	</div>

	<!-- Legs -->
	<div class="flex w-full flex-row gap-x-2">
		<span class="animate-pulse w-[30%] min-w-[5em] bg-primary-dark/40 text-base h-[2rem]"></span>
		<span class="animate-pulse w-[55%] min-w-[5em] bg-primary-dark/40 text-base"></span>
		<span class="animate-pulse w-[15%] min-w-[5em] bg-primary-dark/40 text-base"></span>
	</div>
</div>
