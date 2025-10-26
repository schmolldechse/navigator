import type { TransportType } from "./TransportType";
import type { PositionDTO } from "./PositionDTO";

/**
 *
 * @export
 * @interface StationDetailDTO
 */
export interface StationDetailDTO {
	/**
	 *
	 * @type {Array<string>}
	 * @memberof StationDetailDTO
	 */
	ril100: Array<string>;
	/**
	 *
	 * @type {Array<TransportType>}
	 * @memberof StationDetailDTO
	 */
	transports: Array<TransportType>;
	/**
	 *
	 * @type {PositionDTO}
	 * @memberof StationDetailDTO
	 */
	position: PositionDTO;
	/**
	 *
	 * @type {string}
	 * @memberof StationDetailDTO
	 */
	name: string;
	/**
	 *
	 * @type {number}
	 * @memberof StationDetailDTO
	 */
	evaNumber: number;
}
