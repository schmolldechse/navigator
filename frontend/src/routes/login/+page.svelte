<script lang="ts">
	import GitHub from "$components/ui/icons/GitHub.svelte";
	import { authClient } from "$lib/auth-client";
	import { MetaTags } from "svelte-meta-tags";
</script>

<MetaTags
	title="Navigator"
	description="The Navigator for your train journeys."
	openGraph={{
		url: "https://navigator.voldechse.wtf",
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

<div
	class="bg-primary-darker mb-8 flex w-full flex-1 flex-col items-center justify-center self-center rounded-3xl p-4 md:w-2/4"
>
	<button
		class="bg-primary-dark hover:bg-primary flex flex-row items-center gap-x-4 rounded-l p-4"
		onclick={() => authClient.signIn.social({ provider: "github", callbackURL: window.location.origin })}
	>
		<GitHub height="35px" width="35px" />
		<span class="text-xl font-medium">Continue with GitHub</span>
	</button>
</div>
