<script lang="ts">
	import VisualIdentity from "$components/branding/VisualIdentity.svelte";
	import UserInfo from "$components/auth/UserInfo.svelte";
	import Menu from "lucide-svelte/icons/menu";

	let { userInfo = true, dropdownEnabled = false, currentType = $bindable("timetable") }: {
		userInfo?: boolean,
		dropdownEnabled?: boolean,
		currentType?: string
	} = $props();
	let dropdownOpen = $state<boolean>(false);

	let menuTriggerElement = $state<HTMLElement | null>(null);
	let dropdownElement = $state<HTMLElement | null>(null);

	type DropdownElement = {
		name: string;
		type: string;
	}

	let dropdownElements = $state<DropdownElement[]>([
		{ name: "Timetable", type: "timetable" },
		{ name: "Route Planner", type: "route_planner" }
	]);

	$effect(() => {
		if (!dropdownOpen) return;

		const handleClick = (event: MouseEvent) => {
			if (dropdownElement?.contains(event.target as Node) || menuTriggerElement?.contains(event.target as Node)) return;
			dropdownOpen = false;
		};

		window.addEventListener("click", handleClick);
		return () => window.removeEventListener("click", handleClick);
	});
</script>

<div class="bg-background sticky top-0 flex h-20 w-full items-center justify-between px-2 pt-4 pr-4">
	<VisualIdentity />

	<div class="flex flex-row items-center gap-x-4 relative">
		{#if userInfo}
			<UserInfo />
		{/if}
		{#if dropdownEnabled}
			<button bind:this={menuTriggerElement} onclick={() => dropdownOpen = !dropdownOpen}>
				<Menu size={35} class="cursor-pointer" />
			</button>

			{#if dropdownOpen}
				<div class="absolute top-0 right-0 mt-2 w-48 bg-primary-darker shadow-lg rounded-md p-2 z-10"
					 bind:this={dropdownElement}>
					{#each dropdownElements as element}
						<button
							class="text-left w-full block px-4 py-2 text-base text-text hover:bg-primary-dark cursor-pointer" onclick={() => {
								currentType = element.type;
								dropdownOpen = false;
							}}>{element.name}</button>
					{/each}
				</div>
			{/if}
		{/if}
	</div>
</div>
