/**
 *
 * @export
 * @interface ScheduleAtStopPlaceDTO
 */
export interface ScheduleAtStopPlaceDTO {
	/**
	 *
	 * @type {Date}
	 * @memberof ScheduleAtStopPlaceDTO
	 */
	actualTime: Date;
	/**
	 *
	 * @type {number}
	 * @memberof ScheduleAtStopPlaceDTO
	 */
	delay: number;
	/**
	 *
	 * @type {string}
	 * @memberof ScheduleAtStopPlaceDTO
	 */
	plannedPlatform?: string | null;
	/**
	 *
	 * @type {Date}
	 * @memberof ScheduleAtStopPlaceDTO
	 */
	plannedTime: Date;
	/**
	 *
	 * @type {string}
	 * @memberof ScheduleAtStopPlaceDTO
	 */
	actualPlatform?: string | null;
}
