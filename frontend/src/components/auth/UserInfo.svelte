<script lang="ts">
	import { authClient } from "$lib/auth-client";
	import { goto } from "$app/navigation";

	const session = authClient.useSession();
</script>

{#if $session.data}
	<div class="flex items-center gap-x-4">
		<span class="text-xl font-medium">{$session.data.user.username}</span>
		<img src={$session.data.user.image} height={50} width={50} alt="User avatar" />
	</div>
{:else}
	<button class="text-xl font-bold hover:text-accent transition-colors duration-500" onclick={async () => await goto("/login")}>SIGN IN</button>
{/if}