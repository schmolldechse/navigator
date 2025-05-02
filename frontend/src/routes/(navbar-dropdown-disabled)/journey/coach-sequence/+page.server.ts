import type { Sequence } from "$models/sequence";
import { error } from "@sveltejs/kit";
import { DateTime } from "luxon";
import { env } from "$env/dynamic/private";

export const load = async ({ url }): Promise<{ sequence: Sequence }> => {
	const lineDetails = url.searchParams.get("lineDetails");
	const evaNumber = url.searchParams.get("evaNumber");
	const date = url.searchParams.get("date");

	if (!lineDetails || !evaNumber || !date) {
		throw error(400, "Missing required 'lineDetails', 'evaNumber' or 'date' parameter.");
	}

	if (!/^\d+$/.test(evaNumber)) {
		throw error(400, "Your provided evaNumber is not a number.");
	}

	const dateValidation = DateTime.fromFormat(date, "yyyyMMdd");
	if (!dateValidation.isValid) {
		return error(400, `${dateValidation.invalidExplanation}`);
	}

	const request = await fetch(
		`${env.BACKEND_DOCKER_BASE_URL}/api/v1/journey/sequence?lineDetails=${lineDetails}&evaNumber=${evaNumber}&date=${date}`,
		{
			method: "GET"
		}
	);
	if (!request.ok) {
		throw error(400, "This coach sequence does not exist.");
	}

	return { sequence: (await request.json()) as Sequence };
};
