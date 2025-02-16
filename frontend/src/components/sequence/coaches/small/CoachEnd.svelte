<script lang="ts">
	let { firstClass }: { firstClass: boolean } = $props();
</script>

<svg
	id="coach-sequence"
	xmlns="http://www.w3.org/2000/svg"
	viewBox="0 0 1024 1024"
	class="h-full w-full shrink-0"
	preserveAspectRatio="xMidYMid meet"
>
	<g id="coaches-small">
		<g id="coach-end">
			<polyline class="cls-1" points="75 762 887 762 887 162 75 162" />
			{#if firstClass}
				<line class="cls-2" x1="75" y1="924" x2="913.2" y2="924" />
			{/if}
		</g>
	</g>
</svg>

<style lang="postcss">
	.cls-1 {
		@apply stroke-text;
		stroke-width: 50px;
	}

	.cls-1,
	.cls-2 {
		@apply fill-none;
		stroke-miterlimit: 10;
	}

	.cls-2 {
		@apply stroke-accent;
		stroke-width: 75px;
	}
</style>
