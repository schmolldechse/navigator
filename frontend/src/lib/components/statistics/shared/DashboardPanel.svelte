<script lang="ts">
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import type { LucideIcon } from "@lucide/svelte";
	import Card from "@lib/components/ui/card/Card.svelte";

	type Props = {
		title: string;
		description?: string;
		icon?: LucideIcon;
		actions?: Snippet;
		children: Snippet;
		class?: ClassValue;
	};

	let { title, description, icon: Icon, actions, children, class: className }: Props = $props();
</script>

<Card class={["gap-y-4", className]}>
	<div class="flex flex-col gap-3">
		<div class="min-w-0">
			<div class="flex items-center gap-2">
				{#if Icon}
					<Icon size={20} class="text-accent shrink-0" />
				{/if}
				<h2 class="text-foreground text-lg font-semibold">{title}</h2>
			</div>
			{#if description}
				<p class="text-foreground/60 mt-1 max-w-3xl text-sm leading-relaxed">{description}</p>
			{/if}
		</div>

		{#if actions}
			<div class="flex flex-wrap items-stretch gap-x-3 gap-y-3">
				{@render actions()}
			</div>
		{/if}
	</div>

	{@render children()}
</Card>
