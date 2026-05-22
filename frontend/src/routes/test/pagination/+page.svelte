<script lang="ts">
	import Card from "@lib/components/ui/card/Card.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import Input from "@lib/components/ui/Input.svelte";
	import Pagination from "@lib/components/ui/Pagination.svelte";

	let offset = $state(0);
	let totalItems = $state(218);
	let limit = $state(10);
	let disabled = $state(false);

	const normalizedLimit = $derived(Math.max(1, Number(limit) || 1));
	const normalizedTotalItems = $derived(Math.max(0, Number(totalItems) || 0));
	const totalPages = $derived(Math.ceil(normalizedTotalItems / normalizedLimit));
	const maxOffset = $derived(Math.max(0, (Math.max(totalPages, 1) - 1) * normalizedLimit));
	const currentOffset = $derived(Math.min(offset, maxOffset));
	const hasMore = $derived(currentOffset + normalizedLimit < normalizedTotalItems);

	const setOffset = (nextOffset: number) => {
		offset = Math.min(Math.max(0, nextOffset), maxOffset);
	};

	const setTotalItems = (value: string | number) => {
		totalItems = Number(value) || 0;
		setOffset(offset);
	};

	const setLimit = (value: string | number) => {
		limit = Math.max(1, Number(value) || 1);
		setOffset(0);
	};
</script>

<svelte:head>
	<title>Pagination Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">Pagination Test</h1>
		<p class="text-foreground/60 text-sm">Interactive controls for page count, page size, ellipses, and disabled states.</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-4 lg:grid-cols-[minmax(18rem,24rem)_1fr]">
		<Card class="gap-y-5">
			<div>
				<h2 class="text-lg font-semibold">Settings</h2>
				<p class="text-foreground/60 text-sm">Adjust the same numeric props used by data-backed ranking pages.</p>
			</div>

			<div class="grid gap-4">
				<label class="grid gap-1.5 text-sm font-semibold">
					<span>Total items</span>
					<Input type="number" value={totalItems} min={0} step={1} onchange={setTotalItems} />
				</label>

				<label class="grid gap-1.5 text-sm font-semibold">
					<span>Page size</span>
					<Input type="number" value={limit} min={1} max={100} step={1} onchange={setLimit} />
				</label>

				<label class="grid gap-1.5 text-sm font-semibold">
					<span>Offset</span>
					<Input
						type="number"
						value={currentOffset}
						min={0}
						max={maxOffset}
						step={normalizedLimit}
						onchange={(value) => setOffset(Number(value) || 0)}
					/>
				</label>

				<label class="flex items-center gap-x-2 text-sm font-semibold">
					<Checkbox bind:checked={disabled} />
					<span>Disabled</span>
				</label>
			</div>
		</Card>

		<Card class="gap-y-8">
			<div>
				<h2 class="text-lg font-semibold">Preview</h2>
				<p class="text-foreground/60 text-sm">
					{normalizedTotalItems.toLocaleString()} items across {Math.max(totalPages, 1).toLocaleString()} pages.
				</p>
			</div>

			<Pagination
				offset={currentOffset}
				limit={normalizedLimit}
				totalItems={normalizedTotalItems}
				{totalPages}
				{hasMore}
				{disabled}
				onpagechange={setOffset}
			/>
		</Card>
	</section>
</main>
