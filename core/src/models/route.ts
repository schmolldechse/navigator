import type { Connection } from "./connection.ts";
import type { Message } from "./message.ts";

interface RouteData {
	earlierRef: string;
	laterRef: string;
	journeys: Route[];
}

interface Route {
	legs: Connection[];
	messages: Message[];
	refreshToken?: string;
}

export type { RouteData, Route };
