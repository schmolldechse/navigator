<script lang="ts">
	import { page } from "$app/state";
	import ChartLine from "@lucide/svelte/icons/chart-line";
	import Clock4 from "@lucide/svelte/icons/clock-4";
	import Map from "@lucide/svelte/icons/map";
	import Menu from "@lucide/svelte/icons/menu";
	import X from "@lucide/svelte/icons/x";
	import type { Component } from "svelte";

	export type NavigationItem = {
		href: string;
		pageName: string;
		icon?: string;
	};

	let { navigationItems }: { navigationItems?: NavigationItem[] } = $props();

	let isMenuOpen: boolean = $state(false);

	const icons: Record<string, Component> = {
		Map,
		ChartLine,
		Clock4
	};
</script>

<header class="border-muted border-b">
	<div class="flex h-16 items-center justify-between px-4">
		<a href="/" class="flex items-center gap-3">
			<img src="/logo.svg" alt="Navigator Logo" width="40px" height="40px" class="h-10 w-10" />
		</a>

		<nav class="hidden items-center gap-6 md:flex">
			{#each navigationItems as navigationItem}
				{@const isVisited = page.url.pathname === navigationItem.href}

				<a
					href={navigationItem.href}
					class={[
						"hover:text-accent flex items-center gap-x-2 text-sm font-medium transition-colors",
						{ "text-accent": isVisited },
						{ "text-muted-foreground": !isVisited }
					]}
				>
					{navigationItem.pageName}
				</a>
			{/each}
		</nav>

		<button
			onclick={() => (isMenuOpen = !isMenuOpen)}
			class="border-muted flex flex-col gap-1.5 rounded-md border-2 p-1 md:hidden"
			aria-label="Open menu"
		>
			{#if isMenuOpen}<X />
			{:else}<Menu />
			{/if}
		</button>

		{#if isMenuOpen}
			<!-- backdrop -->
			<div class="fixed inset-0 z-50 bg-black/70 md:hidden" onclick={() => (isMenuOpen = false)}></div>

			<nav
				class="bg-background border-secondary fixed right-0 top-0 z-50 flex h-full w-3/4 flex-col border-2 px-4 py-16 shadow-lg md:hidden"
			>
				{#each navigationItems as navigationItem}
					{@const isVisited = page.url.pathname === navigationItem.href}
					{@const Icon = navigationItem.icon ? icons[navigationItem.icon] : null}

					<a
						href={navigationItem.href}
						class={[
							"hover:text-accent mb-4 flex items-center gap-x-2 rounded-md p-2 text-sm font-medium transition-colors",
							{ "text-accent bg-accent/20": isVisited },
							{ "text-muted-foreground": !isVisited }
						]}
						onclick={() => (isMenuOpen = false)}
					>
						{#if Icon}
							<Icon class="h-4 w-4" />
						{/if}
						{navigationItem.pageName}
					</a>
				{/each}
			</nav>
		{/if}
	</div>
</header>
