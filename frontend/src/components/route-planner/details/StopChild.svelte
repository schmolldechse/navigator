<script lang="ts">
	import type { Time } from "$models/time";
	import type { Stop } from "$models/station";
	import TimeInformation from "$components/ui/info/TimeInformation.svelte";
	import CircleDot from "lucide-svelte/icons/circle-dot";
	import ChevronRight from "lucide-svelte/icons/chevron-right";
	import Platform from "$components/ui/info/Platform.svelte";
	import GeneralWarning from "$components/timetable/messages/icons/GeneralWarning.svelte";
	import NoOnwardJourney from "$components/timetable/messages/icons/NoOnwardJourney.svelte";
	import type { Component } from "svelte";
	import type { Message } from "$models/message";

	let {
		time,
		stop,
		showBothTimes = false,
		isChangeover = false,
		position = "start",
		class: className = ""
	}: {
		time?: Time;
		stop?: Stop;
		showBothTimes?: boolean;
		isChangeover?: boolean;
		position?: "start" | "center" | "end";
		class?: string;
	} = $props();

	// TODO: investigate, if it's possible to check by message's type instead of checking linguistic summaries
	const getMessageComponent = (message: Message): Component => {
		if (message?.summary === "Stop cancelled" || message?.summary === "Halt entfällt") return NoOnwardJourney;
		return GeneralWarning;
	};
</script>

<div
	class={[
		"relative flex min-h-fit flex-row",
		{ "items-end": position === "end" },
		{ "items-center py-0.75": position === "center" },
		className
	]}
>
	<!-- 1/6 Time -->
	{#if !showBothTimes}
		<TimeInformation
			{time}
			direction="col"
			class="min-w-1 basis-1/6 items-end text-base"
			delayClass="text-sm md:text-base"
		/>
	{:else}
		<div class="flex min-w-1 basis-1/6 flex-col items-end self-center">
			<TimeInformation time={stop?.arrival} class="text-base" delayClass="text-sm md:text-base" noDoubleSpan={true} />
			<TimeInformation time={stop?.departure} class="text-base" delayClass="text-sm md:text-base" noDoubleSpan={true} />
		</div>
	{/if}

	<!-- Connecting Line -->
	<div
		class={[
			"flex w-[50px] justify-center transition-all duration-500 md:w-[75px]",
			{ "items-center": position === "center" },
			{ "items-end": position === "end" }
		]}
	>
		<CircleDot class="bg-background absolute z-1 shrink-0" />
		<span
			class={[
				"absolute z-0 h-full w-[4px]",
				{ "bg-text": !isChangeover },
				// prettier-ignore
				{ "changeover": isChangeover }
			]}
		></span>
	</div>

	<!-- 4/6 Stop Info -->
	<div class={["flex basis-4/6 flex-col leading-none", { "self-center": position === "center" }]}>
		<div class="flex flex-row items-center">
			<a class="font-bold break-words hyphens-auto" href={`/${stop?.evaNumber}/departures`} target="_blank">
				{stop?.name}
			</a>
			<ChevronRight color="#ffda0a" class="ml-1 shrink-0" />
		</div>

		<div class="flex flex-col">
			{#each stop?.messages ?? [] as message}
				{@const MessageComponent = getMessageComponent(message)}
				<div class="flex items-center gap-x-2">
					<MessageComponent />
					<span class="text-sm font-semibold">{message?.text}</span>
				</div>
			{/each}
		</div>
	</div>

	<!-- 1/6 Platform -->
	<Platform {time} class="basis-1/6 md:items-start" direction="col" />
</div>
