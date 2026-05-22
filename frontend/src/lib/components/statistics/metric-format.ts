import { MetricUnit } from "@lib/api";
import { Duration } from "luxon";

type FormattedMetricValue = {
	value: string;
	unit: string;
};

const formatNumber = (value: number, maximumFractionDigits: number): string =>
	value.toLocaleString(undefined, {
		maximumFractionDigits
	});

const formatMetricValue = (value: number, unit: MetricUnit): FormattedMetricValue => {
	if (!Number.isFinite(value)) return { value: "N/A", unit: "" };

	if (unit === MetricUnit.SECONDS) {
		const duration = Duration.fromObject({ seconds: value });
		if (Math.abs(value) >= 60) return { value: formatNumber(duration.as("minutes"), 1), unit: "min" };
		return { value: formatNumber(duration.as("seconds"), 0), unit: "s" };
	}

	if (unit === MetricUnit.PERCENT) return { value: formatNumber(value, 2), unit: "%" };
	if (unit === MetricUnit.COUNT) return { value: value.toLocaleString(), unit: "" };
	if (unit === MetricUnit.BYTES) return { value: value.toLocaleString(), unit: "B" };

	return { value: formatNumber(value, 2), unit };
};

export { type FormattedMetricValue, formatMetricValue };
