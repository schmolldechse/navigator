/**
 *
 * @export
 */
export const InformationType = {
	JourneyAttribute: "JOURNEY_ATTRIBUTE",
	Disruption: "DISRUPTION",
	Message: "MESSAGE",
	RisQualityDeviation: "RIS_QUALITY_DEVIATION",
	RisCauseReason: "RIS_CAUSE_REASON"
} as const;
export type InformationType = (typeof InformationType)[keyof typeof InformationType];
