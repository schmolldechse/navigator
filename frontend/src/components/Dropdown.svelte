<script lang="ts">
	import Menu from "lucide-svelte/icons/menu";
	import { onMount } from "svelte";

	export interface DropdownElement {
		name: string;
		type: string;
	}

	let { dropdownElements, setType }: { dropdownElements: DropdownElement[]; setType: (type: string) => void } = $props();

	let isOpen = $state(false);
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
</script>

<div class="relative flex flex-col">
	<button bind:this={menuTriggerElement} class="cursor-pointer" onclick={() => (isOpen = !isOpen)}>
		<Menu size={35} />
	</button>

	{#if isOpen}
		<div
			class="bg-primary-darker absolute top-full right-0 z-10 mt-2 w-48 rounded-md p-2 shadow-lg"
			bind:this={dropdownContainer}
		>
			{#each dropdownElements as element}
				<button
					class="text-text hover:bg-primary-dark block w-full cursor-pointer rounded-md px-4 py-2 text-left text-base"
					onclick={() => {
						isOpen = false;
						setType(element.type);
					}}
				>
					{element.name}
				</button>
			{/each}
		</div>
	{/if}
</div>
