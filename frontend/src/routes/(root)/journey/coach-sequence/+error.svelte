<script lang="ts">
	import { page } from "$app/state";
</script>

<div class="flex items-center justify-center align-middle">
	<div>
		<p class="text-7xl font-bold">
			{page.status}
		</p>
		<p class="text-2xl font-medium">Whoosh! It seems that our provider did not find any coach sequence for this</p>
	</div>
</div>
