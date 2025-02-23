<script lang="ts">
	import VisualIdentity from "$components/branding/VisualIdentity.svelte";
	import UserInfo from "$components/auth/UserInfo.svelte";

	let { userInfo = true }: { userInfo?: boolean } = $props();
</script>

<div class="px-2 pt-4 flex items-center justify-between pr-4 sticky top-0 w-full bg-background h-20">
	<VisualIdentity />

	{#if userInfo}
		<UserInfo />
	{/if}
</div>
