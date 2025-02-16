<script lang="ts">
	import type { PageProps } from "./$types";
	import Sequence from "$components/sequence/Sequence.svelte";
	import { MetaTags } from "svelte-meta-tags";

	let { data }: PageProps = $props();

	const tripReference = (): { referenceId: string; destination: string } => {
		const referenceIds = new Set<string>();
		const destinations = new Set<string>();

		data.sequence?.vehicleGroup?.forEach(({ tripReference: { category, fahrtNr, destination } }) => {
			if (fahrtNr) referenceIds.add(`${category} ${fahrtNr}`);
			if (destination?.name) destinations.add(destination.name);
		});

		return {
			referenceId: Array.from(referenceIds).join(" / "),
			destination: Array.from(destinations).join(" / "),
		};
	};
</script>

<MetaTags
	title={`Coach Sequence for ${tripReference().referenceId}`}
	description={`See the sequence of coaches on the track`}
	openGraph={{
		url: "https://navigator.voldechse.wtf/",
		title: "Navigator",
		siteName: "Navigator",
		description: "The Navigator for your train journeys.",
		images: [
			{
				url: "https://navigator.voldechse.wtf/logo.png",
				width: 1024,
				height: 1024,
				alt: "Navigator Logo"
			}
		]
	}}
/>

<div class="flex flex-[1] justify-center mb-6">
	<Sequence sequence={data.sequence} />
</div>
