<script lang="ts">
</script>

{#snippet renderRow(classes: string)}
	<span class={["block animate-pulse rounded-2xl bg-stone-200/45 px-6 py-2 text-2xl font-bold", classes]}></span>
{/snippet}

<div class="container mx-auto">
	<!-- Separator for smaller screens -->
	<div class="flex flex-col gap-y-2 md:hidden">
		<!-- 1st row -->
		{@render renderRow("w-1/4 h-[20px]")}

		<!-- 2nd row -->
		{@render renderRow("w-1/3 h-[20px]")}

		<!-- 3rd row -->
		{@render renderRow("w-full h-[20px]")}

		<!-- 4th row -->
		{@render renderRow("w-2/4 h-[20px]")}
	</div>

	<!-- Separator for larger screens -->
	<div class="hidden flex-col gap-y-2 md:flex">
		<!-- 1st row -->
		<div class="flex flex-row items-center">
			<div class="mr-8 flex-1"></div>
			<div class="mr-4 flex-4">
				{@render renderRow("w-1/4 h-[20px]")}
			</div>
			<div class="flex-1"></div>
		</div>

		<!-- 2nd row -->
		<div class="flex flex-row items-center">
			<div class="mr-8 flex-1">
				{@render renderRow("w-1/3 h-[30px]")}
			</div>
			<div class="mr-4 flex-4">
				{@render renderRow("w-full h-[30px]")}
			</div>
			<div class="flex-1"></div>
		</div>

		<!-- 3rd row -->
		<div class="flex flex-row items-center">
			<div class="mr-8 flex-1">
				{@render renderRow("w-1/2 h-[30px]")}
			</div>
			<div class="mr-4 flex-4">
				{@render renderRow("w-1/4 h-[30px]")}
			</div>
			<div class="flex-1">
				{@render renderRow("w-1/2 h-[30px]")}
			</div>
		</div>
	</div>
</div>
