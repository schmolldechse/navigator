<script lang="ts">
	let {
		width = "35px",
		height = "35px",
		type = "rounded-corners"
	}: {
		width?: string;
		height?: string;
		type?: "rounded-corners" | "circle";
	} = $props();
</script>

<svg {width} {height} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1024 1024" id="transport-products">
	<g id="suburban">
		{#if type === "rounded-corners"}
			<rect class="cls-1" x="24.55" y="24.5" width="975" height="975" rx="256" ry="256" />
		{:else}
			<circle class="cls-1" cx="511.95" cy="512" r="487.5" />
		{/if}
		<path
			class="cls-2"
			d="M785.02,339.55c-71.31-79.52-178.63-149.45-304.52-149.45-71.23,0-111.94,39.29-111.94,85.27,0,193.49,465.47,57.48,465.47,375.49,0,128.41-120.32,277.9-322.94,277.9-112.93,0-243.46-52.73-314.64-118.85v-166.7c67.51,100.61,190.6,178.19,314.64,178.19,75.89,0,136.92-44.95,136.92-92.92,0-181.05-458.04-68.94-458.04-381.28,0-163.8,154.51-251.97,293.33-251.97,115.7,0,219.34,39.33,301.67,105.46v138.86h.04Z"
		/>
	</g>
</svg>

<style>
	.cls-1 {
		fill: #0a8f50;
	}

	.cls-2 {
		fill: var(--color-text);
		fill-rule: evenodd;
	}
</style>
