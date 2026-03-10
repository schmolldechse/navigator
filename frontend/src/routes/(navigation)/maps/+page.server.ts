import { findNearbyStations } from "@lib/remote/station.remote";
import type { BaseStation } from "@lib/api";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async (): Promise<{ stations: BaseStation[] }> => ({
	stations: await findNearbyStations({ latitude: 50.1066819, longitude: 8.66282825, limit: 100, maxDistance: 5_000 })
});
