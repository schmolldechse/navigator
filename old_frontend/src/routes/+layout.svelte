<script lang="ts">
	import "$src/app.css";
	import type { LayoutProps } from "./$types";

	let { children }: LayoutProps = $props();
</script>

{@render children()}
