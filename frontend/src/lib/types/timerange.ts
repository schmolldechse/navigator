import type { DateTime } from "luxon";

interface Timerange {
	start: DateTime;
	end: DateTime;
}

interface TimerangeOption {
	id: string;
	label: string;
	value?: Timerange;
	isCustom?: boolean;
}

export type { Timerange, TimerangeOption };
