<script lang="ts">
	import NavbarButton from "$components/NavbarButton.svelte";

	let { type = $bindable() } = $props();
</script>

<span class="montserrat-regular flex items-center space-x-4 text-base md:text-xl">
	<NavbarButton isSelected={type === "timetable"} onclick={() => (type = "timetable")}>Timetable</NavbarButton>
	<NavbarButton isSelected={type === "route_planner"} onclick={() => (type = "route_planner")}>Route Planner</NavbarButton>
</span>
