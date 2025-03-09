import type { Handle } from "@sveltejs/kit";
import { authClient } from "$lib/auth-client";

export const handle: Handle = async ({ event, resolve }) => {
	const session = await authClient.getSession({
		fetchOptions: {
			headers: event.request.headers
		}
	});

	event.locals.session = session?.data?.session;
	event.locals.user = session?.data?.user;

	return await resolve(event);
};
