import type { JourneyType } from "./JourneyType";
import type { TransportType } from "./TransportType";

/**
 *
 * @export
 * @interface TransportDTO
 */
export interface TransportDTO {
	/**
	 *
	 * @type {string}
	 * @memberof TransportDTO
	 */
	category: string;
	/**
	 *
	 * @type {TransportType}
	 * @memberof TransportDTO
	 */
	type: TransportType;
	/**
	 *
	 * @type {string}
	 * @memberof TransportDTO
	 */
	line?: string | null;
	/**
	 *
	 * @type {number}
	 * @memberof TransportDTO
	 */
	number: number;
	/**
	 *
	 * @type {JourneyType}
	 * @memberof TransportDTO
	 */
	journeyType?: JourneyType;
	/**
	 *
	 * @type {string}
	 * @memberof TransportDTO
	 */
	journeyDescription: string;
	/**
	 *
	 * @type {TransportType}
	 * @memberof TransportDTO
	 */
	replacementType?: TransportType;
}
