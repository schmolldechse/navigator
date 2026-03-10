<script lang="ts">
	import Dialog from "@lib/components/interactable/dialog/Dialog.svelte";

	type Props = {
		isVisible: boolean;
	};
	let { isVisible = $bindable(false) }: Props = $props();
</script>

<Dialog
	bind:isVisible
	isModal
	class={[
		"mx-0 mt-auto mb-0 max-h-[75vh] w-full max-w-full rounded-t-2xl rounded-b-none border-b-0", // mobile: bottom sheet
		"sm:m-auto sm:max-h-fit sm:max-w-2xl sm:rounded-lg sm:border-b-2" // desktop: centered dialog
	]}
	title="Understanding Total Delay (Sum)"
>
	<p class="text-muted-foreground text-pretty">
		The <strong>Total Delay</strong> graph displays the accumulated delay in seconds of all transport types recorded at every single
		stop during this hour.
	</p>

	<div class="flex flex-col gap-y-2">
		<h4 class="text-accent font-semibold tracking-wider uppercase">How to read this chart:</h4>
		<p class="text-muted-foreground text-pretty">
			Delays are recorded every time a vehicle makes a stop. Because of this, lines with many closely spaced stops (like
			suburban trains) add up to much larger totals and have more weight in this view compared to lines with fewer stops (like
			regional or long-distance trains).
		</p>
	</div>

	<div class="border-muted-foreground/20 rounded-lg border p-3">
		<p class="text-muted-foreground text-pretty">
			<span class="text-text font-semibold">For example:</span>
			If a suburban train runs with a constant 5-minute delay and stops at 6 stations in an hour, it adds
			<strong>30 minutes</strong>
			to the graph. However, an express train that is 25 minutes late but only stops at one station in the same hour will only contribute
			<strong>25 minutes</strong> to the total.
		</p>
	</div>

	<p class="text-muted-foreground text-pretty">
		<span class="text-text font-semibold">Tip:</span> To see how long a single passenger typically had to wait at the platform,
		simply switch to the
		<span class="font-semibold text-emerald-500">Average (AVG)</span> view.
	</p>
</Dialog>
