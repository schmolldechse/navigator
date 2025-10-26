/**
 *
 * @export
 */
export const TimetableProfile = {
	Vendo: "vendo",
	Ris: "ris"
} as const;
export type TimetableProfile = (typeof TimetableProfile)[keyof typeof TimetableProfile];
