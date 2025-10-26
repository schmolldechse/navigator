import type { PositionDTO } from "./PositionDTO";

/**
 *
 * @export
 * @interface StationSummaryDTO
 */
export interface StationSummaryDTO {
	/**
	 *
	 * @type {PositionDTO}
	 * @memberof StationSummaryDTO
	 */
	position: PositionDTO;
	/**
	 *
	 * @type {string}
	 * @memberof StationSummaryDTO
	 */
	name: string;
	/**
	 *
	 * @type {number}
	 * @memberof StationSummaryDTO
	 */
	evaNumber: number;
}
