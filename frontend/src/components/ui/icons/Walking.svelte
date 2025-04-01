<script lang="ts">
	let { width = "75px", height = "75px", class: className = "" }: {
		width?: string;
		height?: string;
		class?: string;
	} = $props();
</script>

<svg {height} {width} viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"
	 stroke="#000000" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
	 color="#000000"
class={["shrink-0", className]}>
	<circle cx="13" cy="5" r="1" />
	<path d="M15 20L14 17L11 14M11 14L12 9M11 14L8 20M12 9L15 12L17 13M12 9L9 11L8 14" />
</svg>