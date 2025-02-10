<script lang="ts">
	import NavbarButton from "$components/NavbarButton.svelte";

	let { type = $bindable() } = $props();
</script>

<span class="montserrat-regular flex items-center space-x-4 text-base md:text-xl">
	<NavbarButton isSelected={type === "departures"} onclick={() => (type = "departures")}>Departures</NavbarButton>
	<NavbarButton isSelected={type === "arrivals"} onclick={() => (type = "arrivals")}>Arrivals</NavbarButton>
</span>
