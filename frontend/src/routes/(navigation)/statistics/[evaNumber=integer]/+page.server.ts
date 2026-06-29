import { error } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
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
import {
	createNetworkTimeSeriesPromises,
	type NetworkTimeSeriesPromises
} from "@lib/components/statistics/global/network-time-series/network-time-series";
import { loadMetric } from "@lib/remote/metrics.remote";
import { getStationByEvaNumber, getStationGatheringInfo } from "@lib/remote/station.remote";
import type { Station, StationGatheringInfo } from "@lib/api";

export const load: PageServerLoad = async ({
	getClientAddress,
	params,
	url
}): Promise<{
	station: Station;
	gatheringPromise: Promise<StationGatheringInfo>;
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
	evaNumber: number;
}> => {
	const evaNumber = Number(params.evaNumber);
	if (!Number.isInteger(evaNumber)) error(404, "Station not found");

	const scopeSettings = createStatisticsScopeSettingsFromSearchParams(url.searchParams);
	const userIp = getClientAddress();

	let station: Station;
	try {
		station = await getStationByEvaNumber({ evaNumber, userIp });
	} catch (stationError) {
		if (stationError instanceof Error && stationError.message.includes("404")) error(404, "Station not found");
		throw stationError;
	}

	const stationScope = [evaNumber];
	const administrationRankingSettings = createDefaultAdministrationRankingSettings();
	const administrationRankingRequest = createAdministrationRankingRequest(
		administrationRankingSettings,
		scopeSettings,
		stationScope
	);

	const lineRankingSettings = createDefaultLineRankingSettings();
	const lineRankingRequest = createLineRankingRequest(lineRankingSettings, scopeSettings, stationScope);

	return {
		station,
		gatheringPromise: getStationGatheringInfo({ evaNumber }),
		networkQuality: {
			promises: createNetworkTimeSeriesPromises(scopeSettings, userIp, evaNumber)
		},
		administrationRanking: {
			promise: loadMetric({ request: administrationRankingRequest, userIp }),
			settings: administrationRankingSettings
		},
		lineRanking: {
			promise: loadMetric({ request: lineRankingRequest, userIp }),
			settings: lineRankingSettings
		},
		scope: scopeSettings,
		evaNumber
	};
};
