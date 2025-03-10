import type { ParamMatcher } from "@sveltejs/kit";

export const match: ParamMatcher = (param) => {
	return ["departures", "arrivals"].includes(param);
};
