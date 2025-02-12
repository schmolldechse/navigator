<script lang="ts">
	let { width = "25px", height = "25px" } = $props();
</script>

<style lang="postcss">
    .cls-1 {
		@apply fill-text;
	}

	.cls-2 {
		fill: none;
		@apply stroke-text;
		stroke-miterlimit: 10;
		stroke-width: 30px;
	}

	.cls-3 {
		@apply fill-background;
	}
</style>

<svg
	id="reservation-required"
	xmlns="http://www.w3.org/2000/svg"
	viewBox="0 0 1024 1024"
	{height}
	{width}
	class={"shrink-0"}
>
	<g id="reservation-required">
		<rect class="cls-3" x="-165.68" y="-115.04" width="1338.73" height="1364.04"/>
		<rect class="cls-2" x="45.76" y="55.81" width="945.91" height="945.91" rx="50" ry="50"/>
		<path class="cls-1" d="M596.44,589.68c71.39-21.38,123.7-69.87,128.23-186.16-2.29-129.95-72.52-193.21-204.27-196.1h-235.39v656.94h97.1v-264.75h109.91l155.7,264.75h111.76l-163.05-274.68ZM382.11,520.99v-229.52h139.19c71.29.86,104.82,41.09,106.26,114.76-.41,73.19-34.46,112.81-106.26,114.76h-139.19Z"/>
	</g>
</svg>