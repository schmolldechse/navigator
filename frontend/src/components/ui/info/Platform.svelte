<script lang="ts">
	import type { Time } from "$models/time";

	let { time, class: className = "", direction = "row" }: {
		time?: Time,
		class?: string,
		direction?: string
	} = $props();

	const samePlatform = $state(time?.plannedPlatform === time?.actualPlatform);
</script>

{#if direction === "row"}
<span class={["md:w-full", {"bg-text text-background px-2 py-1 md:px-0": !samePlatform}, className]}>
	{!samePlatform ? time?.actualPlatform : time?.plannedPlatform}
</span>
{:else}
	<div class={["flex flex-col md:w-full items-end", className]}>
		<span>{time?.plannedPlatform ?? ""}</span>
		{#if !samePlatform}
			<span class="font-bold w-fit bg-text text-background px-2">{time?.actualPlatform ?? ""}</span>
		{/if}
	</div>
{/if}
