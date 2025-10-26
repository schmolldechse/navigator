import type { StopPlace } from "./StopPlace";
import type { TransportDTO } from "./TransportDTO";
import type { AdministrationDTO } from "./AdministrationDTO";
import type { InformationDTO } from "./InformationDTO";
import type { StopAtStopPlace } from "./StopAtStopPlace";
import type { ScheduleAtStopPlaceDTO } from "./ScheduleAtStopPlaceDTO";

/**
 *
 * @export
 * @interface ArrivalEntry
 */
export interface ArrivalEntry {
	/**
	 *
	 * @type {Array<string>}
	 * @memberof ArrivalEntry
	 */
	travelsWith?: Array<string> | null;
	/**
	 *
	 * @type {ScheduleAtStopPlaceDTO}
	 * @memberof ArrivalEntry
	 */
	arrival: ScheduleAtStopPlaceDTO;
	/**
	 *
	 * @type {AdministrationDTO}
	 * @memberof ArrivalEntry
	 */
	administration?: AdministrationDTO;
	/**
	 *
	 * @type {string}
	 * @memberof ArrivalEntry
	 */
	risJourneyId?: string | null;
	/**
	 *
	 * @type {boolean}
	 * @memberof ArrivalEntry
	 */
	cancelled: boolean;
	/**
	 *
	 * @type {boolean}
	 * @memberof ArrivalEntry
	 */
	additional?: boolean | null;
	/**
	 *
	 * @type {boolean}
	 * @memberof ArrivalEntry
	 */
	demand?: boolean | null;
	/**
	 *
	 * @type {Array<StopPlace>}
	 * @memberof ArrivalEntry
	 */
	direction: Array<StopPlace>;
	/**
	 *
	 * @type {TransportDTO}
	 * @memberof ArrivalEntry
	 */
	transport: TransportDTO;
	/**
	 *
	 * @type {StopAtStopPlace}
	 * @memberof ArrivalEntry
	 */
	differingOrigin?: StopAtStopPlace;
	/**
	 *
	 * @type {Array<StopAtStopPlace>}
	 * @memberof ArrivalEntry
	 */
	viaStops: Array<StopAtStopPlace>;
	/**
	 *
	 * @type {Array<InformationDTO>}
	 * @memberof ArrivalEntry
	 */
	informations: Array<InformationDTO>;
	/**
	 *
	 * @type {StopAtStopPlace}
	 * @memberof ArrivalEntry
	 */
	origin: StopAtStopPlace;
	/**
	 *
	 * @type {string}
	 * @memberof ArrivalEntry
	 */
	hafasJourneyId?: string | null;
}
