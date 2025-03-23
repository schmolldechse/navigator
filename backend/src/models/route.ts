import type { Connection } from "./connection.ts";

interface RouteData {
	earlierRef: string;
	laterRef: string;
	journeys: Route[];
}

interface Route {
	legs: Connection[];
	refreshToken?: string;
}

export type { RouteData, Route };
