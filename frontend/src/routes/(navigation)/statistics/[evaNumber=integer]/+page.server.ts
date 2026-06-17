import { error } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { getStationByEvaNumber, getStationGatheringInfo } from "@lib/remote/station.remote";

export const load: PageServerLoad = async ({ getClientAddress, params }) => {
	const evaNumber = Number(params.evaNumber);
	if (!Number.isInteger(evaNumber) || evaNumber <= 0) error(404, "Station not found");

	const userIp = getClientAddress();

	let station;
	try {
		station = await getStationByEvaNumber({ evaNumber, userIp });
	} catch (stationError) {
		if (stationError instanceof Error && stationError.message.includes("404")) error(404, "Station not found");
		throw stationError;
	}

	return {
		evaNumber,
		station,
		gatheringPromise: getStationGatheringInfo({ evaNumber })
	};
};
