<script lang="ts">
	import type {Time} from "$models/time";
	let {time}: { time?: Time } = $props();

	const samePlatform = $state(time?.plannedPlatform === time?.actualPlatform);
</script>

<span class={[
        "md:w-full",
        !samePlatform && "bg-text text-background px-2 py-1 md:px-0"
    ]}
>
    {samePlatform ? time?.plannedPlatform : time?.actualPlatform}
</span>