<script lang="ts">
	let { width = "25px", height = "25px" } = $props();
</script>

<svg id="coach-sequence" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1024 1024" {height} {width} class={"shrink-0"}>
	<g id="equipments">
		<g id="SEATS_SEVERELY_DISABLED">
			<rect class="cls-1" width="1024" height="1024"/>
			<g>
				<polygon class="cls-2" points="144.03 224.93 208.13 224.93 344.73 708.3 565.4 708.3 544.39 781.86 281.69 802.88 144.03 224.93"/>
				<circle class="cls-2" cx="611.84" cy="198.86" r="70.11"/>
				<polygon class="cls-2" points="798.68 387.39 852.19 387.39 879.97 895.47 843.19 895.47 798.68 387.39"/>
				<path class="cls-2" d="M679.94,876.56h60.78l-62.08-254.34c-5.77-26.37-22.1-39.36-45.6-43.25l-113.86-25.43,98.48-162.33,113.28-39.8,125.13-27.07-7.06-43-170.36,16.56c-33.67,4.31-63.38,8.6-78.05,8.41-21.23.38-42.31-8.86-57.46-8.41-23.03.33-39.36,10.86-50.61,29.13l-101.38,165.35c-12.26,20.43-21.63,40.81-25.81,61.07-3.65,15.65-3.92,30.32-.21,43.84,11.94,36.37,37.31,54.21,74.48,55.73,16.16.34,35.45.55,55.69.71l117.94-.21,4.37,20.68,62.33,202.34Z"/>
			</g>
			<rect class="cls-3" x="37" y="37.11" width="950" height="950" rx="95" ry="95"/>
		</g>
	</g>
</svg>

<style lang="postcss">
    .cls-1 {
        @apply fill-primary-darker;
    }

    .cls-2 {
        @apply fill-text;
    }

	.cls-3 {
		@apply fill-none stroke-text;
		stroke-miterlimit: 10;
		stroke-width: 30px;
	}
</style>
