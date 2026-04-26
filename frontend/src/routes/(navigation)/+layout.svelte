<script lang="ts">
	import type { LayoutProps } from "./$types";
	import { page } from "$app/state";
	import ChartLine from "@lucide/svelte/icons/chart-line";
	import Clock_4 from "@lucide/svelte/icons/clock-4";
	import Map from "@lucide/svelte/icons/map";
	import Logo from "$lib/components/Logo.svelte";
	import type { Component } from "svelte";
	import X from "@lucide/svelte/icons/x";
	import Menu from "@lucide/svelte/icons/menu";
	import { type ClassValue } from "svelte/elements";
	import Button from "@lib/components/ui/Button.svelte";
	import Dialog from "@lib/components/ui/dialog/Dialog.svelte";

	let { data, children }: LayoutProps = $props();

	let isMenuOpen: boolean = $state(false);

	const icons: Record<string, Component> = {
		Map: Map as Component,
		ChartLine: ChartLine as Component,
		Clock_4: Clock_4 as Component
	};
</script>

{#snippet pageEntries({ class: className }: { class: ClassValue })}
	<nav class={className}>
		{#each data.pages as pageEntry}
			{@const isVisited = page.url.pathname === pageEntry.href}
			{@const IconItem = pageEntry.icon ? icons[pageEntry.icon] : null}

			<Button
				mode="tertiary"
				href={pageEntry.href}
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

<Dialog
	bind:isVisible={isMenuOpen}
	title="Menu"
	showActions={false}
	clickOutsideToClose
	class="mt-auto max-h-[50vh] w-full max-w-full rounded-b-none border-b-0 md:hidden"
>
	{@render pageEntries({ class: "md:hidden flex flex-col items-start gap-y-2" })}
</Dialog>

{@render children?.()}
