<script lang="ts">
	import { page } from "$app/state";
	import { MetaTags } from "svelte-meta-tags";
</script>

<MetaTags
	title={`Error ${page.status}`}
	description={page.error?.message}
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

<div class="text-center md:text-left mx-12 flex-1 flex justify-center items-center flex-col md:flex-row md:gap-x-4">
	<span class="font-[JetBrains_Mono] text-[8rem] text-text/85 font-extrabold">
		{page.status}
	</span>
	<div class="flex flex-col md:gap-y-2">
		<span class="text-lg md:text-2xl font-semibold">Something went wrong</span>
		<span class="text-text/65">{page.error?.message}</span>
	</div>
</div>
