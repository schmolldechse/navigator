<script lang="ts">
	import GitHub from "$components/ui/icons/GitHub.svelte";
	import { authClient } from "$lib/auth-client";
	import { MetaTags } from "svelte-meta-tags";
	import LogoIcon from "$components/branding/LogoIcon.svelte";
</script>

<MetaTags
	title="Navigator"
	description="Login to continue"
	openGraph={{
		url: "https://navigator.voldechse.wtf/login",
		title: "Navigator",
		siteName: "Navigator",
		description: "Login to continue",
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

<div class="container mx-auto flex h-screen flex-col items-center gap-y-4 pt-24 md:pt-48">
	<a class="flex justify-center" href="/">
		<LogoIcon height="150" width="150" />
	</a>

	<div class="space-y-12 md:w-[40%]">
		<div class="flex flex-col">
			<span class="text-3xl font-bold">Login</span>
			<span>Please login to continue.</span>
		</div>

		<button
			class="bg-primary-darker hover:bg-primary-dark flex w-full cursor-pointer flex-row items-center justify-center gap-x-4 rounded-l p-2"
			onclick={() => authClient.signIn.social({ provider: "github", callbackURL: window.location.origin })}
		>
			<GitHub height="25" width="25" />
			<span class="text-lg font-medium">Continue with GitHub</span>
		</button>
	</div>
</div>
