<script lang="ts">
	import Button from "@lib/components/ui/Button.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Dialog from "@lib/components/ui/dialog/Dialog.svelte";
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import Clock from "@lucide/svelte/icons/clock";
	import Info from "@lucide/svelte/icons/info";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";

	let basicVisible = $state(false);
	let customTitleVisible = $state(false);
	let pickerVisible = $state(false);
	let nonModalVisible = $state(false);
	let lockedVisible = $state(false);
	let sheetVisible = $state(false);

	let closeCount = $state(0);
	let lastClosed = $state("None");
	let lockOutsideClose = $state(false);

	const recordClose = (name: string) => {
		closeCount += 1;
		lastClosed = name;
	};
</script>

<svelte:head>
	<title>Dialog Test - Navigator</title>
</svelte:head>

{#snippet customTitle()}
	<div class="flex items-center gap-x-2">
		<span class="bg-accent text-accent-foreground flex size-8 items-center justify-center rounded-lg">
			<SlidersHorizontal size={18} />
		</span>
		<div>
			<h3 class="text-lg font-semibold">Custom title snippet</h3>
			<p class="text-foreground/60 text-xs">Icon, subtitle, and custom spacing.</p>
		</div>
	</div>
{/snippet}

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">Dialog Test</h1>
		<p class="text-foreground/60 text-sm">Close callbacks: {closeCount}. Last closed: {lastClosed}.</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-4 lg:grid-cols-2">
		<Card class="gap-y-4">
			<div class="flex items-start gap-x-3">
				<Info size={20} class="text-accent mt-1 shrink-0" />
				<div>
					<h2 class="text-lg font-semibold">Basic modal</h2>
					<p class="text-foreground/60 text-sm">String title, action snippet, close button, Escape, and backdrop close.</p>
				</div>
			</div>

			<Button onclick={() => (basicVisible = true)}>Open basic modal</Button>
		</Card>

		<Card class="gap-y-4">
			<div class="flex items-start gap-x-3">
				<SlidersHorizontal size={20} class="text-accent mt-1 shrink-0" />
				<div>
					<h2 class="text-lg font-semibold">Custom title</h2>
					<p class="text-foreground/60 text-sm">Title is rendered from a snippet instead of a string.</p>
				</div>
			</div>

			<Button onclick={() => (customTitleVisible = true)}>Open custom title</Button>
		</Card>

		<Card class="gap-y-4">
			<div class="flex items-start gap-x-3">
				<Clock size={20} class="text-accent mt-1 shrink-0" />
				<div>
					<h2 class="text-lg font-semibold">Headerless picker</h2>
					<p class="text-foreground/60 text-sm">No header, actions, or close button for picker-style content.</p>
				</div>
			</div>

			<Button onclick={() => (pickerVisible = true)}>Open picker dialog</Button>
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Non-modal outside close</h2>
				<p class="text-foreground/60 text-sm">The page remains interactive while the dialog is open.</p>
			</div>

			<Button onclick={() => (nonModalVisible = true)}>Open non-modal</Button>
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Outside close disabled</h2>
				<p class="text-foreground/60 text-sm">Backdrop clicks can be ignored while Escape and the close button still work.</p>
			</div>

			<label class="flex items-center gap-x-2 text-sm">
				<Checkbox bind:checked={lockOutsideClose} />
				<span>Disable outside click close</span>
			</label>

			<Button onclick={() => (lockedVisible = true)}>Open locked backdrop</Button>
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Custom layout</h2>
				<p class="text-foreground/60 text-sm">A wide desktop dialog that becomes a bottom sheet on small screens.</p>
			</div>

			<Button onclick={() => (sheetVisible = true)}>Open sheet layout</Button>
		</Card>
	</section>
</main>

<Dialog
	bind:isVisible={basicVisible}
	title="Basic modal"
	onclose={() => recordClose("Basic modal")}
	class="w-[calc(100vw-2rem)] max-w-md"
>
	<p class="text-foreground/70 text-sm leading-relaxed">
		This dialog uses the default header and close button. Click outside, press Escape, or use the close icon to trigger the
		close callback.
	</p>

	{#snippet actions()}
		<div class="flex justify-end gap-x-2">
			<Button mode="tertiary" onclick={() => (basicVisible = false)}>Cancel</Button>
			<Button onclick={() => (basicVisible = false)}>Done</Button>
		</div>
	{/snippet}
</Dialog>

<Dialog
	bind:isVisible={customTitleVisible}
	title={customTitle}
	onclose={() => recordClose("Custom title")}
	class="w-[calc(100vw-2rem)] max-w-lg"
>
	<div class="border-border bg-secondary/30 grid gap-3 rounded-lg border p-3">
		<p class="text-sm leading-relaxed">
			The header label is supplied by the caller, while the dialog still owns close behavior and accessibility wiring.
		</p>
		<div class="grid grid-cols-2 gap-2">
			<Button mode="secondary">Secondary</Button>
			<Button>Primary</Button>
		</div>
	</div>
</Dialog>

<Dialog
	bind:isVisible={pickerVisible}
	showHeader={false}
	showActions={false}
	onclose={() => recordClose("Headerless picker")}
	class="w-[calc(100vw-2rem)] max-w-xs"
>
	<div class="grid gap-2">
		{#each ["Now", "In 15 minutes", "In 30 minutes", "Tomorrow"] as option}
			<Button mode="tertiary" onclick={() => (pickerVisible = false)} class="justify-start text-left">{option}</Button>
		{/each}
	</div>
</Dialog>

<Dialog
	bind:isVisible={nonModalVisible}
	title="Non-modal dialog"
	isModal={false}
	showActions={false}
	onclose={() => recordClose("Non-modal dialog")}
	class="fixed top-24 right-4 m-0 w-[calc(100vw-2rem)] max-w-sm"
>
	<p class="text-foreground/70 text-sm leading-relaxed">
		This dialog uses <code>show()</code>, so the surrounding page remains available. Click anywhere outside it to close.
	</p>
</Dialog>

<Dialog
	bind:isVisible={lockedVisible}
	title="Outside close disabled"
	clickOutsideToClose={!lockOutsideClose}
	onclose={() => recordClose("Outside close disabled")}
	class="w-[calc(100vw-2rem)] max-w-md"
>
	<p class="text-foreground/70 text-sm leading-relaxed">
		Outside click close is currently {lockOutsideClose ? "disabled" : "enabled"}. The close button and Escape still close the
		dialog.
	</p>
</Dialog>

<Dialog
	bind:isVisible={sheetVisible}
	title="Responsive sheet"
	onclose={() => recordClose("Responsive sheet")}
	class={[
		"w-screen max-w-full rounded-b-none border-x-0 border-b-0 sm:w-[calc(100vw-2rem)] sm:max-w-3xl sm:rounded-xl sm:border-2",
		"mt-auto sm:mt-0"
	]}
>
	<div class="grid gap-3 sm:grid-cols-3">
		{#each ["Dense controls", "Scrollable content", "Custom sizing"] as label}
			<section class="border-border bg-secondary/30 rounded-lg border p-3">
				<h4 class="text-sm font-semibold">{label}</h4>
				<p class="text-foreground/60 mt-2 text-xs leading-relaxed">
					Caller classes can reshape the same dialog primitive without changing its behavior.
				</p>
			</section>
		{/each}
	</div>
</Dialog>
