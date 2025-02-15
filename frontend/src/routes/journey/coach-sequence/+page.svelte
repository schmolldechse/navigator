<script lang="ts">
	import type { PageProps } from "./$types";
	import Sequence from "$components/sequence/Sequence.svelte";
	import { MetaTags } from "svelte-meta-tags";

	let { data }: PageProps = $props();
</script>

<MetaTags
	title={"Coach Sequence"}
	description={`See the sequence of coaches on the track`}
	openGraph={{
		url: "https://navigator.voldechse.wtf/",
		title: "Navigator",
		siteName: "Navigator",
		description: "The Navigator for your train journeys.",
		images: [
			{
				url: "https://navigator.voldechse.wtf/logo.png",
				width: 1024,
				height: 1024,
				alt: "Navigator Logo"
			}
		]
	}}
/>

<div class="flex flex-[1] justify-center mb-6">
	<Sequence sequence={data.sequence} />
</div>
