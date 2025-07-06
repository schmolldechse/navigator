import type { ParamMatcher } from "@sveltejs/kit";

export const match: ParamMatcher = (param) => /^\d+(?:st|nd|rd|th)?$/i.test(param);