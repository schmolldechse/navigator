import { error } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";

// This is to catch all unfound pages and redirect them to the error page with the layout
export const load: PageServerLoad = (async () => {
	throw error(404, "We're sorry, but the page you're looking for does not exist.");
});
