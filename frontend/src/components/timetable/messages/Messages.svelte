<script lang="ts">
	import type { Message } from "$models/message";
	import type { Component } from "svelte";
	import type { Connection } from "$models/connection";
	import { DateTime } from "luxon";
	import type { Stop } from "$models/station";
	import { mapStops, writeStop } from "$lib";
	import BicycleTransport from "$components/timetable/messages/icons/BicycleTransport.svelte";
	import BicycleReservationRequired from "$components/timetable/messages/icons/BicycleReservationRequired.svelte";
	import CancelledStops from "$components/timetable/messages/icons/CancelledStops.svelte";
	import AdditionalStops from "$components/timetable/messages/icons/AdditionalStops.svelte";
	import TrackChanged from "$components/timetable/messages/icons/TrackChanged.svelte";
	import UnplannedInfo from "$components/timetable/messages/icons/UnplannedInfo.svelte";
	import CancelledTrip from "$components/timetable/messages/icons/CancelledTrip.svelte";
	import AccessibilityWarning from "$components/timetable/messages/icons/AccessibilityWarning.svelte";
	import TicketInformation from "$components/timetable/messages/icons/TicketInformation.svelte";
	import AdditionalCoaches from "$components/timetable/messages/icons/AdditionalCoaches.svelte";
	import MissingCoaches from "$components/timetable/messages/icons/MissingCoaches.svelte";
	import ReplacementService from "$components/timetable/messages/icons/ReplacementService.svelte";
	import NoFood from "$components/timetable/messages/icons/NoFood.svelte";
	import NoFirstClass from "$components/timetable/messages/icons/NoFirstClass.svelte";
	import ReservationRequired from "$components/timetable/messages/icons/ReservationRequired.svelte";
	import GeneralWarning from "$components/timetable/messages/icons/GeneralWarning.svelte";
	import ChangedSequence from "$components/timetable/messages/icons/ChangedSequence.svelte";
	import LiftWarning from "$components/timetable/messages/icons/LiftWarning.svelte";
	import NoWiFi from "$components/timetable/messages/icons/NoWiFi.svelte";
	import BicycleWarning from "$components/timetable/messages/icons/BicycleWarning.svelte";
	import NoOnwardJourney from "$components/timetable/messages/icons/NoOnwardJourney.svelte";
	import ContinuationBy from "$components/timetable/messages/icons/ContinuationBy.svelte";
	import ReservationsMissing from "$components/timetable/messages/icons/ReservationsMissing.svelte";
	import NoBicycleTransport from "$components/timetable/messages/icons/NoBicycleTransport.svelte";
	import ShowMore from "$components/timetable/info/ShowMore.svelte";

	let { connection }: { connection?: Connection } = $props();
	let expanded = $state<boolean>(false);

	type ValidMessage = {
		type: string;
		component?: Component;
	};

	const validMessages: ValidMessage[] = [
		{ type: "bicycle-transport", component: BicycleTransport },
		{ type: "bicycle-reservation-required", component: BicycleReservationRequired },
		{ type: "canceled-stops", component: CancelledStops },
		{ type: "additional-stops", component: AdditionalStops },
		{ type: "track-changed", component: TrackChanged },
		{ type: "accessibility-warning", component: AccessibilityWarning }, // e.g. without "Vehicle-mounted boarding aid", "No disabled WC on the train",
		{ type: "unplanned-info", component: UnplannedInfo },
		{ type: "canceled-trip", component: CancelledTrip },
		{ type: "ticket-information", component: TicketInformation }, // if a train has a additionalLineName
		{ type: "additional-coaches", component: AdditionalCoaches },
		{ type: "missing-coaches", component: MissingCoaches },
		{ type: "replacement-service", component: ReplacementService },
		{ type: "no-food", component: NoFood },
		{ type: "no-first-class", component: NoFirstClass },
		{ type: "reservation-required", component: ReservationRequired },
		{ type: "general-warning", component: GeneralWarning },
		{ type: "changed-sequence", component: ChangedSequence },
		{ type: "lift-warning", component: LiftWarning },
		{ type: "no-wi-fi", component: NoWiFi },
		{ type: "bicycle-warning", component: BicycleWarning },
		{ type: "no-onward-journey", component: NoOnwardJourney },
		{ type: "continuation-by", component: ContinuationBy },
		{ type: "reservations-missing", component: ReservationsMissing },
		{ type: "no-bicycle-transport", component: NoBicycleTransport }
	];

	const collectMessages = (messages?: Record<string, Message[]> | undefined): Message[] => {
		return messages ? Object.values(messages).flat() : [];
	};

	const filteredMessages: Message[] = $derived(
		collectMessages(connection?.messages || {}).filter((message) => {
			const isValid = validMessages.some((validMessage) => validMessage.type === message.type);
			return connection?.cancelled ? isValid && message?.important : isValid;
		})
	);

	const formatMessage = (message: Message): string => {
		if (!message?.type || !message?.text) return "Invalid message object";
		if (!message?.links || message?.links?.length === 0) return message?.text;

		let formatted = message?.text;
		message?.links?.forEach((link, index) => {
			const placeholder = `{{${index}}}`;

			if (link?.type === "station") {
				const stop: Stop = mapStops(link)![0];
				formatted = formatted.replace(placeholder, writeStop(stop, link?.name));
			} else if (link?.type === "time") {
				const time = DateTime.fromISO(link?.time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE);
				formatted = formatted.replace(placeholder, time);
			} else if (link?.type === "line") formatted = formatted.replace(placeholder, link?.lineName);
			else if (link?.type === "link") formatted = formatted.replace(placeholder, link?.label);
			else formatted = formatted.replace(placeholder, link?.name);
		});
		return formatted;
	};
</script>

{#if filteredMessages.length > 1 && !expanded}
	<div class="my-1 flex flex-row gap-x-2 py-0.5">
		{#each filteredMessages as message, index (index)}
			{@const validMessage = validMessages.find((validMessage) => validMessage.type === message.type)}
			{#if validMessage?.component}
				{@const Component = validMessage.component}
				<div title={formatMessage(message)}>
					<Component />
				</div>
			{/if}
		{/each}
		<ShowMore onclick={() => (expanded = true)} />
	</div>
{/if}

{#if filteredMessages.length <= 1 || expanded}
	{#each filteredMessages as message, index (index)}
		{@const validMessage = validMessages.find((validMessage) => validMessage.type === message.type)}
		<div class="my-1 flex flex-row gap-x-2 py-0.5 text-lg">
			{#if validMessage?.component}
				{@const Component = validMessage.component}
				<Component />
			{/if}
			<span>{formatMessage(message)}</span>
		</div>
	{/each}
{/if}
