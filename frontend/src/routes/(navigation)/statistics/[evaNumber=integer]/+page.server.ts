import { error } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { getStationByEvaNumber } from "@lib/remote/station.remote";

export const load: PageServerLoad = async ({ params, getClientAddress }) => {
	const evaNumber = Number(params.evaNumber);

	if (!Number.isInteger(evaNumber) || evaNumber <= 0) {
		error(400, "The selected station route is invalid.");
	}

	const station = await getStationByEvaNumber({ evaNumber, userIp: getClientAddress() });
	if (!station) error(404, "The selected station could not be found.");

	return { station };
};
