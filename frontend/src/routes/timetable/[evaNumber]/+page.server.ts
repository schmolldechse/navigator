import { error, redirect } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = (async ({ params, url }) => {
    if (!/^\d+$/.test(params.evaNumber)) throw error(400, "Station's evaNumber must be a number");
    return redirect(308, `${url.pathname}/departures`);
});