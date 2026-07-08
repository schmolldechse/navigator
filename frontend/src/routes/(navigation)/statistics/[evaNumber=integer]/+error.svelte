<script lang="ts">
	import { page } from "$app/state";
	import Button from "@lib/components/ui/Button.svelte";

	const message = $derived(page.error?.message ?? "Station details could not be loaded.");
</script>

<svelte:head>
	<title>Station unavailable - Navigator</title>
</svelte:head>

<main class="grid h-full place-items-center overflow-y-auto px-4 py-8 text-center">
	<div
		class="grid w-full max-w-3xl justify-items-center gap-y-6 md:w-fit md:grid-cols-[auto_minmax(0,1fr)] md:items-center md:justify-items-start md:gap-x-8 md:text-left"
	>
		<span class="text-accent/85 text-7xl font-extrabold sm:text-8xl">
			{page.status}
		</span>

		<div class="grid max-w-xl gap-2">
			<h1 class="text-2xl font-semibold sm:text-3xl">Station unavailable</h1>
			<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">{message}</p>
		</div>

		<Button href="/statistics" mode="primary" class="md:col-start-2">Back to Statistics</Button>
	</div>
</main>
