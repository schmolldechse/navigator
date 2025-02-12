<script lang="ts">
	let { width = "25px", height = "25px" } = $props();
</script>

<style lang="postcss">
    .cls-1 {
        @apply fill-text;
    }

    .cls-2 {
        @apply fill-background;
    }
</style>

<svg
	id="canceled-stops"
	xmlns="http://www.w3.org/2000/svg"
	viewBox="0 0 1024 1024"
	{height}
	{width}
	class={"shrink-0"}
>
	<g id="canceled-stops">
		<rect class="cls-1" x="-41.77" y="-41.77" width="1107.55" height="1107.55"/>
		<polygon class="cls-2" points="376.79 238.63 376.79 455.42 241.79 455.42 241.79 238.63 117.78 238.63 117.78 782.19 241.79 782.19 241.79 567.77 376.79 567.77 376.79 782.19 500.81 782.19 500.81 238.63 376.79 238.63"/>
		<rect class="cls-2" x="601" y="399.36" width="326.77" height="90.07"/>
	</g>
</svg>