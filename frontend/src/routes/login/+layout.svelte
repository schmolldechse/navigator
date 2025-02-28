<script lang="ts">
	import "$src/app.css";
	import type { LayoutProps } from "./$types";
	import { MetaTags } from "svelte-meta-tags";
	import LogoIcon from "$components/branding/LogoIcon.svelte";

	let { children }: LayoutProps = $props();
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

<div class="flex flex-col h-screen container mx-auto items-center gap-y-4 pt-24 md:pt-48">
	<div class="flex justify-center">
		<LogoIcon height="150" width="150" />
	</div>

	<div class="md:w-[40%] space-y-12">
		<div class="flex flex-col">
			<span class="font-bold text-3xl">Login</span>
			<span>Please login to continue.</span>
		</div>

		{@render children()}
	</div>
</div>
