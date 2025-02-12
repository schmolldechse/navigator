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
	id="general-warning"
	xmlns="http://www.w3.org/2000/svg"
	viewBox="0 0 1024 1024"
	{height}
	{width}
	class={"shrink-0"}
>
	<g id="general-warning">
		<rect class="cls-1" x="-88.14" y="-111.88" width="1215.3" height="1262.77"/>
		<path class="cls-2" d="M925.16,837.16L534.62,160.72c-10.05-17.41-35.18-17.41-45.23,0L98.84,837.16c-10.05,17.41,2.51,39.17,22.62,39.17h781.09c20.1,0,32.67-21.76,22.62-39.17ZM565.3,408.08l-6.81,150.95c0,54.15-93.05,54.15-93.05,0l-6.82-149.08c-1.07-61.06,105.61-62.93,106.68-1.87ZM512,791.2c-35.26,0-63.85-28.59-63.85-63.85s28.59-63.85,63.85-63.85,63.85,28.59,63.85,63.85-28.59,63.85-63.85,63.85Z"/>
	</g>
</svg>