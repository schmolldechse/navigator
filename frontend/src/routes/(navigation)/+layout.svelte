<script lang="ts">
	import type { LayoutProps } from "./$types";
	import { page } from "$app/state";
	import ChartLine from "@lucide/svelte/icons/chart-line";
	import Clock_4 from "@lucide/svelte/icons/clock-4";
	import Logo from "$lib/components/Logo.svelte";
	import type { Component } from "svelte";
	import X from "@lucide/svelte/icons/x";
	import Menu from "@lucide/svelte/icons/menu";
	import { type ClassValue } from "svelte/elements";
	import Button from "@lib/components/ui/Button.svelte";

	let { data, children }: LayoutProps = $props();

	let isMenuOpen: boolean = $state(false);
	let currentPath: string = $state(page.url.pathname);
	const mobileNavigationId = "mobile-navigation-menu";

	const icons: Record<string, Component> = {
		ChartLine: ChartLine as Component,
		Clock_4: Clock_4 as Component
	};

	$effect(() => {
		const nextPath = page.url.pathname;
		if (nextPath !== currentPath) {
			currentPath = nextPath;
			isMenuOpen = false;
		}
	});
</script>

{#snippet pageEntries({ class: className, onNavigate }: { class: ClassValue; onNavigate?: () => void })}
	<nav class={className}>
		{#each data.pages as pageEntry}
			{@const isVisited = page.url.pathname === pageEntry.href}
			{@const IconItem = pageEntry.icon ? icons[pageEntry.icon] : null}

			<Button
				mode="tertiary"
				href={pageEntry.href}
				onclick={onNavigate}
				class={[
					"group hover:text-accent! flex items-center gap-2 text-sm font-medium transition-colors",
					isVisited && "text-accent!",
					!isVisited && "text-foreground/60!"
				]}
			>
				{#if IconItem}
					<IconItem class="h-4 w-4 transition-transform group-hover:scale-110" />
				{/if}
				<span>{pageEntry.pageName}</span>
			</Button>
		{/each}
	</nav>
{/snippet}

<header class="bg-background/95 border-border sticky top-0 z-100 w-full border-b-2">
	<div class="flex h-16 items-center justify-between px-4">
		<Logo />

		{@render pageEntries({ class: "hidden md:flex items-center gap-x-6" })}

		<Button
			mode="secondary"
			class="flex md:hidden"
			aria-controls={mobileNavigationId}
			aria-expanded={isMenuOpen}
			aria-label={isMenuOpen ? "Close navigation" : "Open navigation"}
			onclick={(event: MouseEvent) => {
				event.stopPropagation();
				isMenuOpen = !isMenuOpen;
			}}
		>
			{#if isMenuOpen}<X />
			{:else}<Menu />
			{/if}
		</Button>
	</div>
</header>

{#if isMenuOpen}
	<div id={mobileNavigationId} class="fixed top-18 right-4 left-4 z-90 md:hidden">
		<div class="border-border bg-background/95 rounded-xl border-2 p-2 shadow-2xl backdrop-blur-md sm:ml-auto sm:max-w-xs">
			{@render pageEntries({
				class: "flex flex-col items-stretch gap-y-1",
				onNavigate: () => (isMenuOpen = false)
			})}
		</div>
	</div>
{/if}

{@render children?.()}
