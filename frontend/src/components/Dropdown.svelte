<script lang="ts">
	import Menu from "@lucide/svelte/icons/menu";
	import { onMount } from "svelte";

	export interface DropdownElement {
		name: string;
		href: string;
	}

	let { dropdownElements }: { dropdownElements: DropdownElement[] } = $props();

	let isOpen = $state(false);
	let burgerHov = $state(false);
	let dropDownHov = $state(false);
	let menuTriggerElement = $state<HTMLElement | null>(null);
	let dropdownContainer = $state<HTMLElement | null>(null);

	onMount(() => {
		const handleClick = (event: MouseEvent) => {
			if (dropdownContainer?.contains(event.target as Node) || menuTriggerElement?.contains(event.target as Node)) return;
			isOpen = false;
		};

		window.addEventListener("click", handleClick);
		return () => window.removeEventListener("click", handleClick);
	});

	$effect(() => {
		if (burgerHov || dropDownHov) {
			isOpen = true;
		} else {
			isOpen = false;
		}
	});
</script>

<div class="relative flex flex-col">
	<!-- DropDown Button -->
	<button
		bind:this={menuTriggerElement}
		class={["cursor-pointer", {"text-accent": isOpen}]}
		onmouseenter={() => (burgerHov = true)}
		onmouseleave={() => (burgerHov = false)}
	>
		<Menu size={35} />
	</button>

	<!-- Dropdown Options -->
	{#if isOpen}
		<div
			class="bg-primary-darker absolute right-0 top-full z-10 mt-0 w-48 rounded-md p-2 shadow-lg"
			bind:this={dropdownContainer}
			onmouseenter={() => {
				dropDownHov = true;
			}}
			onmouseleave={() => {
				dropDownHov = false;
			}}
			role="button"
			tabindex="0"
		>
			{#each dropdownElements as element}
				<a
					class="text-text hover:text-accent hover:bg-primary-dark block w-full cursor-pointer rounded-md px-4 py-2 text-left text-base"
					href={element.href}
					onclick={() => (isOpen = false)}
				>
					{element.name}
				</a>
			{/each}
		</div>
	{/if}
</div>
