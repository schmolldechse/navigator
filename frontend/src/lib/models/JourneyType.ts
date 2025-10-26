/**
 * Defines whether journey [Fahrt] is regular or some kind of special.
 *
 * - REGULAR (Regular scheduled journey)
 * - REPLACEMENT (Journey that replaces another journey)
 * - RELIEF (Journey that reliefs another journey)
 * - EXTRA (Journey that is somehow extra)
 * @export
 */
export const JourneyType = {
	Regular: "REGULAR",
	Replacement: "REPLACEMENT",
	Relief: "RELIEF",
	Extra: "EXTRA"
} as const;
export type JourneyType = (typeof JourneyType)[keyof typeof JourneyType];
