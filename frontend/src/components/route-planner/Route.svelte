<script lang="ts">
	import type { Route } from "$models/route";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import Minus from "lucide-svelte/icons/minus";

	let { route }: { route: Route } = $props();
</script>

<!-- fix: multiple Route's having the same width for the time info -->
<div class="text-2xl flex flex-row items-start">
	<TimeInformation time={route?.legs[0]?.departure} direction="col" />
	<Minus />
	<TimeInformation time={route?.legs[route?.legs?.length - 1]?.arrival} direction="col" />
</div>