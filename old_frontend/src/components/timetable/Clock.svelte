<script lang="ts">
	import { DateTime } from "luxon";
	import { onMount } from "svelte";

	let date = $state(DateTime.local());
	let show = $derived(date.second % 2 === 0);

	onMount(() => {
		const interval = setInterval(() => (date = DateTime.local()), 1000);
		return () => clearInterval(interval);
	});
</script>

<span class="text-2xl font-medium whitespace-nowrap md:text-4xl">
	{date.toFormat("HH")}
	<span class:invisible={!show}>:</span>
	{date.toFormat("mm")}
</span>
