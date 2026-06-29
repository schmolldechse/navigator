<script lang="ts">
	import Checkbox from "@lib/components/ui/Checkbox.svelte";
	import Button from "@lib/components/ui/Button.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";

	let unchecked = $state(false);
	let checkedExample = $state(true);
	let mixedChecked = $state(false);
	let mixedIndeterminate = $state(true);

	let disabledUnchecked = $state(false);
	let disabledChecked = $state(true);
	let disabledMixedChecked = $state(false);
	let disabledMixedIndeterminate = $state(true);

	let notifications = $state(true);
	let routeWarnings = $state(false);
	let maintenanceWindows = $state(false);
	let maintenanceMixed = $state(true);
	let callbackChecked = $state(false);
	let lastChange = $state("none");

	const resetShowcase = () => {
		unchecked = false;
		checkedExample = true;
		mixedChecked = false;
		mixedIndeterminate = true;
		disabledUnchecked = false;
		disabledChecked = true;
		disabledMixedChecked = false;
		disabledMixedIndeterminate = true;
		notifications = true;
		routeWarnings = false;
		maintenanceWindows = false;
		maintenanceMixed = true;
		callbackChecked = false;
		lastChange = "none";
	};
</script>

<svelte:head>
	<title>Checkbox Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">Checkbox Test</h1>
		<p class="text-foreground/60 text-sm">Visual states for the shared checkbox primitive.</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-4 lg:grid-cols-2">
		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Enabled</h2>
				<p class="text-foreground/60 text-sm">Unchecked, checked, and mixed states.</p>
			</div>

			<div class="grid gap-3">
				<label
					class="border-border hover:bg-secondary/50 flex items-center gap-3 rounded-lg border p-3 text-sm font-medium transition-colors"
					for="unchecked"
				>
					<Checkbox bind:checked={unchecked} aria-label="Unchecked example" id="unchecked" />
					<span>Unchecked</span>
				</label>

				<label
					class="border-border hover:bg-secondary/50 flex items-center gap-3 rounded-lg border p-3 text-sm font-medium transition-colors"
					for="checked"
				>
					<Checkbox bind:checked={checkedExample} aria-label="Checked example" id="checked" />
					<span>Checked</span>
				</label>

				<label
					class="border-border hover:bg-secondary/50 flex items-center gap-3 rounded-lg border p-3 text-sm font-medium transition-colors"
					for="mixed"
				>
					<Checkbox bind:checked={mixedChecked} bind:indeterminate={mixedIndeterminate} aria-label="Mixed example" id="mixed" />
					<span>Mixed</span>
				</label>

				<div class="flex flex-wrap gap-2">
					<Button
						mode="secondary"
						onclick={() => {
							mixedChecked = false;
							mixedIndeterminate = true;
						}}
					>
						Set mixed
					</Button>
					<Button
						mode="tertiary"
						onclick={() => {
							mixedIndeterminate = false;
						}}
					>
						Clear mixed
					</Button>
				</div>
			</div>
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Disabled</h2>
				<p class="text-foreground/60 text-sm">The same states without interaction.</p>
			</div>

			<div class="grid gap-3">
				<label class="border-border flex items-center gap-3 rounded-lg border p-3 text-sm font-medium" for="disabled-unchecked">
					<Checkbox bind:checked={disabledUnchecked} disabled aria-label="Disabled unchecked example" id="disabled-unchecked" />
					<span class="text-foreground/60">Disabled unchecked</span>
				</label>

				<label class="border-border flex items-center gap-3 rounded-lg border p-3 text-sm font-medium" for="disabled-checked">
					<Checkbox bind:checked={disabledChecked} disabled aria-label="Disabled checked example" id="disabled-checked" />
					<span class="text-foreground/60">Disabled checked</span>
				</label>

				<label class="border-border flex items-center gap-3 rounded-lg border p-3 text-sm font-medium" for="disabled-mixed">
					<Checkbox
						bind:checked={disabledMixedChecked}
						bind:indeterminate={disabledMixedIndeterminate}
						disabled
						aria-label="Disabled mixed example"
						id="disabled-mixed"
					/>
					<span class="text-foreground/60">Disabled mixed</span>
				</label>
			</div>
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Form Rows</h2>
				<p class="text-foreground/60 text-sm">Compact label composition used by settings panels.</p>
			</div>

			<div class="grid gap-3">
				<label class="flex items-center gap-3 text-sm font-medium" for="notifications">
					<Checkbox id="notifications" name="notifications" bind:checked={notifications} />
					<span>Notifications</span>
				</label>

				<label class="flex items-center gap-3 text-sm font-medium" for="route-warnings">
					<Checkbox id="route-warnings" name="routeWarnings" bind:checked={routeWarnings} required />
					<span>Route warnings</span>
				</label>

				<label class="flex items-center gap-3 text-sm font-medium" for="maintenance">
					<Checkbox
						id="maintenance"
						name="maintenance"
						bind:checked={maintenanceWindows}
						bind:indeterminate={maintenanceMixed}
					/>
					<span>Maintenance windows</span>
				</label>
			</div>
		</Card>

		<Card class="gap-y-4">
			<div>
				<h2 class="text-lg font-semibold">Change Callback</h2>
				<p class="text-foreground/60 text-sm">Last change: {lastChange}</p>
			</div>

			<div class="flex flex-col gap-4">
				<label class="flex items-center gap-3 text-sm font-medium" for="callback-checkbox">
					<Checkbox
						bind:checked={callbackChecked}
						onchecked={(value) => (lastChange = value ? "checked" : "unchecked")}
						aria-label="Callback example"
						id="callback-checkbox"
					/>
					<span>Callback state</span>
				</label>

				<div class="flex flex-wrap gap-2">
					<Button mode="secondary" onclick={resetShowcase}>Reset</Button>
				</div>
			</div>
		</Card>
	</section>
</main>
