import { t } from "elysia";
import { DateTime } from "luxon";

type DateTimeOptions = {
	fieldName: string;
	description?: string;
	error?: string | ((input: unknown) => string);
	default?: string | number;
};

const DateTimeObject = (options: DateTimeOptions = { fieldName: "field" }) =>
	t
		.Transform(
			t.Union([t.String(), t.Number()], {
				description: options.description,
				error: options.error,
				default: options.default
			})
		)
		.Decode((value) => {
			if (typeof value === "string") {
				const dateTime = DateTime.fromISO(value);
				if (!dateTime.isValid)
					throw new Error(`Invalid ISO datetime for ${options.fieldName}: ${dateTime.invalidReason}`);
				return dateTime;
			} else if (typeof value === "number") {
				const dateTime = DateTime.fromMillis(value);
				if (!dateTime.isValid)
					throw new Error(`Invalid milliseconds timestamp for ${options.fieldName}: ${dateTime.invalidReason}`);
				return dateTime;
			}
			throw new Error("DateTime must be ISO string or milliseconds number");
		})
		.Encode((value) => (DateTime.isDateTime(value) ? value.toISO() : value));

export { DateTimeObject };
