import type { StationSummaryDTO } from "$lib/models/StationSummaryDTO";
import type { PageServerLoad } from "./$types";
import { getStations } from "./stations.remote";

export const load: PageServerLoad = async (): Promise<{ stations: StationSummaryDTO[] }> => {
	return { stations: await getStations({ latitude: 50.1066819, longitude: 8.66282825, radius: 1_500, limit: 250 }) };
};
