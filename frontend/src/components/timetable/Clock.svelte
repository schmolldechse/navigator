<script lang="ts">
	import { onMount } from "svelte";
	import { DateTime } from "luxon";

	let date = $state(DateTime.local());
	let show = $state(true);

	onMount(() => {
		const interval = setInterval(() => {
			show = !show;
			date = DateTime.local();
		}, 1000);

		return () => clearInterval(interval);
	});
</script>

<span class="text-2xl font-medium md:px-4 md:text-4xl">
	{date.toFormat("HH")}
	<span class:invisible={!show}>:</span>
	{date.toFormat("mm")}
</span>
