<script lang="ts">
	import { authClient } from "$lib/auth-client";
	import { goto } from "$app/navigation";

	const session = authClient.useSession();
</script>

{#if $session.data}
	<div class="flex items-center gap-x-4">
		<span class="hidden text-xl font-medium md:block">{$session.data.user.username}</span>
		<img src={$session.data.user.image} height={30} width={30} class="md:h-12 md:w-12" alt="User avatar" />
	</div>
{:else}
	<button
		class="hover:text-accent cursor-pointer text-xl font-bold transition-colors duration-500"
		onclick={async () => await goto("/login")}>SIGN IN</button
	>
{/if}
