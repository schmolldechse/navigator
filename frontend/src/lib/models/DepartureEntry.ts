import type { StopPlace } from "./StopPlace";
import type { TransportDTO } from "./TransportDTO";
import type { CoupledTransport } from "./CoupledTransport";
import type { AdministrationDTO } from "./AdministrationDTO";
import type { InformationDTO } from "./InformationDTO";
import type { StopAtStopPlace } from "./StopAtStopPlace";
import type { ScheduleAtStopPlaceDTO } from "./ScheduleAtStopPlaceDTO";

/**
 *
 * @export
 * @interface DepartureEntry
 */
export interface DepartureEntry {
	/**
	 *
	 * @type {Array<CoupledTransport>}
	 * @memberof DepartureEntry
	 */
	travelsWith?: Array<CoupledTransport> | null;
	/**
	 *
	 * @type {StopAtStopPlace}
	 * @memberof DepartureEntry
	 */
	differingDestination?: StopAtStopPlace;
	/**
	 *
	 * @type {AdministrationDTO}
	 * @memberof DepartureEntry
	 */
	administration?: AdministrationDTO;
	/**
	 *
	 * @type {string}
	 * @memberof DepartureEntry
	 */
	risJourneyId?: string | null;
	/**
	 *
	 * @type {boolean}
	 * @memberof DepartureEntry
	 */
	cancelled: boolean;
	/**
	 *
	 * @type {ScheduleAtStopPlaceDTO}
	 * @memberof DepartureEntry
	 */
	departure: ScheduleAtStopPlaceDTO;
	/**
	 *
	 * @type {boolean}
	 * @memberof DepartureEntry
	 */
	additional?: boolean | null;
	/**
	 *
	 * @type {boolean}
	 * @memberof DepartureEntry
	 */
	demand?: boolean | null;
	/**
	 *
	 * @type {Array<StopPlace>}
	 * @memberof DepartureEntry
	 */
	direction: Array<StopPlace>;
	/**
	 *
	 * @type {TransportDTO}
	 * @memberof DepartureEntry
	 */
	transport: TransportDTO;
	/**
	 *
	 * @type {Array<StopAtStopPlace>}
	 * @memberof DepartureEntry
	 */
	viaStops: Array<StopAtStopPlace>;
	/**
	 *
	 * @type {Array<InformationDTO>}
	 * @memberof DepartureEntry
	 */
	informations: Array<InformationDTO>;
	/**
	 *
	 * @type {StopAtStopPlace}
	 * @memberof DepartureEntry
	 */
	destination: StopAtStopPlace;
	/**
	 *
	 * @type {string}
	 * @memberof DepartureEntry
	 */
	hafasJourneyId?: string | null;
}
