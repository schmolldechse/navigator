<script lang="ts">
	import ChevronLeft from "@lucide/svelte/icons/chevron-left";
	import ChevronRight from "@lucide/svelte/icons/chevron-right";
	import type { ClassValue } from "svelte/elements";
	import Button from "./Button.svelte";

	type PaginationItem = number | "ellipsis";

	type Props = {
		offset: number;
		limit: number;
		totalItems: number;
		totalPages: number;
		hasMore: boolean;
		disabled?: boolean;
		onpagechange: (offset: number) => void;
		class?: ClassValue;
	};
	let { offset, limit, totalItems, totalPages, hasMore, disabled = false, onpagechange, class: className }: Props = $props();

	const currentPage = $derived(totalItems === 0 ? 0 : Math.floor(offset / limit) + 1);
	const canGoBack = $derived(!disabled && offset > 0);
	const canGoForward = $derived(!disabled && hasMore);
	const fromItem = $derived(totalItems === 0 ? 0 : offset + 1);
	const toItem = $derived(Math.min(offset + limit, totalItems));
	const visibleTotalPages = $derived(Math.max(1, totalPages));

	const pageItems: PaginationItem[] = $derived.by(() => {
		if (visibleTotalPages <= 5) {
			return Array.from({ length: visibleTotalPages }, (_, index) => index + 1);
		}

		if (currentPage <= 3) return [1, 2, 3, 4, "ellipsis", visibleTotalPages];
		if (currentPage >= visibleTotalPages - 2) {
			return [1, "ellipsis", visibleTotalPages - 3, visibleTotalPages - 2, visibleTotalPages - 1, visibleTotalPages];
		}

		return [1, "ellipsis", currentPage, "ellipsis", visibleTotalPages];
	});

	const pageToOffset = (page: number) => Math.max(0, (page - 1) * limit);
</script>

<nav class={["flex flex-col items-center gap-4", className]} aria-label="Pagination">
	<div class="flex items-center justify-center gap-1.5 sm:gap-3">
		<Button
			mode="tertiary"
			disabled={!canGoBack}
			onclick={() => onpagechange(Math.max(0, offset - limit))}
			aria-label="Previous page"
			class="text-foreground/80 flex size-9 items-center justify-center p-0! disabled:opacity-30 sm:size-10"
		>
			<ChevronLeft size={22} />
		</Button>

		<div class="flex items-center justify-center gap-1 sm:gap-2">
			{#each pageItems as item, index (item === "ellipsis" ? `ellipsis-${index}` : item)}
				{#if item === "ellipsis"}
					<span class="text-foreground/45 flex size-9 items-center justify-center text-sm font-semibold sm:size-10">...</span>
				{:else}
					<Button
						mode={item === currentPage ? "primary" : "tertiary"}
						disabled={disabled || item === currentPage || totalItems === 0}
						data-active={item === currentPage ? true : undefined}
						aria-current={item === currentPage ? "page" : undefined}
						aria-label={`Page ${item}`}
						onclick={() => onpagechange(pageToOffset(item))}
						class={[
							"flex size-9 items-center justify-center p-0! text-sm font-semibold tabular-nums sm:size-10",
							item === currentPage && "disabled:opacity-100",
							item !== currentPage && "text-foreground/80 disabled:opacity-40"
						]}
					>
						{item}
					</Button>
				{/if}
			{/each}
		</div>

		<Button
			mode="tertiary"
			disabled={!canGoForward}
			onclick={() => onpagechange(offset + limit)}
			aria-label="Next page"
			class="text-foreground/80 flex size-9 items-center justify-center p-0! disabled:opacity-30 sm:size-10"
		>
			<ChevronRight size={22} />
		</Button>
	</div>

	<p class="text-foreground/45 text-sm font-semibold tabular-nums">
		{#if totalItems === 0}
			No results
		{:else}
			Showing {fromItem.toLocaleString()} - {toItem.toLocaleString()}
		{/if}
	</p>
</nav>
