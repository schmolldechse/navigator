import type { StopPlace } from "./StopPlace";

/**
 *
 * @export
 * @interface CoupledTransport
 */
export interface CoupledTransport {
	/**
	 *
	 * @type {string}
	 * @memberof CoupledTransport
	 */
	journeyId: string;
	/**
	 *
	 * @type {StopPlace}
	 * @memberof CoupledTransport
	 */
	separationAt: StopPlace;
}
