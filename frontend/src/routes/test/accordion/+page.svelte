<script lang="ts">
	import * as Accordion from "@lib/components/ui/accordion";

	let singleValue: string | undefined = $state(undefined);
	let multipleValue: string[] = $state(["routes", "signals"]);
	let disabledRootValue: string | undefined = $state("locked-root");
	let forceMountValue: string | undefined = $state(undefined);
	let hiddenUntilFoundValue: string | undefined = $state(undefined);

	let singleChange = $state("none");
	let multipleChange = $state("routes, signals");
</script>

<svelte:head>
	<title>Accordion Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-8 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">Accordion Test</h1>
		<p class="text-foreground/60 text-sm">
			Static examples for controlled state, disabled items, mounting, and searchable content.
		</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-6 lg:grid-cols-2">
		<div class="border-border bg-background rounded-lg border p-4">
			<div class="mb-3">
				<h2 class="text-lg font-semibold">Single</h2>
				<p class="text-foreground/60 text-sm">Selected: {singleValue ?? "none"} · Changed: {singleChange}</p>
			</div>

			<Accordion.Root
				type="single"
				bind:value={singleValue}
				onchange={(value) => (singleChange = value ?? "none")}
				data-testid="single-accordion"
			>
				<Accordion.Item value="overview">
					<Accordion.Trigger>Operational overview</Accordion.Trigger>
					<Accordion.Content>
						Navigator keeps dense operational panels calm by letting one topic take focus without removing the rest of the
						context.
					</Accordion.Content>
				</Accordion.Item>

				<Accordion.Item value="platforms">
					<Accordion.Trigger>Platform events</Accordion.Trigger>
					<Accordion.Content>
						Arrival, departure, cancellation, and delay details can sit behind a compact row until they are needed.
					</Accordion.Content>
				</Accordion.Item>

				<Accordion.Item value="disabled" disabled>
					<Accordion.Trigger>Disabled service note</Accordion.Trigger>
					<Accordion.Content>This item is disabled and does not react to trigger clicks.</Accordion.Content>
				</Accordion.Item>
			</Accordion.Root>
		</div>

		<div class="border-border bg-background rounded-lg border p-4">
			<div class="mb-3">
				<h2 class="text-lg font-semibold">Multiple</h2>
				<p class="text-foreground/60 text-sm">Selected: {multipleChange}</p>
			</div>

			<Accordion.Root
				type="multiple"
				bind:value={multipleValue}
				onchange={(value) => (multipleChange = value.length ? value.join(", ") : "none")}
				data-testid="multiple-accordion"
			>
				<Accordion.Item value="routes">
					<Accordion.Trigger>Route groups</Accordion.Trigger>
					<Accordion.Content>
						Multiple mode lets related sections remain visible together while independent rows continue to toggle.
					</Accordion.Content>
				</Accordion.Item>

				<Accordion.Item value="signals">
					<Accordion.Trigger>Signal windows</Accordion.Trigger>
					<Accordion.Content>
						Open values are represented as an array so dashboards can store several expanded panels at once.
					</Accordion.Content>
				</Accordion.Item>

				<Accordion.Item>
					<Accordion.Trigger>Generated item value</Accordion.Trigger>
					<Accordion.Content>
						This item omits a value prop, so the component assigns a stable generated value for selection state.
					</Accordion.Content>
				</Accordion.Item>
			</Accordion.Root>
		</div>

		<div class="border-border bg-background rounded-lg border p-4">
			<div class="mb-3">
				<h2 class="text-lg font-semibold">Disabled Root</h2>
				<p class="text-foreground/60 text-sm">All triggers are disabled while the open state remains readable.</p>
			</div>

			<Accordion.Root type="single" bind:value={disabledRootValue} disabled data-testid="disabled-root-accordion">
				<Accordion.Item value="locked-root">
					<Accordion.Trigger>Locked state</Accordion.Trigger>
					<Accordion.Content>
						The item starts open from the controlled value, but the disabled root prevents further interaction.
					</Accordion.Content>
				</Accordion.Item>

				<Accordion.Item value="locked-secondary">
					<Accordion.Trigger>Secondary locked item</Accordion.Trigger>
					<Accordion.Content>This row cannot be opened while the root remains disabled.</Accordion.Content>
				</Accordion.Item>
			</Accordion.Root>
		</div>

		<div class="border-border bg-background rounded-lg border p-4">
			<div class="mb-3">
				<h2 class="text-lg font-semibold">Mounting</h2>
				<p class="text-foreground/60 text-sm">Content can stay mounted while still using the fixed transition.</p>
			</div>

			<Accordion.Root type="single" bind:value={forceMountValue} data-testid="force-mount-accordion">
				<Accordion.Item value="force-mounted">
					<Accordion.Trigger>Force mounted content</Accordion.Trigger>
					<Accordion.Content forceMount>
						This panel remains in the DOM when collapsed, which is useful for content that owns persistent local state.
					</Accordion.Content>
				</Accordion.Item>

				<Accordion.Item value="regular-mounted">
					<Accordion.Trigger>Regular content</Accordion.Trigger>
					<Accordion.Content>
						This panel unmounts after the closing transition, keeping the default DOM footprint small.
					</Accordion.Content>
				</Accordion.Item>
			</Accordion.Root>
		</div>

		<div class="border-border bg-background rounded-lg border p-4 lg:col-span-2">
			<div class="mb-3">
				<h2 class="text-lg font-semibold">Hidden Until Found</h2>
				<p class="text-foreground/60 text-sm">Search for signal-authenticator to let the browser reveal the collapsed row.</p>
			</div>

			<Accordion.Root type="single" bind:value={hiddenUntilFoundValue} data-testid="hidden-until-found-accordion">
				<Accordion.Item value="browser-search">
					<Accordion.Trigger>Browser search target</Accordion.Trigger>
					<Accordion.Content hiddenUntilFound>
						The unique signal-authenticator phrase lives inside collapsed content and should open through the browser
						beforematch event.
					</Accordion.Content>
				</Accordion.Item>

				<Accordion.Item value="visible-copy">
					<Accordion.Trigger>Regular searchable row</Accordion.Trigger>
					<Accordion.Content>
						This item behaves like normal collapsed content and is useful for comparing the hidden-until-found behavior.
					</Accordion.Content>
				</Accordion.Item>
			</Accordion.Root>
		</div>
	</section>
</main>
