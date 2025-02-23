<script lang="ts">
	import VisualIdentity from "$components/branding/VisualIdentity.svelte";
	import UserInfo from "$components/auth/UserInfo.svelte";

	let { userInfo = true }: { userInfo?: boolean } = $props();
</script>

<div class="mx-2 mt-4 flex items-center justify-between pr-4">
	<VisualIdentity />

	{#if userInfo}
		<UserInfo />
	{/if}
</div>