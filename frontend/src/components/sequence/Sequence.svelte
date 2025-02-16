<script lang="ts">
	import type { Sequence, Vehicle } from "$models/sequence";
	import Locomotive from "$components/sequence/coaches/Locomotive.svelte";
	import CoachStart from "$components/sequence/coaches/CoachStart.svelte";
	import CoachEnd from "$components/sequence/coaches/CoachEnd.svelte";
	import CoachMiddle from "$components/sequence/coaches/CoachMiddle.svelte";
	import ControlcarStart from "$components/sequence/coaches/ControlcarStart.svelte";
	import ControlcarEnd from "$components/sequence/coaches/ControlcarEnd.svelte";

	let { sequence }: { sequence: Sequence } = $props();
	let vehicle = $state<Vehicle>(sequence.vehicleGroup![0].vehicles[0]);

	const trackLength = $derived(sequence.track.end.position - sequence.track.start.position);

	const calculateLength = (start: number, end: number) => {
		const width = ((end - start) / trackLength) * 100;
		const left = (start / trackLength) * 100;

		return {
			width: `${width}%`,
			left: `${left}%`,
			// ensure minimum width for visibility
			minWidth: Math.max(width, 1.5) + "%"
		};
	};

	const flattenVehicles = $derived(sequence.vehicleGroup
			?.flatMap(g => g.vehicles)
			.sort((a, b) => a.positionOnTrack.start.position - b.positionOnTrack.start.position)
		|| []);
	const hasLocomotive = (currentVehicle: Vehicle, direction: "before" | "after") => {
		const index = flattenVehicles.findIndex(v => v === currentVehicle);
		if (index === -1) return false;

		const adjacentIndex = direction === "before" ? index - 1 : index + 1;
		const adjacentVehicle = flattenVehicles[adjacentIndex];

		return adjacentVehicle?.vehicleType.category === "LOCOMOTIVE";
	};
</script>

<div class="overflow-x-auto h-full max-w-full bg-primary-darker content-end rounded-lg pb-4 md:px-16 space-y-4">
	<!-- Track Visualization -->
	<div class="w-full min-w-[800px] relative">
		{#each sequence.track.sections as section}
			{@const style = calculateLength(section.start.position, section.end.position)}
			<div
				class="absolute h-full flex items-center justify-center"
				style="width: {style.width}; left: {style.left}"
			>
				<span class="font-medium">{section.name}</span>
			</div>
		{/each}
	</div>

	<!-- Vehicle Groups Visualization -->
	{#if sequence.vehicleGroup}
		<div class="relative w-full min-w-[800px] h-20">
			{#each sequence.vehicleGroup as group}
				<!-- Render each vehicle in its exact position on the track -->
				{#each group.vehicles as vehicle, index (vehicle)}
					{@const style = calculateLength(
						vehicle.positionOnTrack.start.position,
						vehicle.positionOnTrack.end.position
					)}
					{@const vehicleType = vehicle.vehicleType}
					<div
						class="h-full absolute cursor-pointer hover:scale-110 transition-transform"
						style="width: {style.width}; left: {style.left}; min-width: {style.minWidth}"
					>
						{#if vehicleType.category.includes("LOCOMOTIVE")}
							<Locomotive />
						{:else if hasLocomotive(vehicle, "before")}
							<CoachStart />
						{:else if hasLocomotive(vehicle, "after")}
							<CoachEnd />
						{:else if vehicleType.category.includes("CONTROLCAR")}
							{#if index === 0}
								<ControlcarEnd />
							{:else if index === group.vehicles.length - 1}
								<ControlcarStart />
							{/if}
						{:else}
							<CoachMiddle />
						{/if}
					</div>
				{/each}
			{/each}
		</div>
	{/if}
</div>