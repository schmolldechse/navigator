import type { BaseMetricDataPoint } from "@lib/api";

type TypePropertyValue<T> = T extends { $type: infer Key } ? (Key extends keyof T ? T[Key] : never) : undefined;

const getMetricTypeProperty = <T extends BaseMetricDataPoint>(dataPoint: T): TypePropertyValue<T> => {
	if ("$type" in dataPoint) {
		const key = dataPoint.$type as keyof T;
		return dataPoint[key] as TypePropertyValue<T>;
	}
	return undefined as TypePropertyValue<T>;
};

export { getMetricTypeProperty, type TypePropertyValue };
