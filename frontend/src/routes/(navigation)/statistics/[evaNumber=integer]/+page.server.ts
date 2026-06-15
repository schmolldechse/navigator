import { error } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import {
	createRangeLabel,
	createStationDashboardRequests,
	createStatisticsFiltersFromSearchParams
} from "@lib/components/statistics/dashboard/statistics-dashboard";
import { loadStationMetric } from "@lib/remote/statistics.remote";
import { getStationByEvaNumber, getStationGatheringInfo } from "@lib/remote/station.remote";

export const load: PageServerLoad = async ({ getClientAddress, params, url }) => {
	const evaNumber = Number(params.evaNumber);
	if (!Number.isInteger(evaNumber) || evaNumber <= 0) error(404, "Station not found");

	const userIp = getClientAddress();
	const filters = createStatisticsFiltersFromSearchParams(url.searchParams);
	const requests = createStationDashboardRequests(filters, evaNumber);

	let station;
	try {
		station = await getStationByEvaNumber({ evaNumber, userIp });
	} catch (stationError) {
		if (stationError instanceof Error && stationError.message.includes("404")) error(404, "Station not found");
		throw stationError;
	}

	return {
		evaNumber,
		station,
		gatheringPromise: getStationGatheringInfo({ evaNumber }),
		filters,
		rangeLabel: createRangeLabel(filters),
		metrics: {
			eventKpis: loadStationMetric({ request: requests.eventKpis, userIp }),
			benchmark: loadStationMetric({ request: requests.benchmark, userIp }),
			timeSeries: loadStationMetric({ request: requests.timeSeries, userIp }),
			arrivalDepartureComparison: loadStationMetric({ request: requests.arrivalDepartureComparison, userIp }),
			weekdayHourHeatmap: loadStationMetric({ request: requests.weekdayHourHeatmap, userIp }),
			lineRanking: loadStationMetric({ request: requests.lineRanking, userIp }),
			directions: loadStationMetric({ request: requests.directions, userIp }),
			transportTypeMix: loadStationMetric({ request: requests.transportTypeMix, userIp }),
			lineHourMatrix: loadStationMetric({ request: requests.lineHourMatrix, userIp }),
			eventDetails: loadStationMetric({ request: requests.eventDetails, userIp })
		}
	};
};
