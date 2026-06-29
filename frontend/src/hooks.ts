import type { Transport } from "@sveltejs/kit";
import { DateTime } from "luxon";

export const transport: Transport = {
	DateTime: {
		encode: (value: DateTime) => value instanceof DateTime && value.toISO(),
		decode: (value: string) => DateTime.fromISO(value)
	}
};
