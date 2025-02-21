<script lang="ts">
	import "$src/app.css";
	import Navbar from "$components/Navbar.svelte";
	import Logo from "$components/Logo.svelte";
	import { setContext } from "svelte";
	import type { LayoutProps } from "./$types";
	import { authClient } from "$lib/auth-client";

	let { data, children }: LayoutProps = $props();

	const session = authClient.useSession();

	const signIn = async () => {
		await authClient.signIn.social({
			provider: "github",
			callbackURL: window.location.href
		});
	}

	let type = $state("timetable");
	setContext("type", () => type);
</script>

<div class="flex h-screen w-screen flex-col overflow-hidden">
	<!-- Logo/ Navbar (Timetables / Route Planner) -->
	<div class="m-4 flex items-center justify-between pr-4">
		<a href="/">
			<div class="flex cursor-pointer flex-row items-center">
				<Logo />
				<h1 class="relative top-[-0.25rem] hidden text-[2rem] font-bold md:inline">NAVIGATOR</h1>
			</div>
		</a>

		<button onclick={signIn}>
			Continue with github
		</button>

		<Navbar bind:type />
	</div>

	<!-- TimetableSearch/ Route Planner -->
	{@render children()}
</div>
