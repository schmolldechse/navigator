import type { LayoutServerLoad } from "./$types";
import { redirect } from "@sveltejs/kit";

export const load: LayoutServerLoad = async ({ locals: { session, user } }) => {
	if (!session || user?.role !== "admin") return redirect(301, "/");
};
