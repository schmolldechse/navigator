<script>
	import { page } from "$app/state";
</script>

<div class="container mx-auto flex min-h-[calc(100vh-80px)] items-center justify-center px-4">
	<div class="flex flex-col items-center justify-center text-center md:flex-row md:gap-x-8 md:text-left">
		<span class="text-accent/85 text-[8rem] font-extrabold">
			{page.status}
		</span>
		<div class="flex flex-col md:gap-y-2">
			<span class="text-lg font-semibold md:text-3xl">Something went wrong</span>
			<span class="text-text/65 text-xl">{page.error?.message}</span>
		</div>
	</div>
</div>
