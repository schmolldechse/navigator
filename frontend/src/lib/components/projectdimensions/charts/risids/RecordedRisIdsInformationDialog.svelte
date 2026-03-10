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
	title="Understanding RIS IDs"
>
	<p class="text-muted-foreground text-pretty">
		These IDs are unique digital fingerprints used to track journeys from the RIS (Deutsche Bahn's Reisenden Informations
		System). Navigator's background processes these through an automated lifecycle.
	</p>

	<ul class="flex flex-col gap-y-2 text-sm">
		<li class="text-muted-foreground text-pretty">
			<strong class="text-accent">Active:</strong> the system is "aware" of this ID and constantly polls for latest journeys data.
			It uses incremental timestamps to ensure history is kept perfectly up to date.
		</li>
		<li class="text-muted-foreground text-pretty">
			<strong class="text-accent">Inactive:</strong> to stay efficient and prevent "pointless" requests, the system
			automatically moves stale IDs to an <strong>inactive</strong> state.
		</li>
		<li class="text-muted-foreground text-pretty">
			<strong class="text-accent">Automatic Reactivation:</strong> the system doesn't just forget about inactive IDs. It
			monitors about 500 major german train stations daily. If an "inactive" journey is spotted appearing on a station timetable
			again, the system instantly flips its status back to <strong>active</strong> and resumes tracking.
		</li>
	</ul>
</Dialog>
