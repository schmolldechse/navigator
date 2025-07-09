<script lang="ts">
	import VisualIdentity from "$components/branding/VisualIdentity.svelte";
	import UserInfo from "$components/auth/UserInfo.svelte";
	import Dropdown, { type DropdownElement } from "$components/Dropdown.svelte";
	import { page } from "$app/state";

	let {
		userInfo = true,
		dropdownEnabled = false
	}: {
		userInfo?: boolean;
		dropdownEnabled?: boolean;
	} = $props();

	let dropdownElements = $state<DropdownElement[]>([
		{ name: "Timetable", href: "/timetable" },
		{ name: "Route Planner", href: "/routeplanner" }
	]);
</script>

<div class="z-999 sticky top-0 flex w-full items-center justify-between bg-[#171717] p-2 px-4 md:hidden">
	<VisualIdentity />

	<div class="relative flex flex-row items-center gap-x-4">
		{#if userInfo}
			<UserInfo />
		{/if}
		{#if dropdownEnabled}
			<Dropdown {dropdownElements} />
		{/if}
	</div>
</div>

<div class="z-999 sticky top-0 hidden w-full items-center justify-between bg-[#171717] p-2 px-4 md:flex">
	<div class="flex flex-row items-center gap-x-12">
		<VisualIdentity />
		<div class="flex flex-row gap-x-8">
			{#each dropdownElements as link}
				<a
					href={link.href}
					class={[
						"hover:text-accent relative cursor-pointer text-xl font-medium transition-colors duration-500",
						{ "text-accent": link.href === page.url.pathname }
					]}
				>
					{link.name}
					<span
						class="bg-accent absolute -bottom-1 left-0 h-0.5 w-full origin-left transition-transform duration-300"
						style="transform: scaleX({link.href === page.url.pathname ? 1 : 0});"
					></span>
				</a>
			{/each}
		</div>
	</div>
	<div class="relative flex flex-row items-center gap-x-4">
		{#if userInfo}
			<UserInfo />
		{/if}
	</div>
</div>
