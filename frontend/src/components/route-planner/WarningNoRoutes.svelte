<script lang="ts">
	import TriangleAlert from "lucide-svelte/icons/triangle-alert";
</script>

<div class="flex gap-x-4 items-center px-5">
	<TriangleAlert size="45px" class="stroke-accent/85 stroke-2 shrink-0" />
	<div class="flex flex-col">
		<span class="text-xl font-bold tracking-tighter">No Routes Available</span>
		<span class="text-md">There are currently no routes available. Please try to look for an earlier/ later time.</span>
	</div>
</div>