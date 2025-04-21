<script lang="ts">
	import Navbar from "$components/Navbar.svelte";
	import type { LayoutProps } from "./$types";
	import { DateTime } from "luxon";
	import GitHub from "$components/ui/icons/GitHub.svelte";
	import { MetaTags } from "svelte-meta-tags";
	import ServerCog from "lucide-svelte/icons/server-cog";

	let { children }: LayoutProps = $props();
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

<div class="relative flex min-h-screen w-full flex-col">
	<Navbar dropdownEnabled={true} />

	<div class="flex flex-1 items-center justify-center">
		{@render children()}
	</div>

	<footer
		class="bg-primary-darker sticky bottom-0 flex w-full flex-row items-center justify-between p-4 text-xs md:text-base"
	>
		<span>&copy; {DateTime.now().year} - Schmolldechse & Contributors</span>

		<div class="flex flex-row items-center justify-center gap-4">
			<a class="flex flex-row items-center gap-2" href="/api-docs" title="API Docs">
				<ServerCog size="24" class="hover:stroke-accent transition-colors duration-200" />
			</a>

			<a
				class="flex flex-row items-center gap-2"
				href="https://github.com/schmolldechse/navigator"
				title="GitHub Repository"
			>
				GitHub
				<GitHub width="24px" height="24px" />
			</a>
		</div>
	</footer>
</div>
