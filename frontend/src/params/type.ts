import type { ParamMatcher } from "@sveltejs/kit";

export const match: ParamMatcher = (param) => ["departures", "arrivals"].includes(param);
