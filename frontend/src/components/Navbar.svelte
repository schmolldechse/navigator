<script lang="ts">
	import VisualIdentity from "$components/branding/VisualIdentity.svelte";
	import UserInfo from "$components/auth/UserInfo.svelte";
	import Dropdown, { type DropdownElement } from "$components/Dropdown.svelte";

	let {
		userInfo = true,
		dropdownEnabled = false
	}: {
		userInfo?: boolean;
		dropdownEnabled?: boolean;
	} = $props();

	let dropdownElements = $state<DropdownElement[]>([
		{ name: "Timetable", href: "timetable" },
		{ name: "Route Planner", href: "route-planner" }
	]);
</script>

<div class="bg-background sticky top-0 z-999 flex w-full items-center justify-between p-2 px-4">
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
