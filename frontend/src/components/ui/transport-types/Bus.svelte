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
	<g id="bus">
		{#if type === "rounded-corners"}
			<rect class="cls-1" x="24.55" y="24.5" width="975" height="975" rx="256" ry="256" />
		{:else}
			<circle class="cls-1" cx="511.95" cy="512" r="487.5" />
		{/if}
		<path
			class="cls-2"
			d="M791.9,303.96c5.93-89.67-21.81-150-121.37-147.71l-324.48.66c-73.65.76-121.84,36.38-114.77,147.71-25.1-9.18-40.72-1.06-46.08,25.76v106.84c5.78,27.04,21.46,34.98,46.16,25.68v248.61c1.58,35.81,8.47,69.02,36.91,91.65v29.06c3.22,21.33,13.65,34.76,35.59,35.59h21.13c23.97-1.66,34.6-16.5,35.59-40.87h302.03c1.01,27.89,14.11,39.88,35.59,40.87h21.13c25.83-2.68,33.47-17.33,35.59-35.59v-29.06c25.13-19.55,38.34-51.6,36.98-98.92v-241.35c24.13,9.88,40.15,2.21,46.82-25.68v-107.5c-4.46-26.99-20.4-35.01-46.82-25.76ZM292.25,291.78h422.08v295.72h-422.08v-295.72ZM327.66,743.54c-21.3,0-38.56-17.26-38.56-38.56s17.26-38.56,38.56-38.56,38.56,17.26,38.56,38.56-17.26,38.56-38.56,38.56ZM679.59,743.54c-21.3,0-38.56-17.26-38.56-38.56s17.26-38.56,38.56-38.56,38.56,17.26,38.56,38.56-17.26,38.56-38.56,38.56Z"
		/>
	</g>
</svg>

<style>
	.cls-1 {
		fill: #c02987;
	}

	.cls-2 {
		fill: var(--color-background);
	}
</style>
