<script lang="ts">
	import { page } from "$app/state";
</script>

<div class="flex items-center justify-center align-middle">
	<div>
		<p class="text-7xl font-bold">
			{page.status}
		</p>
		<p class="text-2xl font-medium">This station does not exist. Maybe you want to try a different?</p>
	</div>
</div>
