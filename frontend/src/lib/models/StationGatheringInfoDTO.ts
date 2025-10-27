import type { TransportType } from "./TransportType";
export type StationGatheringInfoDTO = {
	/**
	 * Indicates if querying for this station is enabled.
	 */
	queryingEnabled: boolean;
	/**
	 * Timestamp of the last successful querying for this station; `nil` if never queried.
	 */
	lastQueried?: string | null;
	/**
	 * Transport types ignored during RIS ID discovery; journeys with these types are skipped and no RIS IDs are created.
	 */
	inactive: Array<TransportType>;
	/**
	 * Transport types enabled for RIS ID discovery; only journeys with these types are inserted as unique RIS IDs.
	 */
	active: Array<TransportType>;
};
