<script lang="ts">
	import Badge from "@lib/components/ui/Badge.svelte";
	import Button from "@lib/components/ui/Button.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import * as DropdownMenu from "@lib/components/ui/dropdown-menu";
	import type { DropdownMenuTriggerChildProps } from "@lib/components/ui/dropdown-menu";
	import Check from "@lucide/svelte/icons/check";
	import ChevronDown from "@lucide/svelte/icons/chevron-down";
	import Keyboard from "@lucide/svelte/icons/keyboard";
	import ListChecks from "@lucide/svelte/icons/list-checks";
	import PanelTopOpen from "@lucide/svelte/icons/panel-top-open";

	const actions = ["Open overview", "Export CSV", "Refresh metrics", "Archive view", "Remove draft"];
	const scrollItems = Array.from({ length: 18 }, (_, index) => `Scrollable item ${index + 1}`);

	let controlledOpen = $state(false);
	let customOpen = $state(false);
	let closeOnOutside = $state(true);
	let disabled = $state(false);
	let selectedAction = $state("None");
	let keyboardSelection = $state("None");
	let customQuery = $state("");

	const select = (value: string) => {
		selectedAction = value;
	};
</script>

{#snippet customTrigger({ props }: DropdownMenuTriggerChildProps)}
	<input
		{...props}
		type="text"
		bind:value={customQuery}
		placeholder="Focus or click for suggestions"
		class={[
			"border-border bg-background w-full rounded-lg border-2 px-3 py-2 text-sm font-semibold outline-none transition-colors",
			"focus:border-accent/70 disabled:cursor-not-allowed disabled:opacity-50"
		]}
	/>
{/snippet}

<svelte:head>
	<title>DropdownMenu Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">DropdownMenu Test</h1>
		<p class="text-foreground/60 text-sm">
			Reusable dropdown examples for controlled state, keyboard navigation, custom triggers and scrolling content.
		</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-4 lg:grid-cols-[minmax(16rem,22rem)_1fr]">
		<Card class="gap-y-5">
			<div class="flex items-start gap-x-3">
				<PanelTopOpen size={20} class="text-accent mt-1 shrink-0" />
				<div>
					<h2 class="text-lg font-semibold">Settings</h2>
					<p class="text-foreground/60 text-sm">Shared toggles for the examples.</p>
				</div>
			</div>

			<label class="flex items-center gap-x-2 text-sm font-semibold">
				<Checkbox bind:checked={disabled} />
				<span>Disable triggers and items</span>
			</label>

			<label class="flex items-center gap-x-2 text-sm font-semibold">
				<Checkbox bind:checked={closeOnOutside} />
				<span>Close on outside click</span>
			</label>

			<div class="flex flex-wrap gap-2">
				<Badge tone={controlledOpen ? "accent" : "neutral"}>Controlled: {controlledOpen ? "open" : "closed"}</Badge>
				<Badge>Selected: {selectedAction}</Badge>
			</div>

			<Button
				mode="secondary"
				onclick={() => {
					controlledOpen = false;
					customOpen = false;
					selectedAction = "None";
					keyboardSelection = "None";
					customQuery = "";
					disabled = false;
					closeOnOutside = true;
				}}
			>
				Reset examples
			</Button>
		</Card>

		<div class="grid gap-4">
			<Card class="gap-y-5">
				<div>
					<h2 class="flex items-center gap-x-2 text-lg font-semibold">
						<ListChecks size={18} class="text-accent" />
						<span>Basic menu</span>
					</h2>
					<p class="text-foreground/60 text-sm">Uncontrolled state, disabled item and close-on-select behavior.</p>
				</div>

				<DropdownMenu.Root {disabled} closeOnInteractOutside={closeOnOutside}>
					<DropdownMenu.Trigger class="inline-flex items-center gap-2">
						Actions
						<ChevronDown size={16} />
					</DropdownMenu.Trigger>

					<DropdownMenu.Content class="w-56">
						<DropdownMenu.Group>
							<DropdownMenu.GroupHeading>Common actions</DropdownMenu.GroupHeading>
							{#each actions.slice(0, 3) as action (action)}
								<DropdownMenu.Item onselect={() => select(action)}>
									<Check size={15} class={selectedAction === action ? "opacity-100" : "opacity-0"} />
									<span>{action}</span>
								</DropdownMenu.Item>
							{/each}
						</DropdownMenu.Group>
						<DropdownMenu.Separator />
						<DropdownMenu.Item disabled onselect={() => select("Disabled")}>Disabled action</DropdownMenu.Item>
						<DropdownMenu.Item onselect={() => select(actions[4])} class="text-destructive enabled:hover:text-destructive">
							{actions[4]}
						</DropdownMenu.Item>
					</DropdownMenu.Content>
				</DropdownMenu.Root>
			</Card>

			<Card class="gap-y-5">
				<div>
					<h2 class="flex items-center gap-x-2 text-lg font-semibold">
						<PanelTopOpen size={18} class="text-accent" />
						<span>Controlled open state</span>
					</h2>
					<p class="text-foreground/60 text-sm">The menu is bound to page state and can be opened externally.</p>
				</div>

				<div class="flex flex-wrap items-center gap-2">
					<Button mode="secondary" onclick={() => (controlledOpen = true)}>Open from page state</Button>
					<DropdownMenu.Root
						bind:open={controlledOpen}
						{disabled}
						closeOnInteractOutside={closeOnOutside}
						onopenchange={(open) => (controlledOpen = open)}
					>
						<DropdownMenu.Trigger class="inline-flex items-center gap-2">
							Controlled menu
							<ChevronDown size={16} />
						</DropdownMenu.Trigger>

						<DropdownMenu.Content align="end" class="w-64">
							{#each actions as action (action)}
								<DropdownMenu.Item onselect={() => select(action)}>{action}</DropdownMenu.Item>
							{/each}
						</DropdownMenu.Content>
					</DropdownMenu.Root>
				</div>
			</Card>

			<Card class="gap-y-5">
				<div>
					<h2 class="flex items-center gap-x-2 text-lg font-semibold">
						<Keyboard size={18} class="text-accent" />
						<span>Custom trigger and keyboard</span>
					</h2>
					<p class="text-foreground/60 text-sm">
						Focus the input, use Arrow keys, Enter, Space and Escape. Last keyboard selection: {keyboardSelection}.
					</p>
				</div>

				<DropdownMenu.Root
					bind:open={customOpen}
					{disabled}
					closeOnInteractOutside={closeOnOutside}
					class="w-full max-w-md"
				>
					<DropdownMenu.Trigger child={customTrigger} openOnFocus clickBehavior="open" class="w-full" />

					<DropdownMenu.Content matchTriggerWidth>
						<DropdownMenu.GroupHeading>Suggestions</DropdownMenu.GroupHeading>
						{#each ["Network", "Station", "Line", "Journey"] as value (value)}
							<DropdownMenu.Item onselect={() => (keyboardSelection = value)}>{value}</DropdownMenu.Item>
						{/each}
					</DropdownMenu.Content>
				</DropdownMenu.Root>
			</Card>

			<Card class="gap-y-5">
				<div>
					<h2 class="text-lg font-semibold">Scrollable content</h2>
					<p class="text-foreground/60 text-sm">The content panel keeps a stable max height and scrolls internally.</p>
				</div>

				<DropdownMenu.Root {disabled} closeOnInteractOutside={closeOnOutside}>
					<DropdownMenu.Trigger class="inline-flex items-center gap-2">
						Scrollable menu
						<ChevronDown size={16} />
					</DropdownMenu.Trigger>

					<DropdownMenu.Content class="w-64">
						{#each scrollItems as item (item)}
							<DropdownMenu.Item onselect={() => select(item)}>{item}</DropdownMenu.Item>
						{/each}
					</DropdownMenu.Content>
				</DropdownMenu.Root>
			</Card>
		</div>
	</section>
</main>
