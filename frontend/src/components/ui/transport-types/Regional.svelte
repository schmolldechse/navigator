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
	<g id="regional">
		{#if type === "rounded-corners"}
			<rect class="cls-1" x="24.55" y="24.5" width="975" height="975" rx="256" ry="256" />
		{:else}
			<circle class="cls-1" cx="511.95" cy="512" r="487.5" />
		{/if}
		<path
			class="cls-2"
			d="M578.82,153.66c102.25,0,185.78,79.56,189.08,179.2l.1,384c0,82.34-64.93,150.04-147.2,153.47-1.33-21.53-19.05-33.15-36.56-30.64-47.92,6.9-96.59,6.9-144.51,0-17.51-2.51-34.23,9.09-36.51,30.64-80.06-3.32-144.12-67.64-147.12-147.71l-.1-383.62c0-100.45,81.48-182.02,182.94-185.24l139.88-.1ZM384,713.84c-17.54,0-28.8,13.77-28.8,28.62s11.62,28.8,28.8,28.62l51.2.18c17.05,0,28.8-13.95,28.8-28.8s-11.37-28.8-28.8-28.8l-51.2.18ZM588.8,713.84c-15.83-.07-28.73,12.71-28.8,28.54,0,.03,0,.05,0,.08,0,14.85,11.96,28.62,28.8,28.8h51.2c15.88-.07,28.73-12.92,28.8-28.8,0-14.85-11.01-28.62-28.8-28.8l-51.2.18ZM597.66,230.46h-171.26c-37.53,0-67.99,30.92-67.99,69.02l15.1,279.81c-.05,33.54,26.27,60.77,58.75,60.77h158.41c32.36,0,58.62-27.08,58.75-60.52l16.18-279.71c.15-38.27-30.31-69.38-67.94-69.38"
		/>
	</g>
</svg>

<style>
	.cls-1 {
		fill: #e94546;
	}

	.cls-2 {
		fill: var(--color-background);
		fill-rule: evenodd;
	}
</style>
