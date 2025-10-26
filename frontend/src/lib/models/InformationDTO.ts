import type { InformationType } from "./InformationType";
import type { InformationKeyDTO } from "./InformationKeyDTO";

/**
 *
 * @export
 * @interface InformationDTO
 */
export interface InformationDTO {
	/**
	 *
	 * @type {InformationType}
	 * @memberof InformationDTO
	 */
	type: InformationType;
	/**
	 *
	 * @type {string}
	 * @memberof InformationDTO
	 */
	text: string;
	/**
	 *
	 * @type {InformationKeyDTO}
	 * @memberof InformationDTO
	 */
	key: InformationKeyDTO;
	/**
	 *
	 * @type {string}
	 * @memberof InformationDTO
	 */
	textShort?: string | null;
}
