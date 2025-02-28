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

<div class="container mx-auto flex h-screen flex-col items-center gap-y-4 pt-24 md:pt-48">
	<div class="flex justify-center">
		<LogoIcon height="150" width="150" />
	</div>

	<div class="space-y-12 md:w-[40%]">
		<div class="flex flex-col">
			<span class="text-3xl font-bold">Login</span>
			<span>Please login to continue.</span>
		</div>

		{@render children()}
	</div>
</div>
