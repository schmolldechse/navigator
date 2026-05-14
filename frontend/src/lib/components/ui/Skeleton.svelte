<script lang="ts">
	type Props = {
		class?: string;
	};
	let { class: className }: Props = $props();
</script>

<div class={["bg-foreground/5 animate-pulse rounded-md", className]}></div>
