<script>
	import TriangleAlert from "lucide-svelte/icons/triangle-alert";
</script>

<div class="flex gap-x-4 items-center px-5">
	<TriangleAlert size="45px" class="stroke-accent/85 stroke-2 shrink-0" />
	<div class="flex flex-col">
		<span class="text-xl font-bold tracking-tighter">No Connections Available</span>
		<span class="text-md">There are currently no connections available. Please check your applied filters or try again at a different time.</span>
	</div>
</div>