<script lang="ts">
	import Button from "@lib/components/ui/Button.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import Check from "@lucide/svelte/icons/check";
	import Circle from "@lucide/svelte/icons/circle";
	import Layers3 from "@lucide/svelte/icons/layers-3";
	import MousePointerClick from "@lucide/svelte/icons/mouse-pointer-click";

	type Option = {
		id: string;
		label: string;
		description: string;
	};

	const OPTIONS: Option[] = [
		{
			id: "overview",
			label: "Overview",
			description: "A compact summary state."
		},
		{
			id: "quality",
			label: "Quality",
			description: "A signal-focused dashboard state."
		},
		{
			id: "operations",
			label: "Operations",
			description: "A denser operational state."
		}
	];

	let singleSelected: Option | undefined = $state(OPTIONS[0]);
	let requiredSingleSelected: Option | undefined = $state(OPTIONS[1]);
	let multipleSelected: Option[] = $state([OPTIONS[0], OPTIONS[2]]);
	let disabled = $state(false);
	let singleChange = $state(OPTIONS[0].id);
	let requiredSingleChange = $state(OPTIONS[1].id);
	let multipleChange = $state([OPTIONS[0], OPTIONS[2]].map((option: Option) => option.id).join(", "));

	const reset = () => {
		singleSelected = OPTIONS[0];
		requiredSingleSelected = OPTIONS[1];
		multipleSelected = [OPTIONS[0], OPTIONS[2]];
		singleChange = singleSelected.id;
		requiredSingleChange = requiredSingleSelected.id;
		multipleChange = multipleSelected.map((option: Option) => option.id).join(", ");
		disabled = false;
	};
</script>

<svelte:head>
	<title>ToggleGroup Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">ToggleGroup Test</h1>
		<p class="text-foreground/60 text-sm">Interactive examples for single-value and array-value selection modes.</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-4 lg:grid-cols-[minmax(16rem,22rem)_1fr]">
		<Card class="gap-y-5">
			<div class="flex items-start gap-x-3">
				<MousePointerClick size={20} class="text-accent mt-1 shrink-0" />
				<div>
					<h2 class="text-lg font-semibold">Settings</h2>
					<p class="text-foreground/60 text-sm">Toggle shared state for both preview groups.</p>
				</div>
			</div>

			<label class="flex items-center gap-x-2 text-sm font-semibold">
				<Checkbox bind:checked={disabled} />
				<span>Disabled</span>
			</label>

			<Button mode="secondary" onclick={reset}>Reset examples</Button>
		</Card>

		<div class="grid gap-4">
			<Card class="gap-y-5">
				<div>
					<h2 class="flex items-center gap-x-2 text-lg font-semibold">
						<Circle size={18} class="text-accent" />
						<span>Single</span>
					</h2>
					<p class="text-foreground/60 text-sm">
						Selected: {singleSelected?.id ?? "none"} - Callback: {singleChange}
					</p>
				</div>

				<ToggleGroup
					mode="single"
					bind:selected={singleSelected}
					keyFn={(option: Option) => option.id}
					onselect={(option: Option | undefined) => (singleChange = option?.id ?? "none")}
					class="gap-2"
				>
					{#each OPTIONS as option (option.id)}
						<ToggleGroupItem
							item={option}
							{disabled}
							title={option.description}
							class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground flex items-center gap-x-2 rounded-lg px-3 py-2 text-sm font-semibold transition-colors"
						>
							<span>{option.label}</span>
						</ToggleGroupItem>
					{/each}
				</ToggleGroup>
			</Card>

			<Card class="gap-y-5">
				<div>
					<h2 class="flex items-center gap-x-2 text-lg font-semibold">
						<Check size={18} class="text-accent" />
						<span>Required single</span>
					</h2>
					<p class="text-foreground/60 text-sm">
						Selected: {requiredSingleSelected?.id ?? "none"} - Callback: {requiredSingleChange}
					</p>
				</div>

				<ToggleGroup
					mode="single"
					allowEmpty={false}
					bind:selected={requiredSingleSelected}
					keyFn={(option: Option) => option.id}
					onselect={(option: Option | undefined) => (requiredSingleChange = option?.id ?? "none")}
					class="gap-2"
				>
					{#each OPTIONS as option (option.id)}
						<ToggleGroupItem
							item={option}
							{disabled}
							title={option.description}
							class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground flex items-center gap-x-2 rounded-lg px-3 py-2 text-sm font-semibold transition-colors"
						>
							<span>{option.label}</span>
						</ToggleGroupItem>
					{/each}
				</ToggleGroup>
			</Card>

			<Card class="gap-y-5">
				<div>
					<h2 class="flex items-center gap-x-2 text-lg font-semibold">
						<Layers3 size={18} class="text-accent" />
						<span>Multiple</span>
					</h2>
					<p class="text-foreground/60 text-sm">
						Selected: {multipleSelected.length ? multipleSelected.map((option: Option) => option.id).join(", ") : "none"}
						- Callback: {multipleChange || "none"}
					</p>
				</div>

				<ToggleGroup
					mode="multiple"
					bind:selected={multipleSelected}
					keyFn={(option: Option) => option.id}
					onselect={(options: Option[]) => (multipleChange = options.map((option: Option) => option.id).join(", "))}
					class="gap-2"
				>
					{#each OPTIONS as option (option.id)}
						<ToggleGroupItem
							item={option}
							{disabled}
							title={option.description}
							class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground flex items-center gap-x-2 rounded-lg px-3 py-2 text-sm font-semibold transition-colors"
						>
							<span>{option.label}</span>
						</ToggleGroupItem>
					{/each}
				</ToggleGroup>
			</Card>
		</div>
	</section>
</main>
