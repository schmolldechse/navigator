<script lang="ts">
	let { children, isSelected, onclick } = $props();
</script>

<button
	onclick={() => onclick("arrivals")}
	class="cursor-pointer decoration-2 underline-offset-4"
	class:underline={isSelected}
	class:no-underline={!isSelected}
>
	{@render children()}
</button>
