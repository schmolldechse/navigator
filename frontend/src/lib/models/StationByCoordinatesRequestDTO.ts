/**
 *
 * @export
 * @interface StationByCoordinatesRequestDTO
 */
export interface StationByCoordinatesRequestDTO {
	/**
	 * The latitude of the location.
	 * @type {number}
	 * @memberof StationByCoordinatesRequestDTO
	 */
	longitude: number;
	/**
	 * The maximum distance in meters to search for stations. Defaults to 1000.
	 * @type {number}
	 * @memberof StationByCoordinatesRequestDTO
	 */
	maxDistanceMeters?: number | null;
	/**
	 * The maximum number of stations to return. Defaults to 100.
	 * @type {number}
	 * @memberof StationByCoordinatesRequestDTO
	 */
	limit?: number | null;
	/**
	 * The longitude of the location.
	 * @type {number}
	 * @memberof StationByCoordinatesRequestDTO
	 */
	latitude: number;
}
