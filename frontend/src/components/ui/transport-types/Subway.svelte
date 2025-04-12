<script lang="ts">
	let { width = "35px", height = "35px", type = "rounded-corners" }: {
		width?: string;
		height?: string;
		type?: "rounded-corners" | "rectangle";
	} = $props();
</script>

<svg {width} {height} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1024 1024" id="transport-products">
	<g id="subway">
		{#if type === "rectangle"}
			<rect class="cls-1" x="0" y="0" width="1024" height="1024"/>
		{:else}
			<rect class="cls-1" x="24.5" y="24.5" width="975" height="975" rx="256" ry="256"/>
		{/if}
		<path class="cls-2" d="M246.19,151.37h-11.11v455.13c0,87.24,27.25,154.23,76.41,199.29,49.03,44.98,118.35,66.84,200.03,66.84s151.24-21.86,200.47-66.8c49.43-45.06,76.92-112.05,76.92-199.33V151.37h-135.25v417.2c0,44.86-1.77,90.11-20.2,124.22-9.21,17.48-23.58,31.7-41.16,40.72-19.26,10.04-45.29,16.27-80.78,16.27s-61.64-6.22-80.82-16.27c-17.5-8.97-31.76-23.18-40.8-40.64-18.12-34.15-19.57-79.36-19.57-124.3V151.37h-124.14Z"/>
	</g>
</svg>

<style>
    .cls-1 {
        fill: #3d64ad;
    }

    .cls-2 {
        fill: var(--color-text);
    }
</style>
