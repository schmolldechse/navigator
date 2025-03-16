<script lang="ts">
	import type { Route } from "$models/route";
	import TimeComponent from "$components/timetable/info/TimeComponent.svelte";

	let { route }: { route: Route } = $props();
</script>

<div>
	<TimeComponent time={route?.legs[0]?.arrival} />
</div>