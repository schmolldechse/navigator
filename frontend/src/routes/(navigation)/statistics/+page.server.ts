import type { PageServerLoad } from "./$types";
import {
	createDefaultStationMetricMapSettings,
	createStationMetricMapRequest,
	type StationMetricMapSettings
} from "@lib/components/statistics/global/station-metric-map/station-metric-map";
import {
	createDefaultAdministrationRankingSettings,
	createAdministrationRankingRequest,
	type AdministrationRankingSettings
} from "@lib/components/statistics/global/administration-ranking/administration-ranking";
import {
	createDefaultLineRankingSettings,
	createLineRankingRequest,
	type LineRankingSettings
} from "@lib/components/statistics/global/line-ranking/line-ranking";
import {
	createStatisticsScopeSettingsFromSearchParams,
	type StatisticsScopeSettings
} from "@lib/components/statistics/global/statistics-scope";
import { loadStationMetricMap } from "@lib/components/statistics/global/station-metric-map/station-metric-map.remote";
import { loadMetric } from "@lib/remote/metrics.remote";
import {
	createNetworkTimeSeriesPromises,
	type NetworkTimeSeriesPromises
} from "@lib/components/statistics/global/network-time-series/network-time-series";
import type { StationMetricMapSeries } from "@lib/components/statistics/global/station-metric-map/station-metric-map-types";

export const load: PageServerLoad = async ({
	getClientAddress,
	url
}): Promise<{
	stationMetricMap: {
		promise: Promise<StationMetricMapSeries>;
		settings: StationMetricMapSettings;
	};
	networkQuality: {
		promises: NetworkTimeSeriesPromises;
	};
	administrationRanking: {
		promise: ReturnType<typeof loadMetric>;
		settings: AdministrationRankingSettings;
	};
	lineRanking: {
		promise: ReturnType<typeof loadMetric>;
		settings: LineRankingSettings;
	};
	scope: StatisticsScopeSettings;
}> => {
	const scopeSettings: StatisticsScopeSettings = createStatisticsScopeSettingsFromSearchParams(url.searchParams);
	const userIp = getClientAddress();

	const stationMetricMapSettings = createDefaultStationMetricMapSettings();
	const stationMetricMapRequest = createStationMetricMapRequest(stationMetricMapSettings, scopeSettings);

	const administrationRankingSettings: AdministrationRankingSettings = createDefaultAdministrationRankingSettings();
	const administrationRankingRequest = createAdministrationRankingRequest(administrationRankingSettings, scopeSettings);

	const lineRankingSettings: LineRankingSettings = createDefaultLineRankingSettings();
	const lineRankingRequest = createLineRankingRequest(lineRankingSettings, scopeSettings);

	return {
		stationMetricMap: {
			promise: loadStationMetricMap({ request: stationMetricMapRequest, userIp }),
			settings: stationMetricMapSettings
		},
		networkQuality: {
			promises: createNetworkTimeSeriesPromises(scopeSettings, userIp)
		},
		administrationRanking: {
			promise: loadMetric({ request: administrationRankingRequest, userIp }),
			settings: administrationRankingSettings
		},
		lineRanking: {
			promise: loadMetric({ request: lineRankingRequest, userIp }),
			settings: lineRankingSettings
		},
		scope: scopeSettings
	};
};
