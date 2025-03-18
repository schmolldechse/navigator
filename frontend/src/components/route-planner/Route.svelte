<script lang="ts">
	import type { Route } from "$models/route";
	import Minus from "lucide-svelte/icons/minus";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";

	let { route }: { route: Route } = $props();
</script>

<div class="text-2xl font-medium flex flex-row">
	<TimeInformation time={route?.legs[0]?.departure} direction="col" />
	<Minus class="mx-2 mt-[0.25rem]" />
	<TimeInformation time={route?.legs[route?.legs?.length - 1]?.arrival} direction="col" />

	<span>hey</span>
</div>