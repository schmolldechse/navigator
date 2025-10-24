<script lang="ts">
	import Header from "$lib/components/Header.svelte";
	import type { LayoutProps } from "./$types";

	let { data, children }: LayoutProps = $props();
</script>

<Header navigationItems={data.pages} />
{@render children?.()}
