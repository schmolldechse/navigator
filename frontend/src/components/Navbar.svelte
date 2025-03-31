<script lang="ts">
	import VisualIdentity from "$components/branding/VisualIdentity.svelte";
	import UserInfo from "$components/auth/UserInfo.svelte";
	import Dropdown, { type DropdownElement } from "$components/Dropdown.svelte";

	let {
		userInfo = true,
		dropdownEnabled = false,
		currentType = $bindable("timetable")
	}: {
		userInfo?: boolean;
		dropdownEnabled?: boolean;
		currentType?: string;
	} = $props();

	let dropdownElements = $state<DropdownElement[]>([
		{ name: "Timetable", type: "timetable" },
		{ name: "Route Planner", type: "route_planner" }
	]);
</script>

<div class="bg-background sticky top-0 z-999 flex h-20 w-full items-center justify-between px-2 pt-4 pr-4">
	<VisualIdentity />

	<div class="relative flex flex-row items-center gap-x-4">
		{#if userInfo}
			<UserInfo />
		{/if}
		{#if dropdownEnabled}
			<Dropdown {dropdownElements} setType={(type) => (currentType = type)} />
		{/if}
	</div>
</div>
