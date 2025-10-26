/**
 *
 * @export
 */
export const InformationKeyDTO = {
	UnplannedInfo: "UNPLANNED_INFO",
	GeneralWarning: "GENERAL_WARNING",
	AdditionalCoaches: "ADDITIONAL_COACHES",
	MissingCoaches: "MISSING_COACHES",
	ReplacementService: "REPLACEMENT_SERVICE",
	CancelledTrip: "CANCELLED_TRIP",
	AdditionalStops: "ADDITIONAL_STOPS",
	NoWiFi: "NO_WI_FI",
	ChangedSequence: "CHANGED_SEQUENCE",
	NoFirstClass: "NO_FIRST_CLASS",
	AccessibilityWarning: "ACCESSIBILITY_WARNING",
	ReservationsMissing: "RESERVATIONS_MISSING",
	ReservationsRequired: "RESERVATIONS_REQUIRED",
	NoFood: "NO_FOOD",
	NoBicycleTransport: "NO_BICYCLE_TRANSPORT",
	BicycleWarning: "BICYCLE_WARNING",
	BicycleTransport: "BICYCLE_TRANSPORT",
	BicycleReservationRequired: "BICYCLE_RESERVATION_REQUIRED",
	TicketInformation: "TICKET_INFORMATION"
} as const;
export type InformationKeyDTO = (typeof InformationKeyDTO)[keyof typeof InformationKeyDTO];
