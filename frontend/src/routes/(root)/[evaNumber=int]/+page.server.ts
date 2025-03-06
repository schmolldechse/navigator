import { redirect } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";

export const load = (async ({ url }) => {
	return redirect(308, `${url.pathname}/departures`);
}) satisfies PageServerLoad;
