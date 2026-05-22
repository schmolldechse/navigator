import type { PageServerLoad } from "./$types";
import { loadStationMetricMap } from "@lib/components/statistics/global/station-metric-map/station-metric-map.remote";
import {
	createDefaultStationMetricMapSettings,
	createStationMetricMapRequest,
	type StationMetricMapSettings
} from "@lib/components/statistics/global/station-metric-map/station-metric-map";
import { loadMetric } from "@lib/remote/metrics.remote";
import {
	createAdministrationRankingRequest,
	createDefaultAdministrationRankingSettings,
	type AdministrationRankingSettings
} from "@lib/components/statistics/global/administration-ranking/administration-ranking";
import {
	createDefaultLineRankingSettings,
	createLineRankingRequest,
	type LineRankingSettings
} from "@lib/components/statistics/global/line-ranking/line-ranking";
import {
	createDefaultNetworkTimeSeriesSettings,
	createNetworkTimeSeriesRequest
} from "@lib/components/statistics/global/network-time-series/network-time-series";

export const load: PageServerLoad = async ({
	getClientAddress
}): Promise<{
	stationMetricMap: {
		promise: Promise<Awaited<ReturnType<typeof loadStationMetricMap>>>;
		settings: StationMetricMapSettings;
	};
	networkTimeSeries: {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		settings: StationMetricMapSettings;
	};
	administrationRanking: {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		settings: AdministrationRankingSettings;
	};
	lineRanking: {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		settings: LineRankingSettings;
	};
}> => {
	const stationMetricMapSettings: StationMetricMapSettings = createDefaultStationMetricMapSettings();
	const networkTimeSeriesSettings: StationMetricMapSettings = createDefaultNetworkTimeSeriesSettings();
	const administrationRankingSettings: AdministrationRankingSettings = createDefaultAdministrationRankingSettings();
	const lineRankingSettings: LineRankingSettings = createDefaultLineRankingSettings();

	return {
		stationMetricMap: {
			promise: loadStationMetricMap({
				request: createStationMetricMapRequest(stationMetricMapSettings),
				userIp: getClientAddress()
			}),
			settings: stationMetricMapSettings
		},
		networkTimeSeries: {
			promise: loadMetric({
				request: createNetworkTimeSeriesRequest(networkTimeSeriesSettings),
				userIp: getClientAddress()
			}),
			settings: networkTimeSeriesSettings
		},
		administrationRanking: {
			promise: loadMetric({
				request: createAdministrationRankingRequest(administrationRankingSettings),
				userIp: getClientAddress()
			}),
			settings: administrationRankingSettings
		},
		lineRanking: {
			promise: loadMetric({
				request: createLineRankingRequest(lineRankingSettings),
				userIp: getClientAddress()
			}),
			settings: lineRankingSettings
		}
	};
};
