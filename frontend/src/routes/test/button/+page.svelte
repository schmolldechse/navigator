<script lang="ts">
	import Button from "@lib/components/ui/Button.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import ArrowRight from "@lucide/svelte/icons/arrow-right";
	import ExternalLink from "@lucide/svelte/icons/external-link";
	import RefreshCw from "@lucide/svelte/icons/refresh-cw";
	import Save from "@lucide/svelte/icons/save";
	import Trash2 from "@lucide/svelte/icons/trash-2";

	let clickCount = $state(0);
	let submitted = $state(false);

	const increment = () => {
		clickCount += 1;
	};

	const submit = (event: SubmitEvent) => {
		event.preventDefault();
		submitted = true;
	};
</script>

<svelte:head>
	<title>Button Test - Navigator</title>
</svelte:head>

<main class="container mx-auto flex min-h-screen flex-col gap-6 p-4 sm:py-8">
	<div class="flex flex-col gap-y-1">
		<h1 class="text-2xl font-medium">Button Test</h1>
		<p class="text-foreground/60 text-sm">
			Examples for modes, disabled state, links, icon composition, custom classes, and form button types.
		</p>
	</div>

	<section class="grid grid-cols-1 items-start gap-4 lg:grid-cols-2">
		<Card class="gap-y-5">
			<div>
				<h2 class="text-lg font-semibold">Modes</h2>
				<p class="text-foreground/60 text-sm">The four visual modes supported by the component.</p>
			</div>

			<div class="flex flex-wrap gap-2">
				<Button mode="primary" onclick={increment}>Primary</Button>
				<Button mode="secondary" onclick={increment}>Secondary</Button>
				<Button mode="tertiary" onclick={increment}>Tertiary</Button>
				<Button mode="destructive" onclick={increment}>Destructive</Button>
			</div>

			<p class="text-foreground/60 text-sm tabular-nums">Clicked {clickCount} times.</p>
		</Card>

		<Card class="gap-y-5">
			<div>
				<h2 class="text-lg font-semibold">Disabled</h2>
				<p class="text-foreground/60 text-sm">Disabled buttons keep layout but remove interaction.</p>
			</div>

			<div class="flex flex-wrap gap-2">
				<Button mode="primary" disabled>Primary</Button>
				<Button mode="secondary" disabled>Secondary</Button>
				<Button mode="tertiary" disabled>Tertiary</Button>
				<Button mode="destructive" disabled>Destructive</Button>
			</div>
		</Card>

		<Card class="gap-y-5">
			<div>
				<h2 class="text-lg font-semibold">Icons</h2>
				<p class="text-foreground/60 text-sm">Callers compose icon and text content directly.</p>
			</div>

			<div class="flex flex-wrap gap-2">
				<Button mode="primary" class="inline-flex items-center gap-x-2">
					<Save size={16} />
					<span>Save</span>
				</Button>
				<Button mode="secondary" class="inline-flex items-center gap-x-2">
					<RefreshCw size={16} />
					<span>Refresh</span>
				</Button>
				<Button mode="tertiary" aria-label="Next" class="inline-flex size-10 items-center justify-center p-0!">
					<ArrowRight size={18} />
				</Button>
				<Button mode="destructive" aria-label="Delete" class="inline-flex size-10 items-center justify-center p-0!">
					<Trash2 size={18} />
				</Button>
			</div>
		</Card>

		<Card class="gap-y-5">
			<div>
				<h2 class="text-lg font-semibold">Links</h2>
				<p class="text-foreground/60 text-sm">Providing <code>href</code> renders the same component as an anchor.</p>
			</div>

			<div class="flex flex-wrap gap-2">
				<Button href="/statistics" mode="primary" class="inline-flex items-center gap-x-2">
					<span>Statistics</span>
					<ExternalLink size={16} />
				</Button>
				<Button href="/test/pagination" mode="secondary" class="inline-flex items-center gap-x-2">
					<span>Pagination test</span>
					<ExternalLink size={16} />
				</Button>
				<Button href="/statistics" mode="secondary" disabled>Disabled link</Button>
			</div>
		</Card>

		<Card class="gap-y-5 lg:col-span-2">
			<div>
				<h2 class="text-lg font-semibold">Types and custom sizing</h2>
				<p class="text-foreground/60 text-sm">Button type is forwarded for form use, and caller classes can adjust sizing.</p>
			</div>

			<form class="flex flex-wrap items-center gap-2" onsubmit={submit}>
				<Button type="submit" mode="primary">Submit form</Button>
				<Button type="reset" mode="secondary" onclick={() => (submitted = false)}>Reset state</Button>
				<Button mode="tertiary" class="px-2! py-1! text-xs">Compact</Button>
				<Button mode="secondary" class="px-5! py-3! text-base">Large secondary</Button>
			</form>

			<p class="text-foreground/60 text-sm">Form submitted: {submitted ? "yes" : "no"}.</p>
		</Card>
	</section>
</main>
