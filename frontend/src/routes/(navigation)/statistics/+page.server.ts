import type { PageServerLoad } from "./$types";
import {
	createNetworkDashboardRequests,
	createRangeLabel,
	createStatisticsFiltersFromSearchParams
} from "@lib/components/statistics/dashboard/statistics-dashboard";
import { loadNetworkMapHotspots, loadNetworkMetric } from "@lib/remote/statistics.remote";

export const load: PageServerLoad = async ({ getClientAddress, url }) => {
	const filters = createStatisticsFiltersFromSearchParams(url.searchParams);
	const requests = createNetworkDashboardRequests(filters);
	const userIp = getClientAddress();

	return {
		filters,
		rangeLabel: createRangeLabel(filters),
		metrics: {
			eventKpis: loadNetworkMetric({ request: requests.eventKpis, userIp }),
			journeyKpis: loadNetworkMetric({ request: requests.journeyKpis, userIp }),
			eventTimeSeries: loadNetworkMetric({ request: requests.eventTimeSeries, userIp }),
			journeyTimeSeries: loadNetworkMetric({ request: requests.journeyTimeSeries, userIp }),
			weekdayHourHeatmap: loadNetworkMetric({ request: requests.weekdayHourHeatmap, userIp }),
			transportTypeComparison: loadNetworkMetric({ request: requests.transportTypeComparison, userIp }),
			stationRanking: loadNetworkMetric({ request: requests.stationRanking, userIp }),
			lineRanking: loadNetworkMetric({ request: requests.lineRanking, userIp })
		},
		mapHotspots: loadNetworkMapHotspots({ request: requests.mapHotspots, userIp })
	};
};
