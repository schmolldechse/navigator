<script lang="ts">
	import type {Time} from "$models/time";
	let {time}: { time?: Time } = $props();

	const samePlatform = $state(time?.plannedPlatform === time?.actualPlatform);
</script>

<span class={[
        "md:w-full",
        !samePlatform && "bg-text text-background px-2 md:px-0 font-bold"
    ]}
>
    {samePlatform ? time?.plannedPlatform : time?.actualPlatform}
</span>