<script lang="ts">
	import type { Equipment, Track, Vehicle } from "$models/sequence";
	import Locomotive from "$components/sequence/coaches/big/Locomotive.svelte";
	import DoubleDeck from "$components/sequence/coaches/big/DoubleDeck.svelte";
	import SingleFloor from "$components/sequence/coaches/big/SingleFloor.svelte";
	import type { Component } from "svelte";
	import SeatsSeverelyDisabled from "$components/sequence/equipments/SeatsSeverelyDisabled.svelte";
	import ZoneFamily from "$components/sequence/equipments/ZoneFamily.svelte";
	import ZoneQuiet from "$components/sequence/equipments/ZoneQuiet.svelte";

	let { track, vehicle }: { track?: Track; vehicle?: Vehicle } = $props();

	const sectionInfo = (): string => {
		const vehicleStart = vehicle?.positionOnTrack.start.position ?? 0;
		const vehicleEnd = vehicle?.positionOnTrack.end.position ?? 0;

		const sections =
			track?.sections.filter(
				(section) => !(vehicleEnd < section.start.position || vehicleStart > section.end.position)
			) ?? [];

		if (sections.length === 1) {
			return `Stops in section ${sections[0].name}`;
		} else if (sections.length > 1) {
			return `Stops between sections ${sections.map((section) => section.name).join(" & ")}`;
		} else return "";
	};

	type ValidEquipment = {
		type: string;
		message?: string;
		component?: Component;
	};

	const validEquipments: ValidEquipment[] = [
		{ type: "SEATS_SEVERELY_DISABLED", message: "Priority seats", component: SeatsSeverelyDisabled },
		{ type: "ZONE_FAMILY", message: "Family area", component: ZoneFamily },
		{ type: "ZONE_QUIET", message: "Resting area", component: ZoneQuiet }
	];

	const filteredEquipments: Equipment[] = $derived(
		vehicle?.equipment?.filter((equipment) => validEquipments.some((validEquipment) => validEquipment.type === equipment.type)) ?? []
	);
</script>

{#if !vehicle}{:else}
	<div
		class="flex w-full flex-col py-8 text-lg font-medium gap-y-4"
		class:border-b={!vehicle?.vehicleType?.firstClass}
		class:border-gray-700={!vehicle?.vehicleType?.firstClass}
		class:border-b-4={vehicle?.vehicleType?.firstClass}
		class:border-accent={vehicle?.vehicleType?.firstClass}
	>
		<div class="flex flex-col">
			{#if filteredEquipments.length > 0}
				{#each filteredEquipments as equipment, index (index)}
					{@const validEquipment = validEquipments.find((validEquipment) => validEquipment.type === equipment.type)}
					<div class="my-1 flex flex-row gap-x-2 text-lg">
						{#if validEquipment?.component}
							{@const Component = validEquipment.component}
							<Component />
						{/if}
						<span>{validEquipment?.message}</span>
					</div>
				{/each}
			{/if}
		</div>

		{#if vehicle?.orderNumber}
			<div class="flex flex-row gap-x-6">
				<div class="flex flex-col">
					{#if vehicle?.vehicleType?.category?.includes("LOCOMOTIVE") || vehicle?.vehicleType?.category?.includes("POWERCAR")}
						<Locomotive height="45px" width="45px" />
					{:else if vehicle?.vehicleType?.category?.includes("DOUBLEDECK")}
						<DoubleDeck height="45px" width="45px" />
					{:else}
						<SingleFloor height="45px" width="45px" />
					{/if}
					<span class="text-6xl font-semibold">{vehicle?.orderNumber}</span>
				</div>

				<div class="flex flex-col justify-end">
					{#if vehicle?.vehicleType?.category?.includes("LOCOMOTIVE")}
						<span class="font-bold">Locomotive</span>
					{:else if vehicle?.vehicleType?.category?.includes("POWERCAR")}
						<span class="font-bold">Powercar</span>
					{:else if vehicle?.vehicleType?.firstClass && vehicle?.vehicleType?.secondClass}
						<div class="flex flex-row gap-x-1 font-bold">
							<span class="text-[#f9c523]">1.</span>
							<span>/</span>
							<span>2. Class</span>
						</div>
					{:else if vehicle?.vehicleType?.firstClass}
						<span class="font-bold text-[#f9c523]">1. Class</span>
					{:else}
						<span class="font-bold">2. Class</span>
					{/if}
					<span>{sectionInfo()}</span>
				</div>
			</div>
		{:else}
			<div class="flex flex-row gap-x-6">
				<div class="translate-y-10">
					{#if vehicle?.vehicleType?.category?.includes("LOCOMOTIVE") || vehicle?.vehicleType?.category?.includes("POWERCAR")}
						<Locomotive height="125px" width="125px" />
					{:else if vehicle?.vehicleType?.category?.includes("DOUBLEDECK")}
						<DoubleDeck height="125px" width="125px" />
					{:else}
						<SingleFloor height="125px" width="125px" />
					{/if}
				</div>

				<div class="flex flex-col justify-end">
					{#if vehicle?.vehicleType?.category?.includes("LOCOMOTIVE")}
						<span class="font-bold">Locomotive</span>
					{:else if vehicle?.vehicleType?.category?.includes("POWERCAR")}
						<span class="font-bold">Powercar</span>
					{:else if vehicle?.vehicleType?.firstClass && vehicle?.vehicleType?.secondClass}
						<div class="flex flex-row gap-x-1 font-bold">
							<span class="text-[#f9c523]">1.</span>
							<span>/</span>
							<span>2. Class</span>
						</div>
					{:else if vehicle?.vehicleType?.firstClass}
						<span class="font-bold text-[#f9c523]">1. Class</span>
					{:else}
						<span class="font-bold">2. Class</span>
					{/if}
					<span>{sectionInfo()}</span>
				</div>
			</div>
		{/if}
	</div>
{/if}
