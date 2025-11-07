import type { DateRangeDTO } from "./DateRangeDTO";

export type MeasuredTimeframeStatisticDTO = {
	unit: string;
	timeframe: DateRangeDTO;
	change: number;
	total: number;
};
