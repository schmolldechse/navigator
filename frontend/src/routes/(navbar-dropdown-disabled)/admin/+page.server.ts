import { redirect } from "@sveltejs/kit";

export const load = async ({ locals: { session, user } }) => {
	if (!session || user?.role !== "admin") return redirect(308, "/");
};
