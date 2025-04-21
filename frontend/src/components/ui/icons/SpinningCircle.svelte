<script lang="ts">
	let {
		width = "75px",
		height = "75px"
	}: {
		width?: string;
		height?: string;
	} = $props();
</script>

<svg id="spinner" {height} {width} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1024 1024">
	<circle class="cls-1" cx="512" cy="512" r="450" />
	<path class="cls-2" d="M687,512c0-96.65-78.35-175-175-175s-175,78.35-175,175" />
</svg>

<style>
	.cls-1 {
		stroke: var(--text);
		stroke-width: 100px;
	}

	.cls-1,
	.cls-2 {
		fill: none;
		stroke-miterlimit: 10;
	}

	.cls-2 {
		stroke: var(--accent);
		stroke-width: 125px;
		stroke-linecap: round;
		animation: spin 1.5s linear infinite;

		transform-origin: center center;
	}

	@keyframes spin {
		0% {
			transform: rotate(0deg);
		}

		100% {
			transform: rotate(360deg);
		}
	}
</style>
