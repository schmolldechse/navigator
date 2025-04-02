<script lang="ts">
	import type { Time } from "$models/time";

	let {
		time,
		class: className = "",
		changeClass = "",
		direction = "row"
	}: {
		time?: Time;
		class?: string;
		changeClass?: string;
		direction?: string;
	} = $props();

	const samePlatform = $state(time?.plannedPlatform === time?.actualPlatform);
</script>

{#if direction === "row"}
	<span
		class={[{ "bg-text text-background px-2 py-1 md:px-0": !samePlatform },
			{ [changeClass]: !samePlatform },
			className
		]}
	>
		{!samePlatform ? time?.actualPlatform : time?.plannedPlatform}
	</span>
{:else}
	<div class={["flex flex-col items-end", className]}>
		<span>{time?.plannedPlatform ?? ""}</span>
		{#if !samePlatform}
			<span class="bg-text text-background w-fit px-2 font-bold">{time?.actualPlatform ?? ""}</span>
		{/if}
	</div>
{/if}
