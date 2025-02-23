<script lang="ts">
	import type { Sequence, Vehicle } from "$models/sequence";
	import Locomotive from "$components/sequence/coaches/small/Locomotive.svelte";
	import CoachStart from "$components/sequence/coaches/small/CoachStart.svelte";
	import CoachEnd from "$components/sequence/coaches/small/CoachEnd.svelte";
	import CoachMiddle from "$components/sequence/coaches/small/CoachMiddle.svelte";
	import ControlcarStart from "$components/sequence/coaches/small/ControlcarStart.svelte";
	import ControlcarEnd from "$components/sequence/coaches/small/ControlcarEnd.svelte";
	import VehicleInfo from "$components/sequence/VehicleInfo.svelte";

	let { sequence }: { sequence: Sequence } = $props();
	let selectedVehicle = $state<{ groupIndex: number; vehicleIndex: number }>({ groupIndex: 0, vehicleIndex: 0 });
	let vehicleById = $derived(sequence.vehicleGroup?.[selectedVehicle.groupIndex]?.vehicles[selectedVehicle.vehicleIndex]);

	const selectVehicle = (groupIndex: number, vehicleIndex: number) => {
		const group = sequence?.vehicleGroup?.[groupIndex];
		if (!group) return;

		selectedVehicle = { groupIndex, vehicleIndex };
	};

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

	const flattenVehicles = $derived(
		sequence.vehicleGroup
			?.flatMap((g) => g.vehicles)
			.sort((a, b) => a.positionOnTrack.start.position - b.positionOnTrack.start.position) || []
	);
	const hasLocomotive = (currentVehicle: Vehicle, direction: "before" | "after") => {
		const index = flattenVehicles.findIndex((v) => v === currentVehicle);
		if (index === -1) return false;

		const adjacentIndex = direction === "before" ? index - 1 : index + 1;
		const adjacentVehicle = flattenVehicles[adjacentIndex];

		return adjacentVehicle?.vehicleType.category === "LOCOMOTIVE";
	};

	const tripReference = (): { referenceId: string; destination: string } => {
		const referenceIds = new Set<string>();
		const destinations = new Set<string>();

		sequence?.vehicleGroup?.forEach(({ tripReference: { category, fahrtNr, destination } }) => {
			if (fahrtNr) referenceIds.add(`${category} ${fahrtNr}`);
			if (destination?.name) destinations.add(destination.name);
		});

		return {
			referenceId: Array.from(referenceIds).join(" / "),
			destination: Array.from(destinations).join(" / ")
		};
	};
</script>

<div class="bg-primary-darker flex h-full max-w-full flex-col overflow-x-auto rounded-lg pb-4 md:px-16">
	<span class="pt-8 text-3xl font-bold">Coach Sequence</span>

	<div class="h-full max-w-full content-end">
		<VehicleInfo track={sequence.track} vehicle={vehicleById} />

		<!-- Track Visualization -->
		<div class="relative mt-12 w-full min-w-[800px]">
			{#each sequence.track.sections as section}
				{@const style = calculateLength(section.start.position, section.end.position)}
				<div class="absolute flex h-full items-center justify-center" style="width: {style.width}; left: {style.left}">
					<span class="font-medium">{section.name}</span>
				</div>
			{/each}
		</div>

		<!-- Vehicle Groups Visualization -->
		{#if sequence.vehicleGroup}
			<div class="relative h-20 w-full min-w-[800px]">
				{#each sequence.vehicleGroup as group, groupIndex (group)}
					<!-- Render each vehicle in its exact position on the track -->
					{#each group.vehicles as vehicle, vehicleIndex (vehicle)}
						{@const style = calculateLength(
							vehicle.positionOnTrack.start.position,
							vehicle.positionOnTrack.end.position
						)}
						{@const vehicleType = vehicle.vehicleType}
						{@const isSelected =
							selectedVehicle.groupIndex === groupIndex && selectedVehicle.vehicleIndex === vehicleIndex}
						<button
							class="absolute h-full cursor-pointer transition-transform hover:scale-110"
							style="width: {style.width}; left: {style.left}; min-width: {style.minWidth}"
							class:scale-110={isSelected}
							onclick={() => selectVehicle(groupIndex, vehicleIndex)}
						>
							{#if vehicleType.category.includes("LOCOMOTIVE")}
								<Locomotive />
							{:else if hasLocomotive(vehicle, "before")}
								<CoachStart firstClass={vehicleType.firstClass} />
							{:else if hasLocomotive(vehicle, "after")}
								<CoachEnd firstClass={vehicleType.firstClass} />
							{:else if vehicleIndex === 0}
								<ControlcarEnd firstClass={vehicleType.firstClass} />
							{:else if vehicleIndex === group.vehicles.length - 1}
								<ControlcarStart firstClass={vehicleType.firstClass} />
							{:else}
								<CoachMiddle firstClass={vehicleType.firstClass} />
							{/if}
						</button>
					{/each}
				{/each}
			</div>
		{/if}

		<div class="mt-4 flex flex-col">
			<span class="text-text">{tripReference().referenceId}</span>
			<span class="font-semibold">{tripReference().destination}</span>
		</div>
	</div>
</div>
