<script lang="ts">
	import Header from "$lib/components/Header.svelte";
	import type { PageProps } from "./$types";

    let { data }: PageProps = $props();
</script>

<svelte:head>
    <title>{data.pages[0].label ?? 'Navigator'}</title>
</svelte:head>

<Header pageName={data.pages[0].label ?? ''} />