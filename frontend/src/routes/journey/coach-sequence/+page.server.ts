import type { Sequence } from "$models/sequence";
import { error } from "@sveltejs/kit";
import { DateTime } from "luxon";

export const load = async ({ url }): Promise<{ sequence: Sequence }> => {
	const lineDetails = url.searchParams.get("lineDetails");
	const evaNumber = url.searchParams.get("evaNumber");
	const date = url.searchParams.get("date");

	if (!lineDetails || !evaNumber || !date) {
		throw error(400, "Missing required parameters");
	}

	if (!/^\d+$/.test(evaNumber)) {
		throw error(400, "evaNumber is not an integer");
	}

	const dateValidation = DateTime.fromFormat(date, "yyyyMMdd");
	if (!dateValidation.isValid) {
		return error(400, `${dateValidation.invalidExplanation}`);
	}

	const baseUrl = import.meta.env.VITE_BACKEND_DOCKER_BASE_URL;
	const request = await fetch(`${baseUrl}/api/v1/journey/sequence?lineDetails=${lineDetails}&evaNumber=${evaNumber}&date=${date}`, {
		method: "GET"
	});
	if (!request.ok) {
		throw error(400, "Failed to fetch coach sequence");
	}

	return { sequence: await request.json() as Sequence };
}