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

<div class="flex-1 self-center bg-primary-darker w-full md:w-2/4 rounded-3xl mb-8 flex flex-col p-4 justify-center items-center">
	<button class="flex flex-row items-center gap-x-4 bg-primary-dark p-4 rounded-l hover:bg-primary"
			onclick={() => authClient.signIn.social({ provider: "github", callbackURL: window.location.origin })}
	>
		<GitHub height="35px" width="35px" />
		<span class="text-xl font-medium">Continue with GitHub</span>
	</button>
</div>
