import { env } from "$env/dynamic/public";
import type { Handle } from "@sveltejs/kit";

export const handle: Handle = async ({ event, resolve }) => {
	if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

	const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/auth/me`, {
		headers: event.request.headers
	});

	if (!response.ok) return await resolve(event);
	return await resolve(event);
};
