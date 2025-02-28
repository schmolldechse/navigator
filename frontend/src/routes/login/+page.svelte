<script lang="ts">
	import GitHub from "$components/ui/icons/GitHub.svelte";
	import { authClient } from "$lib/auth-client";
</script>

<button
	class="w-full bg-primary-darker hover:bg-primary-dark cursor-pointer flex flex-row items-center justify-center gap-x-4 rounded-l p-2"
	onclick={() => authClient.signIn.social({ provider: "github", callbackURL: window.location.origin })}
>
	<GitHub height="25" width="25" />
	<span class="text-lg font-medium">Continue with GitHub</span>
</button>
