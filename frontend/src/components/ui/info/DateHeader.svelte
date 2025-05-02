<script lang="ts">
	import type { DateTime } from "luxon";

	let { date }: { date: DateTime } = $props();
</script>

<div class="m-4 lg:mx-8">
	<span class="border-text/50 text-text block w-full border-b text-lg md:text-xl">
		{date.toLocaleString({ weekday: "short", day: "numeric", month: "short", year: "numeric" })}
	</span>
</div>
