<script lang="ts">
	import type { Stop } from "$models/station";

	let expanded = $state<boolean>(false);

	let { viaStops }: { viaStops: Stop[] } = $props();
</script>