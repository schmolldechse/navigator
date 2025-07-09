<script lang="ts">
	import { client } from "$lib/auth/client";

	const session = client.useSession();
</script>

{#if $session.data}
	<div class="flex items-center gap-x-4">
		<span class="hidden text-xl font-medium md:block">{$session.data.user.username}</span>
		<img src={$session.data.user.image} height={35} width={35} alt="User avatar" />
	</div>
{:else}
	<a class="hover:text-accent cursor-pointer text-xl font-bold transition-colors duration-500" href="/login">SIGN IN</a>
{/if}
