<script lang="ts">
	import { formatDuration } from "$lib";
	import type { Time } from "$models/time";
	import Walking from "$components/ui/icons/Walking.svelte";

	let { arrival, departure }: { arrival?: Time; departure?: Time } = $props();
</script>

<div class="flex min-h-24 flex-row text-base">
	<span class="text-text/65 basis-1/5 self-center text-right">{formatDuration(departure, arrival)}</span>
	<div class="relative flex basis-[15%] justify-center md:basis-[5%]">
		<span
			class="absolute inset-y-4 z-0 h-full w-[4px] self-center bg-[repeating-linear-gradient(0deg,_#9ca3af_0px,_#9ca3af_2px,_transparent_0px,_transparent_5px)]"
		></span>
	</div>
	<span class="flex basis-3/5 items-center self-center">
		<Walking height="35px" width="35px" class="stroke-accent" />
		Changeover
	</span>
	<span class="basis-1/5"></span>
</div>
