import type { Handle } from "@sveltejs/kit";

export const handle: Handle = async ({ event, resolve }) => {
    const response = await fetch('http://localhost:8080/api/v1/auth/me', {
        headers: event.request.headers
    });

    console.log(response.status, response.statusText)
    return await resolve(event);
};