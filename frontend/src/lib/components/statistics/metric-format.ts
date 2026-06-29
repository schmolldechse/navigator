import { MetricUnit } from "@lib/api";
import { Duration } from "luxon";

type FormattedMetricValue = {
	value: string;
	unit: string;
};

const formatMetricValue = (value: number, unit: MetricUnit): FormattedMetricValue => {
	if (!Number.isFinite(value)) return { value: "N/A", unit: "" };

	switch (unit) {
		case MetricUnit.SECONDS: {
			const duration = Duration.fromObject({ seconds: value });

			if (Math.abs(value) >= 60)
				return { value: duration.as("minutes").toLocaleString(undefined, { maximumFractionDigits: 1 }), unit: "min" };

			return { value: duration.as("seconds").toLocaleString(undefined, { maximumFractionDigits: 0 }), unit: "s" };
		}
		case MetricUnit.PERCENT:
			return { value: value.toLocaleString(undefined, { maximumFractionDigits: 2 }), unit: "%" };
		case MetricUnit.COUNT:
			return { value: value.toLocaleString(), unit: "" };
		case MetricUnit.BYTES:
			return { value: value.toLocaleString(), unit: "B" };
		default:
			return { value: value.toLocaleString(undefined, { maximumFractionDigits: 2 }), unit };
	}
};

export { type FormattedMetricValue, formatMetricValue };
