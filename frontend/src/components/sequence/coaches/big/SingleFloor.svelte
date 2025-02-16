<script lang="ts">
	let { height = "75px", width = "75px" }: { height?: string, width?: string } = $props();
</script>

<svg id="coach-sequence" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1024 1024"
	 {height}
	 {width}
	 class="shrink-0">
	<g id="coaches-big">
		<g id="single-floor">
			<path class="cls-1" d="M879.15,311.65H144.85c-24.85,0-45,20.15-45,45v298.46h45.75l.03,47.25c0,5.52,4.48,9.99,10,9.99h163.15c5.52,0,10-4.48,10-10v-47.24h366.36l-.36,47.17c-.04,5.55,4.45,10.08,10,10.08h163.54c5.52,0,10-4.48,10-10v-47.24h45.84v-298.46c0-24.85-20.15-45-45-45ZM832.56,505.22c0,6.9-5.6,12.5-12.5,12.5H203.94c-6.9,0-12.5-5.6-12.5-12.5v-112.38c0-6.9,5.6-12.5,12.5-12.5h616.13c6.9,0,12.5,5.6,12.5,12.5v112.38Z"/>
		</g>
	</g>
</svg>

<style lang="postcss">
    .cls-1 {
        @apply fill-text;
    }
</style>