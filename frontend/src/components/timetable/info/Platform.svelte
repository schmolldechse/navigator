<script lang="ts">
	import type { Time } from "$models/time";
	let { time }: { time?: Time } = $props();

	const samePlatform = $state(time?.plannedPlatform === time?.actualPlatform);
</script>

<span class="md:w-full"
	  class:bg-text={!samePlatform}
	  class:text-background={!samePlatform}
	  class:px-2={!samePlatform}
	  class:py-1={!samePlatform}
	  class:md:px-0={!samePlatform}
>
    {!samePlatform ? time?.actualPlatform : time?.plannedPlatform}
</span>