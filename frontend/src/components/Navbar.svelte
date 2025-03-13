<script lang="ts">
	import VisualIdentity from "$components/branding/VisualIdentity.svelte";
	import UserInfo from "$components/auth/UserInfo.svelte";

	let { userInfo = true, dropdown = false }: { userInfo?: boolean, dropdown?: boolean } = $props();
</script>

<div class="bg-background sticky top-0 flex h-20 w-full items-center justify-between px-2 pt-4 pr-4">
	<VisualIdentity />

	<div class="flex flex-row items-center">
		{#if userInfo}
			<UserInfo />
		{/if}
		{#if dropdown}
			<span>hey!</span>
		{/if}
	</div>
</div>
